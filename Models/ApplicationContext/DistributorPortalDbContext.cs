using Microsoft.EntityFrameworkCore;
using Models.Application;
using Models.ErrorLog;
using Models.UserRights;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Models.ApplicationContext
{
    public class DistributorPortalDbContext : DbContext
    {
        public DistributorPortalDbContext(DbContextOptions<DistributorPortalDbContext> options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            foreach (var ForeignKey in builder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                ForeignKey.DeleteBehavior = DeleteBehavior.Restrict;
            }
        }
        public DbSet<ExceptionLog> ExceptionLog { get; set; }
        public DbSet<ApplicationModule> ApplicationModule { get; set; }
        public DbSet<ApplicationAction> ApplicationAction { get; set; }
        public DbSet<ApplicationPage> ApplicationPage { get; set; }
        public DbSet<ApplicationPageAction> ApplicationPageAction { get; set; }
        public DbSet<Role> Role { get; set; }
        public DbSet<RolePermission> RolePermission { get; set; }
        public DbSet<City> City { get; set; }
        public DbSet<Distributor> Distributor { get; set; }
        public DbSet<OrderDetail> OrderDetail { get; set; }
        public DbSet<OrderMaster> OrderMaster { get; set; }
        public DbSet<PaymentMaster> PaymentMaster { get; set; }
        public DbSet<ProductDetail> ProductDetail { get; set; }
        public DbSet<ProductMaster> ProductMaster { get; set; }
        public DbSet<Region> Region { get; set; }
        public DbSet<SubRegion> SubRegion { get; set; }
        public DbSet<User> User { get; set; }
        public DbSet<Designation> Designation { get; set; }
        public DbSet<PaymentMode> PaymentMode { get; set; }
        public DbSet<Bank> Bank { get; set; }
        public DbSet<OrderValue> OrderValue { get; set; }
    }
}
