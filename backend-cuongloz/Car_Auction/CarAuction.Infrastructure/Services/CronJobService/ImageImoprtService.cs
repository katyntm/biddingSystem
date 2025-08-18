using CarAuction.Infrastructure.Mock;
using CarAuction.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.IO.Compression;

namespace CarAuction.Infrastructure.Services.CronJobService
{
    public class ImageImportService
    {
        private readonly CarAuctionDbContext _context;
        private readonly ILogger<ImageImportService> _logger;
        private readonly string _wwwrootPath;
        private readonly string _zipImportPath;

        public ImageImportService(CarAuctionDbContext context, ILogger<ImageImportService> logger)
        {
            _context = context;
            _logger = logger;
            _wwwrootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
            _zipImportPath = Path.Combine(Directory.GetCurrentDirectory(), "LoadData", "car_images");

            // Ensure directories exist
            Directory.CreateDirectory(_wwwrootPath);
            Directory.CreateDirectory(_zipImportPath);
            Directory.CreateDirectory(Path.Combine(_zipImportPath, "processed"));
            Directory.CreateDirectory(Path.Combine(_wwwrootPath, "placeholder"));
        }

        public async Task ImportVehicleImagesAsync()
        {
            try
            {
                // Create placeholder image if it doesn't exist
                await EnsurePlaceholderImageExists();

                // Get all ZIP files from import directory
                var zipFiles = Directory.GetFiles(_zipImportPath, "*.zip");
                var processedCount = 0;
                var errorCount = 0;

                foreach (var zipFile in zipFiles)
                {
                    try
                    {
                        var fileName = Path.GetFileNameWithoutExtension(zipFile);
                        
                        // Extract VIN from filename (assuming filename is VIN.zip)
                        var vin = fileName;

                        // Validate VIN format
                        if (!IsValidVin(vin))
                        {
                            _logger.LogWarning($"Invalid VIN in filename: {fileName}");
                            errorCount++;
                            continue;
                        }

                        // Check if vehicle exists
                        var vehicle = await _context.Set<Vehicle>()
                            .FirstOrDefaultAsync(v => v.VIN == vin);

                        if (vehicle == null)
                        {
                            _logger.LogWarning($"Vehicle with VIN {vin} not found in database, skipping image import");
                            errorCount++;
                            continue;
                        }

                        await ProcessVehicleImages(zipFile, vehicle);
                        processedCount++;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"Error processing ZIP file {zipFile}");
                        errorCount++;
                    }
                }

                // Set placeholder images for vehicles without images
                await SetPlaceholderForVehiclesWithoutImages();

                await _context.SaveChangesAsync();
                _logger.LogInformation($"Vehicle image import completed. Processed: {processedCount}, Errors: {errorCount}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error importing vehicle images");
                throw;
            }
        }

