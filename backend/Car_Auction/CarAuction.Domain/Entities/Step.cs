namespace CarAuction.Domain.Entities
{
    public class Step
    {
        public Guid Id { get; set; }
        public int StepNumber { get; set; }
        //FK
        public Guid TacticId { get; set; }
        public Guid SaleChannelId { get; set; }
        public Tactic Tactic { get; set; }
        public SaleChannel SaleChannel { get; set; }
    }
}
