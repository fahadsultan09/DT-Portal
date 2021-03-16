using DataAccessLayer.WorkProcess;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Models.ApplicationContext;
using Scheduler.SAPBAPIIntegration;
using System.Threading;
using Utility.HelperClasses;

namespace Scheduler
{
    public class Program
    {
        private static DistributorPortalDbContext _appDbContext;

        static void Main(string[] args)
        {
            Thread.Sleep(5000);
            IConfiguration configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json", true, true).Build();
            var InProcessOrderStatus = configuration.GetSection("AppSettings:GetInProcessOrderStatus");
            var ConnectionString = configuration.GetSection("ConnectionStrings:DistributorPortalDbContext");
            var config = new Configuration(null) { GetInProcessOrderStatus = InProcessOrderStatus.Value, ConnectionString = ConnectionString.Value };
            var services = new ServiceCollection();
            services.AddDbContext<DistributorPortalDbContext>(options => options.UseMySQL(config.ConnectionString));
            var serviceProvider = services.BuildServiceProvider();
            _appDbContext = serviceProvider.GetService<DistributorPortalDbContext>();
            IUnitOfWork unitOfWork = new UnitOfWork(_appDbContext);
            OrderBAPI orderBAPI = new OrderBAPI(unitOfWork, config);
            orderBAPI.GetInProcessOrderStatus();
            orderBAPI.GetInProcessOrderReturnStatus();
        }
    }
}
