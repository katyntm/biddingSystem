using System.ComponentModel.DataAnnotations;

namespace CarAuction.Application.DTOs
{
    public class BuyNowRequestDto
    {
        [Required]
        public Guid AuctionVehicleId { get; set; }
    }
}