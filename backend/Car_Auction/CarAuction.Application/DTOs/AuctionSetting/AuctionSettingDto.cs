namespace CarAuction.Application.DTOs.AuctionSetting
{
    public class AuctionSettingDto
    {
        public AuctionSessionDto Session { get; set; }
        public List<TacticDto> Tactics { get; set; }

    }

    public class AuctionSessionDto
    {
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }

    public class TacticDto
    {
        public string Name { get; set; }
        public List<CriteriaDto> Criteria { get; set; }
        public List<StepDto> Steps { get; set; }
    }

    public class CriteriaDto
    {
        public string FieldName { get; set; }
        public string Operator { get; set; }
        public string Value { get; set; }
    } 
    public class StepDto
    {
        public int  StepNumber { get; set; }
        public SaleChannelDto SaleChannel { get; set; }
    }
    public class SaleChannelDto
    {
        public string Name { get; set; }
        public decimal PricePercentage { get; set; }
        public decimal BuyItNowPercentage { get; set; }
    }
   
}
