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
        private static string fileName = Guid.NewGuid().ToString() + ".txt";
        static void Main()
        {
            try
            {
                var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
                ExtensionUtility.WriteToFile(environmentName, FolderName.OrderStatus, fileName);
                IConfiguration configuration = new ConfigurationBuilder().AddJsonFile($"appsettings.json", true, true).AddJsonFile($"appsettings.{environmentName}.json", true, true).Build();
                var ConnectionString = configuration.GetSection("ConnectionStrings:DistributorPortalDbContext");
                var FromEmail = configuration.GetSection("Settings:FromEmail");
                var Password = configuration.GetSection("Settings:Password");
                var Port = configuration.GetSection("Settings:Port");
                var ServerAddress = configuration.GetSection("Settings:ServerAddress");
                var POUserName = configuration.GetSection("Settings:POUserName");
                var POPassword = configuration.GetSection("Settings:POPassword");
                var URL = configuration.GetSection("Settings:URL");
                var GetInProcessOrderStatus = configuration.GetSection("AppSettings:GetInProcessOrderStatus");
                var GetPendingQuantity = configuration.GetSection("AppSettings:GetPendingQuantity");
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
                    GetInProcessOrderStatus = GetInProcessOrderStatus.Value,
                    GetPendingQuantity = GetPendingQuantity.Value,
                };
                var services = new ServiceCollection();
                services.AddDbContext<DistributorPortalDbContext>(options => options.UseMySQL(config.ConnectionString));
                services.AddDbContextPool<DistributorPortalDbContext>(option => option.UseLazyLoadingProxies().UseMySQL(config.ConnectionString));
                var serviceProvider = services.BuildServiceProvider();
                _DistributorPortalDbContext = serviceProvider.GetService<DistributorPortalDbContext>();
                _unitOfWork = new UnitOfWork(_DistributorPortalDbContext);
                ExtensionUtility.WriteToFile("Console Start - " + DateTime.Now, FolderName.OrderStatus, fileName);

                //Order and Order Return Product Status
                DistributorOrderStatus distributorOrderStatus = new DistributorOrderStatus(_unitOfWork, config);
                ExtensionUtility.WriteToFile("GetInProcessOrderProductStatus Start - " + DateTime.Now, FolderName.OrderStatus, fileName);
                distributorOrderStatus.GetInProcessOrderProductStatus(config.GetInProcessOrderStatus);
                ExtensionUtility.WriteToFile("GetInProcessOrderProductStatus End - " + DateTime.Now, FolderName.OrderStatus, fileName);
                ExtensionUtility.WriteToFile("GetInProcessOrderReturnProductStatus Start - " + DateTime.Now, FolderName.OrderStatus, fileName);
                distributorOrderStatus.GetInProcessOrderReturnProductStatus(config.GetInProcessOrderStatus);
                ExtensionUtility.WriteToFile("GetInProcessOrderReturnProductStatus End - " + DateTime.Now, FolderName.OrderStatus, fileName);

                ////Complaint Status
                //KPIEmailScheduler KPIEmailScheduler = new KPIEmailScheduler(_unitOfWork, config, _env);
                //KPIEmailScheduler.GetPendingComplaints();

                ////Get pending value
                //DistributorPendingQuanityValue distributorPendingQuanityValue = new DistributorPendingQuanityValue(_unitOfWork, config);
                //distributorPendingQuanityValue.AddDistributorPendingValue();

                ////Get pending quantity
                //DistributorPendingQuanityValue distributorPendingQuanityValue = new DistributorPendingQuanityValue(_unitOfWork, config);
                //distributorPendingQuanityValue.AddDistributorPendingQuantity(fileName);

                ExtensionUtility.WriteToFile("Console End - " + DateTime.Now, FolderName.OrderStatus, fileName);
            }
            catch (Exception ex)
            {
                ExtensionUtility.WriteToFile("Error occured - " + DateTime.Now, FolderName.OrderStatus, fileName);
                new ErrorLogBLL(_unitOfWork).AddSchedulerExceptionLog(ex);
            }
        }
    }
}
