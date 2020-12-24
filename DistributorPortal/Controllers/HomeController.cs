using BusinessLogicLayer.ErrorLog;
using DataAccessLayer.WorkProcess;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.FileProviders;
using System;
using System.IO;

namespace DistributorPortal.Controllers
{
    public class HomeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public HomeController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
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
        public IActionResult GetFile(string filepath)
        {
            try
            {
                string contenttype;
                string filename = Path.GetFileName(filepath);
                using (var provider = new PhysicalFileProvider(Path.GetDirectoryName(filepath)))
                {
                    var stream = provider.GetFileInfo(filename).CreateReadStream();
                    new FileExtensionContentTypeProvider().TryGetContentType(filename, out contenttype);
                    if (contenttype == "application/vnd.openxmlformats-officedocument.wordprocessingml.document" || contenttype == "image/jpeg" || contenttype == "image/png")
                    {
                        return File(stream, contenttype, filename.Split('_')[1]);
                    }
                    return File(stream, contenttype);
                }
            }
            catch (Exception ex)
            {
                new ErrorLogBLL(_unitOfWork).AddExceptionLog(ex);
                return Json(false);
            }

        }
    }
}
