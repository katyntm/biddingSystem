using CarAuction.Domain.Interfaces.UnitOfWork;
using CarAuction.Infrastructure.Jobs;
using CarAuction.Infrastructure.Persistence;
using CarAuction.Infrastructure.Services.CronJobService;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using System.Text.Json;

namespace CarAuction.Infrastructure.DI
{
    public static class DependencyInjection
    {

        public static async Task<IServiceCollection> AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // var filePath = "C:\\Users\\CuongPC10\\Desktop\\OJT_Training\\backend\\Car_Auction\\CarAuction.Infrastructure\\LoadData\\auctionSetting.json";
            // relative file path
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "LoadData", "auctionSetting.json");
            var json = await File.ReadAllTextAsync(filePath);
            var root = JsonDocument.Parse(json).RootElement;

            var startTime = root.GetProperty("auctionSession").GetProperty("startTime").GetDateTime();
            var endTime = root.GetProperty("auctionSession").GetProperty("endTime").GetDateTime();

            services.AddDbContext<CarAuctionDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
            });

            services.AddQuartz(options =>
            {
                options.UseMicrosoftDependencyInjectionJobFactory();

                options.UsePersistentStore(persistenceOptions =>
                {
                    persistenceOptions.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
                    persistenceOptions.UseNewtonsoftJsonSerializer();
                    persistenceOptions.UseProperties = true;
                });

                var startJobKey = new JobKey("AuctionStartJob");
                options.AddJob<AuctionStartJob>(job => job.WithIdentity(startJobKey));

                options.AddTrigger(trigger => trigger
                    .ForJob(startJobKey)
                    .WithIdentity("AuctionStartJobTrigger")
                    .WithCronSchedule($"0 0 {startTime.Hour} * * ?")
                );

                var endJobKey = new JobKey("AuctionEndJob");
                options.AddJob<AuctionEndJob>(job => job.WithIdentity(endJobKey));

                options.AddTrigger(trigger => trigger
                    .ForJob(endJobKey)
                    .WithIdentity("AuctionEndJobTrigger")
                    .WithCronSchedule($"0 0 {endTime.Hour} * * ?")
                );

                // Vehicle Import Job
                var vehicleImportJobKey = new JobKey("VehicleImportJob");
                options.AddJob<VehicleImportJob>(job => job.WithIdentity(vehicleImportJobKey));
                options.AddTrigger(trigger => trigger
                    .ForJob(vehicleImportJobKey)
                    .WithIdentity("VehicleImportJobTrigger")
                    .WithCronSchedule("0 */5 0 * * * ?") // 5 minutes interval
                );
            });

            services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);

            services.AddScoped<IUnitOfWork, UnitOfWork.UnitOfWork>();
            services.AddScoped<ImportAuctionSetting>();
            services.AddScoped<LoadVehicleInventory>();
            services.AddScoped<LoadAuctionVehicle>();
            services.AddScoped<moveNextSession>();
            services.AddScoped<AuctionStartJob>();
            services.AddScoped<AuctionEndJob>();

            // CSV Import Service
            services.AddScoped<CsvImportService>();
            services.AddScoped<ImageImportService>();
            services.AddScoped<VehicleImportJob>();

            return services;
        }
    }
}
