using Microsoft.Extensions.Configuration;

namespace Utility.HelperClasses
{
    public class Configuration
    {
        public string ResetPassword { get; set; }
        public string SAPConnection { get; set; }
        public string SyncDistributorURL { get; set; }
        public string SyncProductURL { get; set; }
        public string SyncDistributorBalanceURL { get; set; }

        public Configuration(IConfiguration configuration)
        {
            ResetPassword = configuration["AppSettings:ResetPassword"];
            SyncDistributorURL = configuration["AppSettings:SyncDistributorURL"];
            SyncProductURL = configuration["AppSettings:SyncProductURL"];
            SyncDistributorBalanceURL = configuration["AppSettings:SyncDistributorBalanceURL"];
        }
    }
}
