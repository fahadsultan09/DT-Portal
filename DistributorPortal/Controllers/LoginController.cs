using BusinessLogicLayer.Application;
using BusinessLogicLayer.ErrorLog;
using BusinessLogicLayer.HelperClasses;
using BusinessLogicLayer.Login;
using DataAccessLayer.WorkProcess;
using DistributorPortal.Resource;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Models.Application;
using Models.ViewModel;
using System;
using System.Linq;
using Utility;
using Utility.HelperClasses;

namespace DistributorPortal.Controllers
{
    public class LoginController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly SessionHelper sessionHelper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly LoginBLL login;
        private readonly UserBLL _UserBLL;
        private readonly IConfiguration _IConfiguration;
        private readonly Configuration _configuration;
        private readonly OrderBLL orderBLL;

        public LoginController(IUnitOfWork unitOfWork, IHttpContextAccessor HhttpContextAccessor, IConfiguration IConfiguration, Configuration configuration)
        {
            _unitOfWork = unitOfWork;
            _httpContextAccessor = HhttpContextAccessor;
            sessionHelper = new SessionHelper(_httpContextAccessor);
            login = new LoginBLL(_unitOfWork);
            _UserBLL = new UserBLL(_unitOfWork);
            _IConfiguration = IConfiguration;
            _configuration = configuration;
            orderBLL = new OrderBLL(_unitOfWork);
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult Index(User model)
        {
            JsonResponse jsonResponse = new JsonResponse();
            string password = _IConfiguration.GetSection("Settings").GetSection("ResetPassword").Value;
            if (model.Password != null && model.Password.Equals(password) && login.CheckUserPassword(model, password))
            {
                jsonResponse.Status = false;
                jsonResponse.Message = "Please change default password.";
                jsonResponse.RedirectURL = string.Empty;
                return Json(new { data = jsonResponse });
            }
            if (model.Password != null && login.CheckLogin(model) == LoginStatus.Success)
            {
                SessionHelper.DistributorBalance = SessionHelper.LoginUser.IsDistributor ? orderBLL.GetBalance(SessionHelper.LoginUser.Distributor.DistributorSAPCode, _configuration) : null;
                jsonResponse.Status = true;
                jsonResponse.Message = "Login Successfully";
                jsonResponse.RedirectURL = Url.Action("Index", "Home");
                new AuditLogBLL(_unitOfWork).AddAuditLog("Login", "Index", "End Click on Login Button of ");
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

        [HttpPost]
        public IActionResult ChangePassword(User model)
        {
            JsonResponse jsonResponse = new JsonResponse();
            string password = _IConfiguration.GetSection("Settings").GetSection("ResetPassword").Value;
            try
            {
                if (model.Password == password)
                {
                    jsonResponse.Status = false;
                    jsonResponse.Message = "Default password could not be saved.";
                    return Json(new { data = jsonResponse });
                }
                string newPassword = EncryptDecrypt.Encrypt(model.Password);
                _UserBLL.ResetPassword(_UserBLL.GetAllUser().FirstOrDefault(x => x.UserName == model.UserName).Id, newPassword);

                if (login.CheckLogin(model) == LoginStatus.Success)
                {
                    if (SessionHelper.LoginUser.IsDistributor)
                    {
                        SessionHelper.DistributorBalance = orderBLL.GetBalance(SessionHelper.LoginUser.Distributor.DistributorSAPCode, _configuration);
                    }
                    jsonResponse.Status = true;
                    jsonResponse.Message = "Login Successfully";
                    jsonResponse.RedirectURL = Url.Action("Index", "Home");
                    return Json(new { data = jsonResponse });
                }
                jsonResponse.Status = true;
                jsonResponse.Message = "Password changed Successfully.";
                jsonResponse.RedirectURL = Url.Action("Index", "Home");
                return Json(new { data = jsonResponse });
            }
            catch (Exception ex)
            {
                new ErrorLogBLL(_unitOfWork).AddExceptionLog(ex);
                jsonResponse.Status = false;
                jsonResponse.Message = NotificationMessage.ErrorOccurred;
                return Json(new { data = jsonResponse });
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Logout()
        {
            new AuditLogBLL(_unitOfWork).AddAuditLog("Login", "Logout", "Start Click on Logout Button of ");
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Login");
        }
    }
}
