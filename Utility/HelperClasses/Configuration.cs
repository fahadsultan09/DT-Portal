using Microsoft.Extensions.Configuration;

namespace Utility.HelperClasses
{
    public class Configuration
    {
        public string URL { get; set; }
        public string FileSize { get; set; }
        public string ResetPassword { get; set; }
        public string SAPConnection { get; set; }
        public string PostTaxChallan { get; set; }
        public string SyncDistributorURL { get; set; }
        public string SyncProductURL { get; set; }
        public string SyncDistributorBalanceURL { get; set; }
        public string PostOrder { get; set; }
        public string PostPayment { get; set; }
        public string GetPendingQuantity { get; set; }
        public string GetInProcessOrderStatus { get; set; }
        public string ConnectionString { get; set; }
        public string PostReturnOrder { get; set; }
        public int DistributorFiler { get; set; }
        public int DistributorNonFiler { get; set; }
        public string GetPendingOrderValue { get; set; }
        public string BaseFilePath { get; set; }
        public string FromEmail { get; set; }
        public string Password { get; set; }
        public int Port { get; set; }
        public string ServerAddress { get; set; }
        public string DistributorWiseProduct { get; set; }
        public string ProductWisePriceDiscount { get; set; }
        public string SalesOrder { get; set; }
        public string OrderReturn { get; set; }
        public string Invoice { get; set; }
        public string SaleReturnCreditNote { get; set; }
        public string CustomerBalance { get; set; }
        public string CustomerLedger { get; set; }
        public string POUserName { get; set; }
        public string POPassword { get; set; }
        public Configuration(IConfiguration configuration)
        {
            if (configuration != null)
            {
                URL = configuration["Settings:URL"];
                FileSize = configuration["Settings:FileSize"];
                ResetPassword = configuration["AppSettings:ResetPassword"];
                PostTaxChallan = configuration["AppSettings:PostTaxChallan"];
                SyncDistributorURL = configuration["AppSettings:SyncDistributorURL"];
                SyncProductURL = configuration["AppSettings:SyncProductURL"];
                SyncDistributorBalanceURL = configuration["AppSettings:SyncDistributorBalanceURL"];
                PostOrder = configuration["AppSettings:PostOrder"];
                PostPayment = configuration["AppSettings:PostPayment"];
                GetPendingQuantity = configuration["AppSettings:GetPendingQuantity"];
                GetInProcessOrderStatus = configuration["AppSettings:GetInProcessOrderStatus"];
                PostReturnOrder = configuration["AppSettings:PostReturnOrder"];
                DistributorFiler = configuration["Distributor:Filer"].ParseToInt32();
                DistributorNonFiler = configuration["Distributor:NonFiler"].ParseToInt32();
                GetPendingOrderValue = configuration["AppSettings:GetPendingOrderValue"];
                BaseFilePath = configuration["Settings:FolderPath"];
                FromEmail = configuration["Settings:FromEmail"];
                Password = configuration["Settings:Password"];
                ServerAddress = configuration["Settings:ServerAddress"];
                DistributorWiseProduct = configuration["AppSettings:DistributorWiseProduct"];
                ProductWisePriceDiscount = configuration["AppSettings:ProductWisePriceDiscount"];
                SalesOrder = configuration["AppSettings:SalesOrder"];
                OrderReturn = configuration["AppSettings:OrderReturn"];
                Invoice = configuration["AppSettings:Invoice"];
                SaleReturnCreditNote = configuration["AppSettings:SaleReturnCreditNote"];
                CustomerBalance = configuration["AppSettings:CustomerBalance"];
                CustomerLedger = configuration["AppSettings:CustomerLedger"];
                POUserName = configuration["Settings:POUserName"];
                POPassword = configuration["Settings:POPassword"];
                int.TryParse(configuration["Settings:Port"], out int portnumber);
                Port = portnumber;
            }
        }
    }
}
