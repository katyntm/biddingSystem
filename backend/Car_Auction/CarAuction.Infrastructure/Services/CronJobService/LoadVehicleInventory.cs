using CarAuction.Infrastructure.Mock;
using CarAuction.Infrastructure.Persistence;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace CarAuction.Infrastructure.Services.CronJobService
{
    public class LoadVehicleInventory
    {
        private readonly CarAuctionDbContext _db;
        private readonly ILogger<LoadVehicleInventory> _logger;

        public LoadVehicleInventory(CarAuctionDbContext db, ILogger<LoadVehicleInventory> logger)
        {
            _db = db;
            _logger = logger;
        }

        public bool IsValidVin(string vin)
        {
            return !string.IsNullOrWhiteSpace(vin) && vin.Length == 17 && vin.Any(char.IsLetter) && vin.Any(char.IsDigit);
        }

        public async Task LoadVehicleInventoryAsync()
        {
            var filePath = "C:\\Users\\CuongPC10\\Desktop\\OJT_Training\\backend\\Car_Auction\\CarAuction.Infrastructure\\LoadData\\vehicle_inventory.csv";
            var vehicles = new List<Vehicle>();

            if (!File.Exists(filePath))
            {
                _logger.LogWarning($"File CSV not found: {filePath}");
                return;
            }

            using var reader = new StreamReader(filePath);
            await reader.ReadLineAsync(); // skip header

            while (!reader.EndOfStream)
            {
                var line = await reader.ReadLineAsync();
                if (string.IsNullOrEmpty(line)) continue;

                var columns = line.Split(',');
                var vin = columns[0].Trim();
                if (!IsValidVin(vin))
                {
                    _logger.LogWarning($"Invalid VIN: {vin}");
                    continue;
                }

                vehicles.Add(new Vehicle
                {
                    Id = Guid.NewGuid(),
                    VIN = vin,
                    Make = columns[1].Trim(),
                    ModelYear = int.Parse(columns[2].Trim()),
                    FuelType = columns[3].Trim(),
                    ModelType = columns[4].Trim(),
                    Transmission = columns[5].Trim(),
                    BodyStyle = columns[6].Trim(),
                    Color = columns[7].Trim(),
                    Grade = decimal.Parse(columns[8].Trim()),
                    Price = decimal.Parse(columns[9].Trim()),
                    Location = columns[10].Trim(),
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    VehicleImages = new List<VehicleImage>()
                });
            }

            // Ghi ra file JSON
            var jsonPath = Path.Combine(Path.GetDirectoryName(filePath), "mockVehiclesTable.json");
            var jsonContent = JsonSerializer.Serialize(vehicles, new JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(jsonPath, jsonContent);
        }
    }
}
