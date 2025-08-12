namespace CarAuction.Domain.Entities
{
    public class SaleChannel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public decimal PricePercentage { get; set; }
        public decimal? BuyItNowPercentage { get; set; }
    }
}
