using Microsoft.Extensions.Configuration;

namespace CarAuction.Infrastructure.Options
{
    public class AuctionSettingOptions
    {
        [ConfigurationKeyName("auctionSession")]
        public AuctionSession AuctionSession { get; set; }

        [ConfigurationKeyName("saleChannel")]
        public List<SaleChannels> SaleChannels { get; set; }

        [ConfigurationKeyName("tactics")]
        public List<Tactics> Tactics { get; set; }
    }

    public class AuctionSession
    {
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }

    public class SaleChannels
    {
        public string Name { get; set; }
        public decimal PricePercentage { get; set; }
        public decimal BuyItNowPercentage { get; set; }
    }

    public class Tactics
    {
        public string Name { get; set; }
        [ConfigurationKeyName("criteria")]
        public List<Criterias> Criterias { get; set; }

        [ConfigurationKeyName("steps")]
        public List<Steps> Steps { get; set; }
    }

    public class Criterias
    {
        public string FieldName { get; set; }
        public string Operator { get; set; }
        public string Value { get; set; }
    }

    public class Steps
    {
        public int StepNumber { get; set; }
        public string SaleChannelName { get; set; }
    }
}