        private async Task ProcessVehicleImages(string zipFilePath, Vehicle vehicle)
        {
            try
            {
                var vinImageDirectory = Path.Combine(_wwwrootPath, vehicle.VIN);
                Directory.CreateDirectory(vinImageDirectory);

                // Remove existing images for this vehicle from database
                var existingImages = await _context.Set<VehicleImage>()
                    .Where(vi => vi.VehicleId == vehicle.Id)
                    .ToListAsync();

                if (existingImages.Any())
                {
                    _context.Set<VehicleImage>().RemoveRange(existingImages);
                }

                // Clear existing files in the directory
                if (Directory.Exists(vinImageDirectory))
                {
                    foreach (var file in Directory.GetFiles(vinImageDirectory))
                    {
                        File.Delete(file);
                    }
                }

                // Extract ZIP file
                using var archive = ZipFile.OpenRead(zipFilePath);
                
                var imageExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp" };
                var processedImages = new List<VehicleImage>();

                foreach (var entry in archive.Entries)
                {
                    if (entry.FullName.EndsWith("/") || string.IsNullOrEmpty(entry.Name))
                        continue; // Skip directories

                    var extension = Path.GetExtension(entry.FullName).ToLowerInvariant();
                    
                    if (!imageExtensions.Contains(extension))
                        continue;

                    var safeFileName = GetSafeFileName(entry.Name);
                    var destinationPath = Path.Combine(vinImageDirectory, safeFileName);
                    
                    // Extract file
                    entry.ExtractToFile(destinationPath, overwrite: true);

                    // Create VehicleImage record
                    var vehicleImage = new VehicleImage
                    {
                        Id = Guid.NewGuid(),
                        VehicleId = vehicle.Id,
                        Url = $"/images/{vehicle.VIN}/{safeFileName}",
                        CreatedAt = DateTime.UtcNow,
                        Vehicle = vehicle
                    };

                    processedImages.Add(vehicleImage);
                }

                if (processedImages.Any())
                {
                    // Add new images
                    await _context.Set<VehicleImage>().AddRangeAsync(processedImages);

                    _logger.LogInformation($"Processed {processedImages.Count} images for vehicle {vehicle.VIN}");
                }
                else
                {
                    // No valid images found, will be handled by SetPlaceholderForVehiclesWithoutImages
                    _logger.LogWarning($"No valid images found in ZIP for vehicle {vehicle.VIN}");
                }

                // Move processed ZIP to archive folder
                var processedPath = Path.Combine(_zipImportPath, "processed");
                var archivedZipPath = Path.Combine(processedPath, Path.GetFileName(zipFilePath));
                File.Move(zipFilePath, archivedZipPath, true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error processing images for vehicle {vehicle.VIN}");
                throw;
            }
        }

        private async Task SetPlaceholderForVehiclesWithoutImages()
        {
            // Get all vehicles that don't have any images
            var vehiclesWithoutImages = await _context.Set<Vehicle>()
                .Where(v => !_context.Set<VehicleImage>().Any(vi => vi.VehicleId == v.Id))
                .ToListAsync();

            foreach (var vehicle in vehiclesWithoutImages)
            {
                var placeholderImage = new VehicleImage
                {
                    Id = Guid.NewGuid(),
                    VehicleId = vehicle.Id,
                    Url = "/images/placeholder/no-image.jpg",
                    CreatedAt = DateTime.UtcNow,
                    Vehicle = vehicle
                };

                await _context.Set<VehicleImage>().AddAsync(placeholderImage);
            }

            if (vehiclesWithoutImages.Any())
            {
                _logger.LogInformation($"Set placeholder images for {vehiclesWithoutImages.Count} vehicles without images");
            }
        }

        private async Task EnsurePlaceholderImageExists()
        {
            var placeholderPath = Path.Combine(_wwwrootPath, "placeholder", "no-image.jpg");
            
            if (!File.Exists(placeholderPath))
            {
                // Create a simple placeholder image file
                Directory.CreateDirectory(Path.GetDirectoryName(placeholderPath));
                
                // Create a minimal placeholder file (you would replace this with an actual image)
                await File.WriteAllTextAsync(placeholderPath + ".txt", "Placeholder for no-image.jpg");
                _logger.LogInformation("Created placeholder image marker");
            }
        }

        private string GetSafeFileName(string fileName)
        {
            // Remove or replace invalid characters for file names
            var invalidChars = Path.GetInvalidFileNameChars();
            var safeName = fileName;
            
            foreach (var invalidChar in invalidChars)
            {
                safeName = safeName.Replace(invalidChar, '_');
            }
            
            return safeName;
        }

        private bool IsValidVin(string vin)
        {
            if (string.IsNullOrWhiteSpace(vin) || vin.Length != 17)
                return false;

            return vin.All(c => char.IsLetterOrDigit(c)) && 
                   vin.Any(char.IsLetter) && 
                   vin.Any(char.IsDigit);
        }
    }
}