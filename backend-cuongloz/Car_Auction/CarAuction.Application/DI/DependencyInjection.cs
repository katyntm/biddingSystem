using CarAuction.Application.Interfaces.HashService;
using CarAuction.Application.Services.HashService;
using Microsoft.Extensions.DependencyInjection;

namespace CarAuction.Application.DI
{
    public static class DependencyInjection
    {
        public static async Task<IServiceCollection> AddApplication(this IServiceCollection services)
        {
            services.AddScoped<IHashService, HashService>();
            return services;
        }
    }
}
