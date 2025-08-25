using CarAuction.Application.Common.Constants;
using CarAuction.Application.OptionsSetup;
using CarAuction.Domain.Interfaces;
using CarAuction.Domain.Interfaces.UnitOfWork;
using CarAuction.Infrastructure.Jobs;
using CarAuction.Infrastructure.Options;
using CarAuction.Infrastructure.Persistence;
using CarAuction.Infrastructure.Repositories;
using CarAuction.Infrastructure.Services;
using CarAuction.Infrastructure.Services.CronJobService;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Quartz;

namespace CarAuction.Infrastructure.DI
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<CarAuctionDbContext>(o =>
                o.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            // Register repositories
            services.AddScoped<IVehicleRepository, VehicleRepository>();
            services.AddScoped<IVehicleImageRepository, VehicleImageRepository>();

            // Register Unit of Work
            services.AddScoped<IUnitOfWork, UnitOfWork.UnitOfWork>();

            services.AddAuctionSettingsOptions();

            services.AddQuartz(q =>
            {
                q.UsePersistentStore(p =>
                {
                    p.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
                    p.UseNewtonsoftJsonSerializer();
                    p.UseProperties = true;
                });

                var sp = services.BuildServiceProvider();
                var auctionSetting = sp.GetRequiredService<IOptionsMonitor<AuctionSettingOptions>>().CurrentValue;
                // log current settings
                System.Console.WriteLine($"Configuring Quartz Jobs with Start Time: {auctionSetting.AuctionSession.StartTime}, End Time: {auctionSetting.AuctionSession.EndTime}");

                var startJobKey = new JobKey(QuartzConstants.Jobs.AuctionStart, QuartzConstants.Group);
                q.AddJob<AuctionStartJob>(j => j.WithIdentity(startJobKey));
                q.AddTrigger(t => t.ForJob(startJobKey)
                    .WithIdentity(QuartzConstants.Triggers.AuctionStart, QuartzConstants.Group)
                    .WithCronSchedule($"0 {auctionSetting.AuctionSession.StartTime.Minute} {auctionSetting.AuctionSession.StartTime.Hour} * * ?"));

                var endJobKey = new JobKey(QuartzConstants.Jobs.AuctionEnd, QuartzConstants.Group);
                q.AddJob<AuctionEndJob>(j => j.WithIdentity(endJobKey));
                q.AddTrigger(t => t.ForJob(endJobKey)
                    .WithIdentity(QuartzConstants.Triggers.AuctionEnd, QuartzConstants.Group)
                    .WithCronSchedule($"0 {auctionSetting.AuctionSession.EndTime.Minute} {auctionSetting.AuctionSession.EndTime.Hour} * * ?"));

                // Add the Vehicle Import Job to run every 5 minutes
                var vehicleImportJobKey = new JobKey(QuartzConstants.Jobs.VehicleImport, QuartzConstants.Group);
                q.AddJob<VehicleImportJob>(job => job.WithIdentity(vehicleImportJobKey));

                q.AddTrigger(trigger => trigger
                .ForJob(vehicleImportJobKey)
                .WithIdentity(QuartzConstants.Triggers.VehicleImport, QuartzConstants.Group)
                .WithCronSchedule("0 */5 * * * ?") // Run every 5 minutes
                );
            });

            services.AddQuartzHostedService(o => o.WaitForJobsToComplete = true);

            services.AddScoped<IUnitOfWork, UnitOfWork.UnitOfWork>();
            services.AddScoped<ImportAuctionSetting>();
            services.AddScoped<LoadAuctionVehicle>();
            services.AddScoped<MoveNextSession>();
            services.AddScoped<AuctionStartJob>();
            services.AddScoped<AuctionEndJob>();

            // Added services for job execution of vehicle import
            services.AddScoped<VehicleCsvImportService>();
            services.AddScoped<VehicleImageImportService>();

            // Register the Vehicle Import Job for dependency injection
            services.AddScoped<VehicleImportJob>();
            services.AddScoped<AuctionEndService>();

            return services;
        }
    }
}