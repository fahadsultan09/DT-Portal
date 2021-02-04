using BusinessLogicLayer.HelperClasses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Linq;
using Utility;

namespace DistributorPortal.Controllers
{
    public class BaseController : Controller
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            // our code before action executes
            var context = filterContext.HttpContext;
            if (context.Session != null)
            {                
                if (SessionHelper.LoginUser != null)
                {
                    if (!HttpContext.Request.IsAjaxRequest())
                    {
                        string Controller = (string)filterContext.RouteData.Values["Controller"];
                        string Action = (string)filterContext.RouteData.Values["Action"];
                        string URL = "/" + Controller + "/" + Action;
                        if (!SessionHelper.NavigationMenu.Select(e => e.ApplicationPage.PageURL).Contains(URL))
                        {
                            filterContext.HttpContext.Response.StatusCode = 403;
                            filterContext.Result = new RedirectResult("~/Login/Index");
                        }
                    }
                }
                else
                {
                    filterContext.HttpContext.Response.StatusCode = 403;
                    filterContext.Result = new RedirectResult("~/Login/Index");
                }
            }
            else
            {
                filterContext.HttpContext.Response.StatusCode = 403;
                filterContext.Result = new RedirectResult("~/Login/Index");
            }
            base.OnActionExecuting(filterContext);
        }
    }
}
