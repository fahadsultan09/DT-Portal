using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DistributorPortal.Controllers
{
    public class ApplicationActionController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
