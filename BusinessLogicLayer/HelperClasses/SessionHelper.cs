﻿using Microsoft.AspNetCore.Http;
using Models.Application;
using Models.UserRights;
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
    }
}
