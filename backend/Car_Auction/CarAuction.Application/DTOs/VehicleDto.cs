namespace CarAuction.Application.DTOs
{
    public class VehicleDto
    {
        public Guid Id { get; set; }
        public string VIN { get; set; }
        public string Make { get; set; }
        public int ModelYear { get; set; }
        public string FuelType { get; set; }
        public string ModelType { get; set; }
        public string Transmission { get; set; }
        public string BodyStyle { get; set; }
        public string Color { get; set; }
        public decimal Grade { get; set; }
        public decimal Price { get; set; }
        public string Location { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}