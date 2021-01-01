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
using Newtonsoft.Json;
using RestSharp;
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
        private readonly SessionHelper sessionHelper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly LoginBLL login;
        private readonly UserBLL _UserBLL;
        private readonly IConfiguration _IConfiguration;
        private readonly Configuration _configuration;

        public LoginController(IUnitOfWork unitOfWork, IHttpContextAccessor HhttpContextAccessor, IConfiguration IConfiguration, Configuration configuration)
        {
            _unitOfWork = unitOfWork;
            _httpContextAccessor = HhttpContextAccessor;
            sessionHelper = new SessionHelper(_httpContextAccessor);
            login = new LoginBLL(_unitOfWork);
            _UserBLL = new UserBLL(_unitOfWork);
            _IConfiguration = IConfiguration;
            _configuration = configuration;
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
            if (login.CheckUserPassword(model, password))
            {
                jsonResponse.Status = false;
                jsonResponse.Message = "Please change default password.";
                jsonResponse.RedirectURL = string.Empty;
                return Json(new { data = jsonResponse });
            }
            if (login.CheckLogin(model) == LoginStatus.Success)
            {
                SessionHelper.DistributorBalance = GetDistributorBalance();
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
                        SessionHelper.DistributorBalance = GetDistributorBalance();
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

            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Login");
        }
        public double GetDistributorBalance()
        {
            try
            {
                var Client = new RestClient(_configuration.SyncDistributorBalanceURL);
                var request = new RestRequest(Method.GET);
                IRestResponse response = Client.Execute(request);
                return Convert.ToDouble(JsonConvert.DeserializeObject<List<Distributor>>(response.Content));
            }
            catch (Exception ex)
            {
                return 0;
            }
        }
    }
}
