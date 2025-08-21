using CarAuction.Infrastructure.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace CarAuction.Application.OptionsSetup
{
    public sealed class AuctionSettingOptionsSetup : IConfigureOptions<AuctionSettingOptions>
    {
        private readonly IConfiguration _cfg;

        public AuctionSettingOptionsSetup()
        {
            _cfg = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)              
                .AddJsonFile("LoadData/auctionSetting.json", optional: false, reloadOnChange: true)
                .Build();
        }

        public void Configure(AuctionSettingOptions options) => _cfg.Bind(options);
    }

    public static class AuctionOptionsExtensions
    {
        public static IServiceCollection AddAuctionSettingsOptions(this IServiceCollection services)
        {
            services.AddOptions<AuctionSettingOptions>();
            services.AddSingleton<IConfigureOptions<AuctionSettingOptions>, AuctionSettingOptionsSetup>();
            return services;
        }
    }
}