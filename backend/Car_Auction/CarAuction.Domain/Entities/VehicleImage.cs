using System;

namespace CarAuction.Domain.Entities
{
    public class VehicleImage
    {
        public Guid Id { get; set; }
        public Guid VehicleId { get; set; }
        public string Url { get; set; }
        public DateTime CreatedAt { get; set; }

        // Navigation property
        public virtual Vehicle Vehicle { get; set; }
    }
}