using CarAuction.Application.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Text;


public class JwtBearerOptionsSetup : IConfigureNamedOptions<JwtBearerOptions>
{
  private readonly JwtOption _jwtOption;

  public JwtBearerOptionsSetup(IOptions<JwtOption> jwtOption)
  {
    _jwtOption = jwtOption.Value;
  }

  public void Configure(string name, JwtBearerOptions options)
  {
    options.TokenValidationParameters = new TokenValidationParameters
    {
      ValidateIssuer = true,
      ValidateAudience = true,
      ValidateLifetime = true,
      ValidateIssuerSigningKey = true,
      ValidIssuer = _jwtOption.Issuer,
      ValidAudience = _jwtOption.Audience,
      IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOption.Key)),
      ClockSkew = TimeSpan.Zero
    };
  }

  public void Configure(JwtBearerOptions options)
  {
    Configure(string.Empty, options);
  }
}