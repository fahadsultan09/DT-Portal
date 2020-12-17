using BusinessLogicLayer.HelperClasses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace DistributorPortal.Controllers
{
    public class BaseController : Controller
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {            
            var context = filterContext.HttpContext;

            //if (Request.IsAjaxRequest())
            //{
            //    var b = true;
            //    //filterContext.HttpContext.Response.StatusCode = 403;
            //    //filterContext.Result = new RedirectResult("~/Login/Index");
            //    filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Login", action = "Index" }));
            //}


            if (context.Session != null)
            {
                if (SessionHelper.LoginUser != null)
                {
                    string name = (string)filterContext.RouteData.Values["Controller"];
                    string Action = (string)filterContext.RouteData.Values["Action"];                    
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
