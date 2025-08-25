using CarAuction.Application.Interfaces.Services;
using CarAuction.Application.Services;
using CarAuction.Application.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace CarAuction.Application.DI
{
  public static class DependencyInjection
  {

    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
      // Register application services
      services.AddScoped<IAuthService, AuthService>();
      services.AddScoped<IAuctionService, AuctionService>();

      // Register SignalR
      services.AddSignalR();
      
      return services;
    }
  }
}
