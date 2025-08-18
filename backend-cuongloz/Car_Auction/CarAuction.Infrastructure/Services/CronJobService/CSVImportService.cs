using CarAuction.Infrastructure.Mock;
using CarAuction.Infrastructure.Persistence;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Globalization;
using System.Text.RegularExpressions;

namespace CarAuction.Infrastructure.Services.CronJobService
{
    public class CsvImportService
    {
        private readonly CarAuctionDbContext _context;
        private readonly ILogger<CsvImportService> _logger;
        private readonly string _csvImportPath;

        public CsvImportService(CarAuctionDbContext context, ILogger<CsvImportService> logger)
        {
            _context = context;
            _logger = logger;
            _csvImportPath = Path.Combine(Directory.GetCurrentDirectory(), "LoadData");
            
            // Ensure directory exists
            Directory.CreateDirectory(_csvImportPath);
        }

        public async Task ImportVehiclesFromCsvAsync()
        {
            var csvPath = Path.Combine(_csvImportPath, "vehicle_inventory.csv");
            
            if (!File.Exists(csvPath))
            {
                _logger.LogWarning($"CSV file not found at: {csvPath}");
                return;
            }

            var processedVehicles = new List<Vehicle>();
            var duplicateVins = new HashSet<string>();
            var processedVins = new HashSet<string>();
            var skippedCount = 0;

            try
            {
                using var reader = new StringReader(await File.ReadAllTextAsync(csvPath));
                using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    HasHeaderRecord = true,
                    TrimOptions = TrimOptions.Trim,
                    MissingFieldFound = null // Ignore missing fields
                });

                // Read header first to validate structure
                await csv.ReadAsync();
                csv.ReadHeader();
                
                var requiredColumns = new[] { "VIN", "Make", "ModelYear", "ModelType", "Price" };
                var headers = csv.HeaderRecord;
                
                foreach (var required in requiredColumns)
                {
                    if (!headers.Contains(required, StringComparer.OrdinalIgnoreCase))
                    {
                        _logger.LogError($"Required column '{required}' not found in CSV file");
                        return;
                    }
                }

                // Get existing VINs from database to check duplicates
                var existingVins = await _context.Set<Vehicle>()
                    .Select(v => v.VIN)
                    .ToHashSetAsync();

                while (await csv.ReadAsync())
                {
                    try
                    {
                        var record = csv.GetRecord<VehicleCsvRecord>();
                        
                        if (record == null) continue;

                        // Validate VIN
                        if (!IsValidVin(record.VIN))
                        {
                            _logger.LogWarning($"Invalid VIN: {record.VIN}. Must be 17 alphanumeric characters.");
                            skippedCount++;
                            continue;
                        }

                        // Check for duplicate VINs in current batch
                        if (processedVins.Contains(record.VIN))
                        {
                            duplicateVins.Add(record.VIN);
                            _logger.LogWarning($"Duplicate VIN found in CSV: {record.VIN}");
                            skippedCount++;
                            continue;
                        }

                        // Check if VIN already exists in database
                        if (existingVins.Contains(record.VIN))
                        {
                            _logger.LogInformation($"Vehicle with VIN {record.VIN} already exists in database, skipping.");
                            skippedCount++;
                            continue;
                        }

                        // Validate required columns
                        if (string.IsNullOrWhiteSpace(record.Make) ||
                            string.IsNullOrWhiteSpace(record.ModelType) ||
                            record.ModelYear <= 0 ||
                            record.Price <= 0)
                        {
                            _logger.LogWarning($"Required columns missing for VIN: {record.VIN}");
                            skippedCount++;
                            continue;
                        }

                        var vehicle = new Vehicle
                        {
                            Id = Guid.NewGuid(),
                            VIN = record.VIN,
                            Make = record.Make,
                            ModelYear = record.ModelYear,
                            FuelType = string.IsNullOrWhiteSpace(record.FuelType) ? "Unknown" : record.FuelType,
                            ModelType = record.ModelType,
                            Transmission = string.IsNullOrWhiteSpace(record.Transmission) ? "Unknown" : record.Transmission,
                            BodyStyle = string.IsNullOrWhiteSpace(record.BodyStyle) ? "Unknown" : record.BodyStyle,
                            Color = string.IsNullOrWhiteSpace(record.Color) ? "Unknown" : record.Color,
                            Grade = record.Grade,
                            Price = record.Price,
                            Location = string.IsNullOrWhiteSpace(record.Location) ? "Unknown" : record.Location,
                            CreatedAt = DateTime.UtcNow,
                            UpdatedAt = DateTime.UtcNow,
                            VehicleImages = new List<VehicleImage>()
                        };

                        processedVehicles.Add(vehicle);
                        processedVins.Add(record.VIN);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, $"Error processing CSV row: {ex.Message}");
                        skippedCount++;
                    }
                }

                if (processedVehicles.Any())
                {
                    await _context.Set<Vehicle>().AddRangeAsync(processedVehicles);
                    await _context.SaveChangesAsync();
                    
                    // Update the JSON file for compatibility with existing LoadAuctionVehicle logic
                    await UpdateVehicleJsonFile();
                    
                    _logger.LogInformation($"Successfully imported {processedVehicles.Count} vehicles from CSV");
                }

                if (duplicateVins.Any())
                {
                    _logger.LogWarning($"Found {duplicateVins.Count} duplicate VINs in CSV: {string.Join(", ", duplicateVins)}");
                }

                if (skippedCount > 0)
                {
                    _logger.LogInformation($"Skipped {skippedCount} records due to validation errors or duplicates");
                }

                _logger.LogInformation($"CSV import completed. Processed: {processedVehicles.Count}, Skipped: {skippedCount}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error importing vehicles from CSV");
                throw;
            }
        }

        private async Task UpdateVehicleJsonFile()
        {
            try
            {
                var vehicles = await _context.Set<Vehicle>().ToListAsync();
                var jsonPath = Path.Combine(_csvImportPath, "mockVehiclesTable.json");
                
                var jsonContent = System.Text.Json.JsonSerializer.Serialize(vehicles, new System.Text.Json.JsonSerializerOptions 
                { 
                    WriteIndented = true,
                    PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase
                });
                
                await File.WriteAllTextAsync(jsonPath, jsonContent);
                _logger.LogInformation("Updated vehicle JSON file for LoadAuctionVehicle compatibility");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating vehicle JSON file");
            }
        }

        private bool IsValidVin(string vin)
        {
            if (string.IsNullOrWhiteSpace(vin) || vin.Length != 17)
                return false;

            // Check if VIN contains only alphanumeric characters
            var alphanumericRegex = new Regex("^[A-Za-z0-9]+$");
            if (!alphanumericRegex.IsMatch(vin))
                return false;

            // Ensure it has at least one letter and one number
            var hasLetter = vin.Any(char.IsLetter);
            var hasDigit = vin.Any(char.IsDigit);

            return hasLetter && hasDigit;
        }
    }

    public class VehicleCsvRecord
    {
        public string VIN { get; set; } = string.Empty;
        public string Make { get; set; } = string.Empty;
        public int ModelYear { get; set; }
        public string? FuelType { get; set; }
        public string ModelType { get; set; } = string.Empty;
        public string? Transmission { get; set; }
        public string? BodyStyle { get; set; }
        public string? Color { get; set; }
        public decimal Grade { get; set; }
        public decimal Price { get; set; }
        public string? Location { get; set; }
    }
}