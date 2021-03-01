﻿using Microsoft.Extensions.Configuration;

namespace Utility.HelperClasses
{
    public class Configuration
    {
        public string ResetPassword { get; set; }
        public string SAPConnection { get; set; }
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

        public Configuration(IConfiguration configuration)
        {
            if (configuration != null)
            {
                ResetPassword = configuration["AppSettings:ResetPassword"];
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
                int portnumber;
                BaseFilePath = configuration["Settings:FolderPath"];
                FromEmail = configuration["Settings:FromEmail"];
                Password = configuration["Settings:Password"];
                ServerAddress = configuration["Settings:ServerAddress"];
                int.TryParse(configuration["Settings:Port"], out portnumber);
                Port = portnumber;
            }
        }
    }
}
