namespace CarAuction.Infrastructure.Options
{

    public class SwaggerOptions
    {
        public const string ConfigurationSection = "Swagger";
        public string Title { get; set; } = "Car Auction API";
        public string Version { get; set; } = "v1";
        public string Description { get; set; } = "JWT Authorization header using the Bearer scheme. \r\n\r\n " +
                                                  "Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\n" +
                                                  "Example: \"Bearer eyJhbGciOiJIUzI1NiIsIn...\"";
    }

}
