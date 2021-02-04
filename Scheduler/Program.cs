using DataAccessLayer.WorkProcess;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Models.ApplicationContext;
using Scheduler.SAPBAPIIntegration;
using System;
using Utility.HelperClasses;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.Configuration.FileExtensions;
using System.Threading;

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
