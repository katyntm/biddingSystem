using CarAuction.Domain.Entities;
using CarAuction.Domain.Interfaces.UnitOfWork;
using CarAuction.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.IO.Compression;

namespace CarAuction.Infrastructure.Services.CronJobService
{
  public class VehicleImageImportService
  {
    private readonly ILogger<VehicleImageImportService> _logger;
    private readonly string _zipDirectory;
    private readonly string _imagesDirectory;
    private readonly IUnitOfWork _uow;

    public VehicleImageImportService(
      IUnitOfWork uow,
    ILogger<VehicleImageImportService> logger)
    {
      _uow = uow;
      _logger = logger;

      var loadDataPath = Path.Combine(Directory.GetCurrentDirectory(), "LoadData");
      _zipDirectory = Path.Combine(loadDataPath, "ZipImages");
      _imagesDirectory = Path.Combine(loadDataPath, "Images");

      // Ensure directories exist
      Directory.CreateDirectory(_zipDirectory);
      Directory.CreateDirectory(_imagesDirectory);
    }

    public async Task ImportVehicleImagesAsync()
    {
      try
      {
        var zipFiles = Directory.GetFiles(_zipDirectory, "*.zip");
        _logger.LogInformation($"Found {zipFiles.Length} ZIP files to process");

        foreach (var zipFile in zipFiles)
        {
          var fileName = Path.GetFileNameWithoutExtension(zipFile);

          // The file name should be the VIN
          var vin = fileName;

          // Find vehicle with this VIN
          var vehicle = await _uow.Vehicles.GetByVinAsync(vin);

          if (vehicle == null)
          {
            _logger.LogWarning($"No vehicle found with VIN {vin}, skipping image import");
            continue;
          }

          await ProcessVehicleImagesAsync(zipFile, vehicle);
        }

      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error during image import");
        throw;
      }
    }

    private async Task ProcessVehicleImagesAsync(string zipFilePath, Vehicle vehicle)
    {
      try
      {
        // Create vehicle directory if it doesn't exist
        var vehicleImagesPath = Path.Combine(_imagesDirectory, vehicle.VIN);
        Directory.CreateDirectory(vehicleImagesPath);

        // Extract images from ZIP file
        using var archive = ZipFile.OpenRead(zipFilePath);

        var imageFiles = new List<string>();
        foreach (var entry in archive.Entries)
        {
          // Skip directories and hidden files
          if (string.IsNullOrEmpty(entry.Name) || entry.Name.StartsWith("."))
            continue;

          var extension = Path.GetExtension(entry.Name).ToLowerInvariant();
          if (IsImageFile(extension))
          {
            var destinationPath = Path.Combine(vehicleImagesPath, entry.Name);
            entry.ExtractToFile(destinationPath, overwrite: true);
            imageFiles.Add(destinationPath);
          }
        }

        // Remove existing images for this vehicle
        var existingImages = (await _uow.VehicleImages.GetByVehicleIdAsync(vehicle.Id)).ToList();

        if (existingImages.Any())
        {
          _uow.VehicleImages.DeleteRange(existingImages);
        }

        // Add new images
        if (imageFiles.Any())
        {
          var vehicleImages = imageFiles.Select(file => new VehicleImage
          {
            Id = Guid.NewGuid(),
            VehicleId = vehicle.Id,
            Url = $"/LoadData/Images/{vehicle.VIN}/{Path.GetFileName(file)}",
            CreatedAt = DateTime.UtcNow,
            Vehicle = vehicle
          }).ToList();

          await _uow.VehicleImages.AddRangeAsync(vehicleImages);
          await _uow.SaveChangesAsync();
          _logger.LogInformation($"Added {vehicleImages.Count} images for vehicle {vehicle.VIN}");
        }
        else
        {
          _logger.LogWarning($"No valid images found in ZIP for vehicle {vehicle.VIN}");
        }

        // Move processed ZIP to archive folder
        var archiveFolder = Path.Combine(_zipDirectory, "Processed");
        Directory.CreateDirectory(archiveFolder);


        var archiveFilePath = Path.Combine(archiveFolder, Path.GetFileName(zipFilePath));
        File.Move(zipFilePath, archiveFilePath, overwrite: true);
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, $"Error processing images for vehicle {vehicle.VIN}");
      }
    }

    private bool IsImageFile(string extension)
    {
      return new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp" }.Contains(extension);
    }
  }
}