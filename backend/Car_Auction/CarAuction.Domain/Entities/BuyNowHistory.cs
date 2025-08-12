namespace CarAuction.Domain.Entities
{
    public class BuyNowHistory
    {
        public Guid Id { get; set; }
        public decimal BuyAmount { get; set; }
        public DateTime BuyTime { get; set; }
        //FK
        public Guid AuctionVehicleId { get; set; }
        public string BuyerUserId { get; set; }
        public AuctionVehicle AuctionVehicle { get; set; }
        public ApplicationUser BuyerUser { get; set; }
    }
}
