using CarAuction.Application.Interfaces.HashService;
using CarAuction.Domain.Entities;
using CarAuction.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace CarAuction.Infrastructure.Services.CronJobService
{
    public class ImportAuctionSetting
    {
        private readonly CarAuctionDbContext _db;
        private readonly ILogger<ImportAuctionSetting> _logger;
        private readonly IHashService _hash;

        public ImportAuctionSetting(CarAuctionDbContext db, ILogger<ImportAuctionSetting> logger, IHashService hash)
        {
            _db = db;
            _logger = logger;
            _hash = hash;
        }
        public async Task ImportAuctionSettingAsync()
        {
            var filePath = "C:\\Users\\CuongPC10\\Desktop\\OJT_Training\\backend\\Car_Auction\\CarAuction.Infrastructure\\LoadData\\auctionSetting.json";
            if (!File.Exists(filePath))
            {
                _logger.LogWarning("auctionSetting.json not found at {path}", filePath);
                return;
            }

            var json = await File.ReadAllTextAsync(filePath);
            var root = JsonDocument.Parse(json).RootElement;

            using var tx = await _db.Database.BeginTransactionAsync();

            try
            {
                // 1) Upsert SaleChannels (name → upsert)
                var saleChannelsArray = root.GetProperty("saleChannel");
                foreach (var sc in saleChannelsArray.EnumerateArray())
                {
                    var name = sc.GetProperty("name").GetString()?.Trim();
                    var price = sc.GetProperty("pricePercentage").GetDecimal();
                    var buyNow = sc.TryGetProperty("buyItNowPercentage", out var binp) ? binp.GetDecimal() : (decimal?)null;

                    var existing = await _db.SaleChannels.FirstOrDefaultAsync(x => x.Name == name);
                    if (existing == null)
                    {
                        _db.SaleChannels.Add(new SaleChannel
                        {
                            Id = Guid.NewGuid(),
                            Name = name,
                            PricePercentage = price,
                            BuyItNowPercentage = buyNow
                        });
                    }
                    else
                    {
                        bool changed = existing.PricePercentage != price || existing.BuyItNowPercentage != buyNow;
                        if (changed)
                        {
                            existing.PricePercentage = price;
                            existing.BuyItNowPercentage = buyNow;
                            _db.SaleChannels.Update(existing);
                        }
                    }
                }

                await _db.SaveChangesAsync();

                // 2) Upsert Tactics + Criteria + Steps (hash-based reconciliation)
                var tacticsArray = root.GetProperty("tactics");
                foreach (var tacticItem in tacticsArray.EnumerateArray())
                {
                    var tacticName = tacticItem.GetProperty("name").GetString()?.Trim();

                    // match tactic by Name (business key)
                    var tactic = await _db.Tactics
                        .Include(t => t.Criterias)
                        .Include(t => t.Steps)
                        .FirstOrDefaultAsync(t => t.Name == tacticName);

                    if (tactic == null)
                    {
                        tactic = new Tactic
                        {
                            Id = Guid.NewGuid(),
                            Name = tacticName,
                            CreatedAt = DateTime.UtcNow,
                            Criterias = new List<Criteria>(),
                            Steps = new List<Step>()
                        };
                        _db.Tactics.Add(tactic);
                        await _db.SaveChangesAsync();
                    }

                    // --- CRITERIA reconcile by signature(FieldName|Operator|Value)
                    var existingCriteria = await _db.Criteria.Where(c => c.TacticId == tactic.Id).ToListAsync();

                    var jsonCriterias = new List<(string field, string op, string val, string sig, string hash)>();
                    foreach (var c in tacticItem.GetProperty("criteria").EnumerateArray())
                    {
                        var field = c.GetProperty("fieldName").GetString()?.Trim() ?? "";
                        var op = c.GetProperty("operator").GetString()?.Trim() ?? "";
                        var val = c.GetProperty("value").GetString()?.Trim() ?? "";
                        var sig = $"{field}|{op}|{val}".ToLowerInvariant();
                        var chash = _hash.ComputeMD5(sig);
                        jsonCriterias.Add((field, op, val, sig, chash));
                    }

                    var dbCriteriaSigs = existingCriteria
                        .Select(ec =>
                        {
                            var sig = $"{ec.FieldName?.Trim()}|{ec.Operator?.Trim()}|{ec.Value?.Trim()}".ToLowerInvariant();
                            return (Entity: ec, Sig: sig, Hash: _hash.ComputeMD5(sig));
                        })
                        .ToList();

                    // Add / Update
                    foreach (var jc in jsonCriterias)
                    {
                        var match = dbCriteriaSigs.FirstOrDefault(x => x.Sig == jc.sig);
                        if (match.Entity == null)
                        {
                            // not exists -> add
                            var newC = new Criteria
                            {
                                Id = Guid.NewGuid(),
                                TacticId = tactic.Id,
                                FieldName = jc.field,
                                Operator = jc.op,
                                Value = jc.val
                            };
                            _db.Criteria.Add(newC);

                            _db.CriteriaHashLogs.Add(new CriteriaHashLog
                            {
                                Id = Guid.NewGuid(),
                                CriteriaId = newC.Id,
                                MD5Hash = jc.hash,
                                LastUpdatedAt = DateTime.UtcNow
                            });
                        }
                        else
                        {
                            // exists same signature => ensure hash log exists
                            var clog = await _db.CriteriaHashLogs.FirstOrDefaultAsync(h => h.CriteriaId == match.Entity.Id);
                            if (clog == null || clog.MD5Hash != jc.hash)
                            {
                                if (clog == null)
                                {
                                    _db.CriteriaHashLogs.Add(new CriteriaHashLog
                                    {
                                        Id = Guid.NewGuid(),
                                        CriteriaId = match.Entity.Id,
                                        MD5Hash = jc.hash,
                                        LastUpdatedAt = DateTime.UtcNow
                                    });
                                }
                                else
                                {
                                    clog.MD5Hash = jc.hash;
                                    clog.LastUpdatedAt = DateTime.UtcNow;
                                    _db.CriteriaHashLogs.Update(clog);
                                }
                            }
                        }
                    }

                    // Delete criteria that disappeared from JSON
                    var jsonSigSet = jsonCriterias.Select(x => x.sig).ToHashSet();
                    var toDeleteCriteria = dbCriteriaSigs.Where(x => !jsonSigSet.Contains(x.Sig)).Select(x => x.Entity).ToList();
                    if (toDeleteCriteria.Count > 0)
                    {
                        var delIds = toDeleteCriteria.Select(x => x.Id).ToList();
                        var delLogs = await _db.CriteriaHashLogs.Where(h => delIds.Contains(h.CriteriaId)).ToListAsync();
                        _db.CriteriaHashLogs.RemoveRange(delLogs);
                        _db.Criteria.RemoveRange(toDeleteCriteria);
                    }

                    await _db.SaveChangesAsync();

                    // --- STEPS reconcile by StepNumber (update SaleChannel if changed)
                    var existingSteps = await _db.Steps.Where(s => s.TacticId == tactic.Id).ToListAsync();

                    var jsonSteps = new List<(int stepNo, Guid saleChannelId, string sig, string hash)>();
                    foreach (var s in tacticItem.GetProperty("steps").EnumerateArray())
                    {
                        var stepNo = s.GetProperty("stepNumber").GetInt32();
                        var saleChannelName = s.GetProperty("saleChannelName").GetString()?.Trim();

                        var saleChannel = await _db.SaleChannels.FirstOrDefaultAsync(sc => sc.Name == saleChannelName);
                        if (saleChannel == null)
                        {
                            _logger.LogWarning("SaleChannel '{sc}' not found for tactic '{t}' step {n}", saleChannelName, tactic.Name, stepNo);
                            continue;
                        }

                        var sig = $"{stepNo}|{saleChannel.Id}".ToLowerInvariant();
                        var shash = _hash.ComputeMD5(sig);
                        jsonSteps.Add((stepNo, saleChannel.Id, sig, shash));
                    }

                    var dbStepMap = existingSteps.ToDictionary(x => x.StepNumber, x => x);
                    // Add/Update by step number
                    foreach (var js in jsonSteps)
                    {
                        if (!dbStepMap.TryGetValue(js.stepNo, out var dbStep))
                        {
                            // new step
                            var newStep = new Step
                            {
                                Id = Guid.NewGuid(),
                                TacticId = tactic.Id,
                                StepNumber = js.stepNo,
                                SaleChannelId = js.saleChannelId
                            };
                            _db.Steps.Add(newStep);

                            _db.StepHashLogs.Add(new StepHashLog
                            {
                                Id = Guid.NewGuid(),
                                StepId = newStep.Id,
                                MD5Hash = js.hash,
                                LastUpdatedAt = DateTime.UtcNow
                            });
                        }
                        else
                        {
                            // existing: update SaleChannel if changed
                            if (dbStep.SaleChannelId != js.saleChannelId)
                            {
                                dbStep.SaleChannelId = js.saleChannelId;
                                _db.Steps.Update(dbStep);
                            }

                            var sig = $"{dbStep.StepNumber}|{dbStep.SaleChannelId}".ToLowerInvariant();
                            var hash = _hash.ComputeMD5(sig);

                            var slog = await _db.StepHashLogs.FirstOrDefaultAsync(h => h.StepId == dbStep.Id);
                            if (slog == null)
                            {
                                _db.StepHashLogs.Add(new StepHashLog
                                {
                                    Id = Guid.NewGuid(),
                                    StepId = dbStep.Id,
                                    MD5Hash = hash,
                                    LastUpdatedAt = DateTime.UtcNow
                                });
                            }
                            else if (slog.MD5Hash != hash)
                            {
                                slog.MD5Hash = hash;
                                slog.LastUpdatedAt = DateTime.UtcNow;
                                _db.StepHashLogs.Update(slog);
                            }
                        }
                    }

                    // Delete steps missing in JSON
                    var jsonStepNoSet = jsonSteps.Select(x => x.stepNo).ToHashSet();
                    var toDeleteSteps = existingSteps.Where(s => !jsonStepNoSet.Contains(s.StepNumber)).ToList();
                    if (toDeleteSteps.Count > 0)
                    {
                        var delIds = toDeleteSteps.Select(x => x.Id).ToList();
                        var delLogs = await _db.StepHashLogs.Where(h => delIds.Contains(h.StepId)).ToListAsync();
                        _db.StepHashLogs.RemoveRange(delLogs);
                        _db.Steps.RemoveRange(toDeleteSteps);
                    }

                    await _db.SaveChangesAsync();

                    // --- TAC TIC hash (bao gồm Name + hashes con để detect tổng thể)
                    var criteriaHashes = await _db.Criteria
                        .Where(c => c.TacticId == tactic.Id)
                        .Select(c => new
                        {
                            Sig = (c.FieldName + "|" + c.Operator + "|" + c.Value).ToLower()
                        })
                        .ToListAsync();

                    var stepsHashes = await _db.Steps
                        .Where(s => s.TacticId == tactic.Id)
                        .Select(s => new
                        {
                            Sig = (s.StepNumber.ToString() + "|" + s.SaleChannelId.ToString()).ToLower()
                        })
                        .ToListAsync();

                    var tacticSig =
                        $"{tactic.Name?.Trim().ToLowerInvariant()}|C:{string.Join(",", criteriaHashes.Select(x => _hash.ComputeMD5(x.Sig)).OrderBy(x => x))}|S:{string.Join(",", stepsHashes.Select(x => _hash.ComputeMD5(x.Sig)).OrderBy(x => x))}";
                    var tacticHash = _hash.ComputeMD5(tacticSig);

                    var tlog = await _db.TacticHashLogs.FirstOrDefaultAsync(h => h.TacticId == tactic.Id);
                    if (tlog == null)
                    {
                        _db.TacticHashLogs.Add(new TacticHashLog
                        {
                            Id = Guid.NewGuid(),
                            TacticId = tactic.Id,
                            MD5Hash = tacticHash,
                            LastUpdatedAt = DateTime.UtcNow
                        });
                    }
                    else if (tlog.MD5Hash != tacticHash)
                    {
                        tlog.MD5Hash = tacticHash;
                        tlog.LastUpdatedAt = DateTime.UtcNow;
                        _db.TacticHashLogs.Update(tlog);
                    }

                    await _db.SaveChangesAsync();
                }

                await tx.CommitAsync();
                _logger.LogInformation("ImportAuctionSetting completed successfully (idempotent with hashes).");
            }
            catch (Exception ex)
            {
                await tx.RollbackAsync();
                _logger.LogError(ex, "ImportAuctionSetting failed.");
                throw;
            }
        }
    }
}


