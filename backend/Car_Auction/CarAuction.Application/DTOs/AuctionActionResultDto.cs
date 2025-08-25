namespace CarAuction.Application.DTOs
{
    public class AuctionActionResultDto
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public Guid AuctionVehicleId { get; set; }
        public decimal CurrentPrice { get; set; }
        public bool IsSold { get; set; }
        public string WinnerUserId { get; set; }
        public bool CanBid { get; set; }
        public bool CanBuyNow { get; set; }
    }
}