using CarAuction.Domain.Entities;
using CarAuction.Domain.Interfaces.UnitOfWork;
using CarAuction.Infrastructure.Persistence;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Globalization;
using System.Text.RegularExpressions;

namespace CarAuction.Infrastructure.Services.CronJobService
{
  public class VehicleCsvImportService
  {
    private readonly ILogger<VehicleCsvImportService> _logger;
    private readonly string _csvPath;
    private readonly IUnitOfWork _uow;

    public VehicleCsvImportService(
    ILogger<VehicleCsvImportService> logger,
    IUnitOfWork uow)
    {
      _logger = logger;
      _csvPath = Path.Combine(Directory.GetCurrentDirectory(), "LoadData", "vehicle_inventory.csv");
      _uow = uow;
    }

    public async Task ImportVehiclesFromCsvAsync()
    {
      // 1. Check if the CSV file exists
      if (!File.Exists(_csvPath))
      {
        _logger.LogWarning($"CSV file not found at {_csvPath}");
        return;
      }

      try
      {
        // 2. Read and Validate CSV
        using var reader = new StreamReader(_csvPath);
        using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
        {
          HasHeaderRecord = true,
          HeaderValidated = null,
          MissingFieldFound = null
        });

        // Validate headers
        csv.Read();
        csv.ReadHeader();
        var headers = csv.HeaderRecord;

        var requiredColumns = new[] { "VIN", "Make", "Model Year", "Model Type", "Price", "Fuel Type", "Body Style", "Color", "Transmission", "Location", "Grade" };
        foreach (var column in requiredColumns)
        {
          if (!headers.Contains(column))
          {
            _logger.LogError($"Required column {column} not found in CSV");
            return;
          }
        }

        // 3. Check for duplicates by fetching existing VINs from the database
        var existingVins = (await _uow.Vehicles
          .GetAllVinsAsync())
          .ToHashSet();

        var processedVins = new HashSet<string>();
        var duplicateVins = new HashSet<string>();
        var newVehicles = new List<Vehicle>();

        // Process each record
        while (csv.Read())
        {
          var vin = csv.GetField("VIN");

          // Validate VIN
          if (!IsValidVin(vin))
          {
            _logger.LogWarning($"Invalid VIN: {vin}. VIN must be 17 characters alphanumeric.");
            continue;
          }

          // Skip duplicate VINs in the CSV itself
          if (processedVins.Contains(vin))
          {
            _logger.LogWarning($"Duplicate VIN in CSV: {vin}");
            duplicateVins.Add(vin);
            continue;
          }

          // Skip duplicate VINs already in the database
          if (existingVins.Contains(vin))
          {
            _logger.LogInformation($"VIN {vin} already exists in database, skipping");
            continue;
          }

          // 4. Process the CSV row and save a new Vehicle entity to database
          try
          {
            if (!ValidateRequiredFields(csv, vin))
            {
              continue;
            }

            // Create a new vehicle entity
            var vehicle = new Vehicle
            {
              Id = Guid.NewGuid(),
              VIN = vin,
              Make = csv.GetField("Make"),
              ModelYear = int.Parse(csv.GetField("Model Year")),
              ModelType = csv.GetField("Model Type"),
              Price = decimal.Parse(csv.GetField("Price")),
              FuelType = csv.GetField("Fuel Type"),
              BodyStyle = csv.GetField("Body Style"),
              Color = csv.GetField("Color"),
              Transmission = csv.GetField("Transmission"),
              Location = csv.GetField("Location"),
              Grade = decimal.Parse(csv.GetField("Grade")),
              CreatedAt = DateTime.UtcNow,
              UpdatedAt = DateTime.UtcNow,
              VehicleImages = new List<VehicleImage>()
            };

            var placeholderImage = new VehicleImage
            {
              Id = Guid.NewGuid(),
              VehicleId = vehicle.Id,
              Url = "/LoadData/Images/placeholder.jpg",
              CreatedAt = DateTime.UtcNow,
              Vehicle = vehicle
            };

            // Add placeholder to vehicle
            for (int i = 0; i < 4; ++i)
            {
              vehicle.VehicleImages.Add(placeholderImage);
            }

            newVehicles.Add(vehicle);
            processedVins.Add(vin);
          }
          catch (Exception ex)
          {
            _logger.LogError(ex, $"Error processing row with VIN {vin}");
          }
        }

        if (newVehicles.Any())
        {
          await _uow.Vehicles.AddRangeAsync(newVehicles);
          await _uow.SaveChangesAsync();
          _logger.LogInformation($"Successfully imported {newVehicles.Count} vehicles");
        }

        // Log any duplicate VINs found in the CSV
        if (duplicateVins.Any())
        {
          _logger.LogWarning($"Found {duplicateVins.Count} duplicate VINs in CSV");
        }
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error importing vehicles from CSV");
        throw;
      }
    }

    private bool ValidateRequiredFields(CsvReader csv, string vin)
    {
      try
      {
        // Check for empty or missing values in required fields
        var requiredFields = new[] {
          "Make", "Model Year", "Model Type", "Price",
          "Fuel Type", "Body Style", "Color", "Transmission",
          "Location", "Grade"
          };

        foreach (var field in requiredFields)
        {
          var value = csv.GetField(field);
          if (string.IsNullOrWhiteSpace(value))
          {
            _logger.LogWarning($"Required field {field} is missing or empty for VIN {vin}");
            return false;
          }
        }

        // Validate numeric fields
        if (!int.TryParse(csv.GetField("Model Year"), out _))
        {
          _logger.LogWarning($"Invalid ModelYear for VIN {vin}: must be a number", csv.GetField("Model Year"));
          return false;
        }

        if (!decimal.TryParse(csv.GetField("Price"), out _))
        {
          _logger.LogWarning($"Invalid Price for VIN {vin}: must be a decimal number");
          return false;
        }

        if (!decimal.TryParse(csv.GetField("Grade"), out _))
        {
          _logger.LogWarning($"Invalid Grade for VIN {vin}: must be a decimal number");
          return false;
        }

        return true;
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, $"Error validating fields for VIN {vin}");
        return false;
      }
    }

    private bool IsValidVin(string vin)
    {
      if (string.IsNullOrWhiteSpace(vin) || vin.Length != 17)
        return false;

      // Check if VIN contains only alphanumeric characters
      return Regex.IsMatch(vin, @"^[A-Za-z0-9]+$");
    }


  }
}