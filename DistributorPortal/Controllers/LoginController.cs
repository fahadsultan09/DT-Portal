using BusinessLogicLayer.HelperClasses;
using BusinessLogicLayer.Login;
using DataAccessLayer.WorkProcess;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.Application;
using Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utility;
using Utility.HelperClasses;

namespace DistributorPortal.Controllers
{
    public class LoginController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private SessionHelper sessionHelper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly LoginBLL login;

        public LoginController(IUnitOfWork unitOfWork, IHttpContextAccessor HhttpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _httpContextAccessor = HhttpContextAccessor;
            login = new LoginBLL(_unitOfWork);
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult Index(User model)
        {
            JsonResponse jsonResponse = new JsonResponse();
            sessionHelper = new SessionHelper(_httpContextAccessor);
            if (login.CheckLogin(model) == LoginStatus.Success)
            {
                jsonResponse.Status = true;
                jsonResponse.Message = "Login Successfully";
                jsonResponse.RedirectURL = Url.Action("Index", "Home");
                return Json(new { data = jsonResponse });
            }
            else
            {
                jsonResponse.Status = false;
                jsonResponse.Message = "Invalid username or password. Please try again.";
                jsonResponse.RedirectURL = string.Empty;
                return Json(new { data = jsonResponse });
            }
        }
    }
}
