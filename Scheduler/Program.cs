using BusinessLogicLayer.ErrorLog;
using DataAccessLayer.WorkProcess;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Models.ApplicationContext;
using System;
using Utility;
using Utility.HelperClasses;
using static Utility.Constant.Common;

namespace Scheduler
{
    public class Program
    {
        private static DistributorPortalDbContext _DistributorPortalDbContext;
        private static IUnitOfWork _unitOfWork;
        private static IWebHostEnvironment _env;
        private static string fileName = Guid.NewGuid().ToString() + ".txt";
        static void Main(string[] args)
        {
            try
            {
                IConfiguration configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json", true, true).Build();
                var ConnectionString = configuration.GetSection("ConnectionStrings:DistributorPortalDbContext");
                var FromEmail = configuration.GetSection("Settings:FromEmail");
                var Password = configuration.GetSection("Settings:Password");
                var Port = configuration.GetSection("Settings:Port");
                var ServerAddress = configuration.GetSection("Settings:ServerAddress");
                var POUserName = configuration.GetSection("Settings:POUserName");
                var POPassword = configuration.GetSection("Settings:POPassword");
                var URL = configuration.GetSection("Settings:URL");
                var config = new Configuration(null)
                {
                    ConnectionString = ConnectionString.Value,
                    FromEmail = FromEmail.Value,
                    Password = Password.Value,
                    Port = Convert.ToInt32(Port.Value),
                    ServerAddress = ServerAddress.Value,
                    POUserName = POUserName.Value,
                    POPassword = POPassword.Value,
                    URL = URL.Value,
                };
                var services = new ServiceCollection();
                services.AddDbContext<DistributorPortalDbContext>(options => options.UseMySQL(config.ConnectionString));
                services.AddDbContextPool<DistributorPortalDbContext>(option => option.UseLazyLoadingProxies().UseMySQL(config.ConnectionString));
                var serviceProvider = services.BuildServiceProvider();
                _DistributorPortalDbContext = serviceProvider.GetService<DistributorPortalDbContext>();
                _unitOfWork = new UnitOfWork(_DistributorPortalDbContext);

                ////Order and Order Return Product Status
                //ExtensionUtility.WriteToFile("Console Start-" + DateTime.Now, FolderName.OrderStatus, fileName);
                //DistributorOrderStatus distributorOrderStatus = new DistributorOrderStatus(_unitOfWork, config);
                //distributorOrderStatus.GetInProcessOrderProductStatus();
                //distributorOrderStatus.GetInProcessOrderReturnProductStatus();
                //ExtensionUtility.WriteToFile("Console End-" + DateTime.Now, FolderName.OrderStatus, fileName);

                ////Complaint Status
                //ExtensionUtility.WriteToFile("Console Start-" + DateTime.Now, FolderName.Complaint, fileName);
                //KPIEmailScheduler KPIEmailScheduler = new KPIEmailScheduler(_unitOfWork, config, _env);
                //KPIEmailScheduler.GetPendingComplaints();
                //ExtensionUtility.WriteToFile("Console End-" + DateTime.Now, FolderName.Complaint, fileName);

                ////Get pending value
                //ExtensionUtility.WriteToFile("Console Start-" + DateTime.Now, FolderName.PendingValue, fileName);
                //DistributorPendingQuanityValue distributorPendingQuanityValue = new DistributorPendingQuanityValue(_unitOfWork, config);
                //distributorPendingQuanityValue.AddDistributorPendingValue();
                //ExtensionUtility.WriteToFile("Console End-" + DateTime.Now, FolderName.PendingValue, fileName);

                //Get pending quantity
                ExtensionUtility.WriteToFile("Console Start-" + DateTime.Now, FolderName.PendingQuantity, fileName);
                DistributorPendingQuanityValue distributorPendingQuanityValue = new DistributorPendingQuanityValue(_unitOfWork, config);
                distributorPendingQuanityValue.AddDistributorPendingQuantity();
                ExtensionUtility.WriteToFile("Console End-" + DateTime.Now, FolderName.PendingQuantity, fileName);
            }
            catch (Exception ex)
            {
                ExtensionUtility.WriteToFile("Error occured-" + DateTime.Now, FolderName.PendingQuantity, fileName);
                new ErrorLogBLL(_unitOfWork).AddSchedulerExceptionLog(ex);
            }
        }
    }
}
