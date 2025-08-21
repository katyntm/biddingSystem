namespace CarAuction.Application.Options;

public class JwtOption
{
  public const string ConfigurationSection = "JWT";
  public string Key { get; set; } 
  public string Issuer { get; set; } 
  public string Audience { get; set; } 
  public int DurationInMinutes { get; set; } 
}