using System.ComponentModel.DataAnnotations;

namespace CarAuction.Domain.Entities
{
    public class AuctionVehicle
    {
        public Guid Id { get; set; }
        public decimal CurrentPrice { get; set; }
        public decimal? BuyItNowPrice { get; set; }
        public bool IsSold { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        //FK
        public Guid VehicleId { get; set; }
        public Guid TacticId { get; set; }
        public Guid StepId { get; set; }
        public string WinnerUserId { get; set; }

        //public Vehicle Vehicle { get; set; }
        public Tactic Tactic { get; set; }
        public Step Step { get; set; }
        public ApplicationUser WinnerUser { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }
    }
}
