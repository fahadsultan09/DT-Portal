using DataAccessLayer.WorkProcess;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Models.ApplicationContext;
using System;
using Utility.HelperClasses;

namespace Scheduler
{
    public class Program
    {
        private static DistributorPortalDbContext _DistributorPortalDbContext;

        static void Main(string[] args)
        {
            try
            {
                IConfiguration configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json", true, true).Build();
                var ConnectionString = configuration.GetSection("ConnectionStrings:DistributorPortalDbContext");
                var InProcessOrderStatus = configuration.GetSection("AppSettings:GetInProcessOrderStatus");
                var FromEmail = configuration.GetSection("Settings:FromEmail");
                var Password = configuration.GetSection("Settings:Password");
                var Port = configuration.GetSection("Settings:Port");
                var ServerAddress = configuration.GetSection("Settings:ServerAddress");
                var config = new Configuration(null) 
                { 
                    GetInProcessOrderStatus = InProcessOrderStatus.Value, 
                    ConnectionString = ConnectionString.Value,
                    FromEmail = FromEmail.Value,
                    Password = Password.Value,
                    Port = Convert.ToInt32(Port.Value),
                    ServerAddress = ServerAddress.Value,
                };
                var services = new ServiceCollection();
                services.AddDbContext<DistributorPortalDbContext>(options => options.UseMySQL(config.ConnectionString));
                services.AddDbContextPool<DistributorPortalDbContext>(option => option.UseLazyLoadingProxies().UseMySQL(config.ConnectionString));
                var serviceProvider = services.BuildServiceProvider();
                _DistributorPortalDbContext = serviceProvider.GetService<DistributorPortalDbContext>();
                IUnitOfWork unitOfWork = new UnitOfWork(_DistributorPortalDbContext);

                ////Order and Order Return Produc Status
                //DistributorOrderStatus distributorOrderStatus = new DistributorOrderStatus(unitOfWork, config);
                //distributorOrderStatus.GetInProcessOrderProductStatus();
                //distributorOrderStatus.GetInProcessOrderReturnProductStatus();

                //Order and Order Return Produc Status
                KPIEmailScheduler KPIEmailScheduler = new KPIEmailScheduler(unitOfWork, config);
                KPIEmailScheduler.GetPendingComplaints();

            }
            catch (Exception ex)
            {
            }
        }
    }
}
