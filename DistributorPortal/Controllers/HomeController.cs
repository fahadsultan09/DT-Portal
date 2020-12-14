using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace DistributorPortal.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return RedirectToAction("AdminDashboard");
        }
        public IActionResult AdminDashboard()
        {
            return View();
        }
        public IActionResult AccountDashboard()
        {
            return View();
        }
        public IActionResult DistributorDashboard()
        {
            return View();
        }
    }
}
