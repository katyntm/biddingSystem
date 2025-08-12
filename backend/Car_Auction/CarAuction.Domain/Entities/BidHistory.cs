namespace CarAuction.Domain.Entities
{
    public class BidHistory
    {
        public Guid Id { get; set; }
        public decimal BidAmount { get; set; }
        public DateTime BidTime { get; set; }
        //FK
        public Guid AuctionVehicleId { get; set; }
        public string BidderUserId { get; set; }
        public AuctionVehicle AuctionVehicle { get; set; }
        public ApplicationUser BidderUser { get; set; }
    }
}
