using System.ComponentModel.DataAnnotations;

namespace CarAuction.Application.DTOs
{
    public class BidRequestDto
    {
        [Required]
        public Guid AuctionVehicleId { get; set; }
        
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Bid amount must be greater than zero")]
        public decimal BidAmount { get; set; }
    }
}