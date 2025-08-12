using CarAuction.Application.Services;
using CarAuction.Application.Services.Interfaces;
using CarAuction.Infrastructure.Persistence;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CarAuction.Infrastructure.DI
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
