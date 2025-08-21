using System;
using System.Collections.Generic;

namespace CarAuction.Domain.Entities
{
    public class Vehicle
    {
        public Guid Id { get; set; }
        public string VIN { get; set; }
        public string Make { get; set; }
        public int ModelYear { get; set; }
        public string ModelType { get; set; }
        public decimal Price { get; set; }
        public string FuelType { get; set; }
        public string BodyStyle { get; set; }
        public string Color { get; set; }
        public string Transmission { get; set; }
        public string Location { get; set; }
        public decimal Grade { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Navigation properties
        public virtual ICollection<VehicleImage> VehicleImages { get; set; } 
        public virtual ICollection<AuctionVehicle> AuctionVehicles { get; set; }
    }
}