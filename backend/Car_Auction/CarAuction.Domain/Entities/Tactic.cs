namespace CarAuction.Domain.Entities
{
    public class Tactic
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public DateTime CreatedAt { get; set; }
        public ICollection<Criteria> Criterias { get; set; }
        public ICollection<Step> Steps { get; set; }
    }
}
