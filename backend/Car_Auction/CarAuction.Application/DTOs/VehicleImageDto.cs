namespace CarAuction.Infrastructure.DTOs
{
    public class VehicleImageDto
    {
        public Guid Id { get; set; }   
        public Guid VehicleId { get; set; }
        public string Url { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}