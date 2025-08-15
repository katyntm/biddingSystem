namespace CarAuction.Domain.Entities
{
    public class Criteria
    {
        public Guid Id { get; set; }
        public string FieldName { get; set; }
        public string Operator { get; set; }
        public string Value { get; set; }
        //FK
        public Guid TacticId { get; set; }
        public Tactic Tactic { get; set; }
    }
}
