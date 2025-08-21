using CarAuction.Infrastructure.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace CarAuction.Application.OptionsSetup
{
    public class SwaggerGenOptionsSetup : IConfigureOptions<SwaggerGenOptions>
    {
        private readonly SwaggerOptions _options;

        public SwaggerGenOptionsSetup(IOptions<SwaggerOptions> options)
        {
            _options = options.Value;
        }

        public void Configure(SwaggerGenOptions swaggerGenOptions)
        {
            swaggerGenOptions.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = _options.Title,
                Version = _options.Version
            });

            swaggerGenOptions.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = _options.Description,
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer"
            });

            swaggerGenOptions.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    },
                    Scheme = "oauth2",
                    In = ParameterLocation.Header
                },
                new List<string>()
            }
        });
        }
    }
}