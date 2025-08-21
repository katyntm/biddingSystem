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

      return services;
    }
  }
}
