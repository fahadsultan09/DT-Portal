using BusinessLogicLayer.ErrorLog;
using DataAccessLayer.WorkProcess;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Models.ApplicationContext;
using System;
using System.IO;
using System.Reflection;
using Utility.HelperClasses;

namespace Scheduler
{
    public class Program
    {
        private static DistributorPortalDbContext _DistributorPortalDbContext;
        private static IUnitOfWork _unitOfWork;
        private static IWebHostEnvironment _env;
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
                var config = new Configuration(null) 
                { 
                    ConnectionString = ConnectionString.Value,
                    FromEmail = FromEmail.Value,
                    Password = Password.Value,
                    Port = Convert.ToInt32(Port.Value),
                    ServerAddress = ServerAddress.Value,
                    POUserName = POUserName.Value,
                    POPassword = POPassword.Value,
                };
                var services = new ServiceCollection();
                services.AddDbContext<DistributorPortalDbContext>(options => options.UseMySQL(config.ConnectionString));
                services.AddDbContextPool<DistributorPortalDbContext>(option => option.UseLazyLoadingProxies().UseMySQL(config.ConnectionString));
                var serviceProvider = services.BuildServiceProvider();
                _DistributorPortalDbContext = serviceProvider.GetService<DistributorPortalDbContext>();
                _unitOfWork = new UnitOfWork(_DistributorPortalDbContext);

                ////Order and Order Return Produc Status
                //DistributorOrderStatus distributorOrderStatus = new DistributorOrderStatus(_unitOfWork, config);
                //distributorOrderStatus.GetInProcessOrderProductStatus();
                //distributorOrderStatus.GetInProcessOrderReturnProductStatus();

                //Order and Order Return Produc Status
                KPIEmailScheduler KPIEmailScheduler = new KPIEmailScheduler(_unitOfWork, config, _env);
                KPIEmailScheduler.GetPendingComplaints();

            }
            catch (Exception ex)
            {
                new ErrorLogBLL(_unitOfWork).AddSchedulerExceptionLog(ex);
            }
        }
    }
}
