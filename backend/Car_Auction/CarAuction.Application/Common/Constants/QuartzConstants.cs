namespace CarAuction.Application.Common.Constants
{
    public static class QuartzConstants
    {
        public const string Group = "Auction";

        public static class Jobs
        {
            public const string AuctionStart = "AuctionStartJob";
            public const string AuctionEnd = "AuctionEndJob";
            public const string VehicleImport = "VehicleImportJob";
        }

        public static class Triggers
        {
            public const string AuctionStart = "AuctionStartJobTrigger";
            public const string AuctionEnd = "AuctionEndJobTrigger";
            public const string VehicleImport = "VehicleImportJobTrigger";
        }

        public static class Cron
        {
            public const string Every30Seconds = "0/30 * * * * ?";
        }
    }
}
