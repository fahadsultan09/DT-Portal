using DataAccessLayer.WorkProcess;
using DistributorPortal.SignalRNotification;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Models.ApplicationContext;
using Rotativa.AspNetCore;
using SapNwRfc.Pooling;
using System;
using System.Linq;
using Utility.HelperClasses;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

namespace DistributorPortal
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHttpContextAccessor();
            services.AddDistributedMemoryCache();
            services.AddSession(option =>
            {
                option.IdleTimeout = TimeSpan.FromHours(1);
                option.Cookie.HttpOnly = true;
                option.Cookie.IsEssential = true;
            });

            services.ConfigureApplicationCookie(options =>
            {
                // Cookie settings
                options.Cookie.HttpOnly = true;
                options.ExpireTimeSpan = TimeSpan.FromHours(1);
                options.LoginPath = "/Login/Index";
                options.AccessDeniedPath = "/Login/Index";
                options.Cookie.Name = "DistributorPortalSessionExpire";
            });

            services.AddDbContextPool<DistributorPortalDbContext>(option => option.UseLazyLoadingProxies().UseMySQL(Configuration.GetConnectionString("DistributorPortalDbContext")));
            services.AddMvc(option => option.EnableEndpointRouting = false).AddJsonOptions(opt => opt.JsonSerializerOptions.PropertyNamingPolicy = null);

            services.AddTransient<IUnitOfWork, UnitOfWork>();
            services.AddSingleton<Configuration, Configuration>();
            services.AddControllersWithViews(options =>
            {
                options.InputFormatters.Insert(0, GetJsonPatchInputFormatter());
            });
            services.AddControllersWithViews().AddRazorRuntimeCompilation();

            services.AddSingleton<ISapConnectionPool>(_ => new SapConnectionPool(Configuration.GetValue<string>("SAPSettings:SAPConnection")));
            services.AddScoped<ISapPooledConnection, SapPooledConnection>();
            services.AddSignalR(option =>
            {
                option.KeepAliveInterval = TimeSpan.FromMinutes(1);
                option.EnableDetailedErrors = true;
            });
        }

        private static NewtonsoftJsonPatchInputFormatter GetJsonPatchInputFormatter()
        {
            var builder = new ServiceCollection().AddLogging().AddMvc().AddNewtonsoftJson().Services.BuildServiceProvider();
            return builder.GetRequiredService<IOptions<MvcOptions>>().Value.InputFormatters.OfType<NewtonsoftJsonPatchInputFormatter>().First();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        [Obsolete]
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHostingEnvironment env2)
        {
            app.UseStatusCodePagesWithReExecute("/Home/HandleError/{0}");
            if (env.IsDevelopment())
            {
                //app.UseDeveloperExceptionPage();
                app.UseExceptionHandler("/Home/Error");
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseStatusCodePages();
            app.Use(async (context, next) =>
            {
                await next();
                if (context.Response.StatusCode == 404)
                {
                    context.Request.Path = "/Home/Error";
                    await next();
                }
                else if (context.Response.StatusCode == 500)
                {
                    context.Request.Path = "/Home/InternalServerError";
                    await next();
                }
                else if (context.Response.StatusCode == 403)
                {
                    context.Request.Path = "/Home/AccessDenied";
                    await next();
                }
                else if (context.Response.StatusCode == 302 || context.Response.StatusCode == 304)
                {
                    context.Request.Path = "/Login/Index";
                    await next();
                }
            });
            app.UseStaticFiles();
            app.UseSession();
            app.UseAuthorization();
            app.UseSignalR(endpoints =>
            {
                endpoints.MapHub<ChatHub>("/chatHub");
            });
            app.UseMvc(route =>
            {
                route.MapRoute("default", "{Controller=Login}/{Action=Index}/{Id?}");

            });
            RotativaConfiguration.Setup(env2, "Rotativa");
        }
    }
}
