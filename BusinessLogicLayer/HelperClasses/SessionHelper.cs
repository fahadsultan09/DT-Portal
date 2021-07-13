using Microsoft.AspNetCore.Http;
using Models.Application;
using Models.UserRights;
using Models.ViewModel;
using PDU.BusinessLogicLayer.HelperClasses;
using System.Collections.Generic;

namespace BusinessLogicLayer.HelperClasses
{
    public class SessionHelper
    {
        private static IHttpContextAccessor _httpContextAccessor;
        public SessionHelper(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        public static User LoginUser
        {
            get => _httpContextAccessor.HttpContext.Session.Get<User>("LoginUser");
            set => _httpContextAccessor.HttpContext.Session.Set("LoginUser", value);
        }
        public static List<RolePermission> NavigationMenu
        {
            get => _httpContextAccessor.HttpContext.Session.Get<List<RolePermission>>("NavigationMenu");
            set => _httpContextAccessor.HttpContext.Session.Set("NavigationMenu", value);
        }
        public static List<ProductDetail> AddProduct
        {
            get => _httpContextAccessor.HttpContext.Session.Get<List<ProductDetail>>("AddProduct");
            set => _httpContextAccessor.HttpContext.Session.Set("AddProduct", value);
        }
        public static List<DistributorWiseProductDiscountAndPrices> AddDistributorWiseProduct
        {
            get => _httpContextAccessor.HttpContext.Session.Get<List<DistributorWiseProductDiscountAndPrices>>("AddDistributorWiseProduct");
            set => _httpContextAccessor.HttpContext.Session.Set("AddDistributorWiseProduct", value);
        }
        public static List<OrderReturnDetail> AddReturnProduct
        {
            get => _httpContextAccessor.HttpContext.Session.Get<List<OrderReturnDetail>>("AddReturnProduct");
            set => _httpContextAccessor.HttpContext.Session.Set("AddReturnProduct", value);
        }
        public static DistributorBalance DistributorBalance
        {
            get => _httpContextAccessor.HttpContext.Session.Get<DistributorBalance>("DistributorBalance");
            set => _httpContextAccessor.HttpContext.Session.Set("DistributorBalance", value);
        }
        public static List<DistributorPendingQuantity> DistributorPendingQuantity
        {
            get => _httpContextAccessor.HttpContext.Session.Get<List<DistributorPendingQuantity>>("DistributorPendingQuantity");
            set => _httpContextAccessor.HttpContext.Session.Set("DistributorPendingQuantity", value);
        }
        public static List<DistributorPendingValue> DistributorPendingValue
        {
            get => _httpContextAccessor.HttpContext.Session.Get<List<DistributorPendingValue>>("DistributorPendingValue");
            set => _httpContextAccessor.HttpContext.Session.Set("DistributorPendingValue", value);
        }
        public static string URL
        {
            get => _httpContextAccessor.HttpContext.Session.Get<string>("URL");
            set => _httpContextAccessor.HttpContext.Session.Set("URL", value);
        }
        public static string Disclaimer
        {
            get => _httpContextAccessor.HttpContext.Session.Get<string>("Disclaimer");
            set => _httpContextAccessor.HttpContext.Session.Set("Disclaimer", value);
        }
        public static List<Notification> Notification
        {
            get => _httpContextAccessor.HttpContext.Session.Get<List<Notification>>("Notification");
            set => _httpContextAccessor.HttpContext.Session.Set("Notification", value);
        }
        public static int NotificationCount
        {
            get => _httpContextAccessor.HttpContext.Session.Get<int>("NotificationCount");
            set => _httpContextAccessor.HttpContext.Session.Set("NotificationCount", value);
        }
        public static string TotalOrderValue
        {
            get => _httpContextAccessor.HttpContext.Session.Get<string>("TotalOrderValue");
            set => _httpContextAccessor.HttpContext.Session.Set("TotalOrderValue", value);
        }
    }
}
