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
using System.Runtime.InteropServices;
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
            Tuple<string,string> item = GetClientIPAndMacAddress();
            model.RegisteredAddress = item.Item2;
            new AuditLogBLL(_unitOfWork).AddAuditLog(item.Item2.ToString(), item.Item1.ToString(), "");
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
            else if(login.CheckLogin(model) == LoginStatus.NotRegistered)
            {
                jsonResponse.Status = false;
                jsonResponse.Message = "You are not registered.";
                jsonResponse.RedirectURL = string.Empty;
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
            Tuple<string, string> item = GetClientIPAndMacAddress();
            model.RegisteredAddress = item.Item2;
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
        [DllImport("Iphlpapi.dll")]
        private static extern int SendARP(int dest, int host, ref long mac, ref int length);

        [DllImport("Ws2_32.dll")]
        private static extern Int32 inet_addr(string ip);
        public Tuple<string, string> GetClientIPAndMacAddress()
        {
            string IPAddress = string.Empty;
            string MacAddress = string.Empty;
            try
            {
                string userip = this.HttpContext.Connection.RemoteIpAddress.ToString();
                string strClientIP = this.HttpContext.Connection.RemoteIpAddress.ToString();
                Int32 ldest = inet_addr(strClientIP);
                Int32 lhost = inet_addr("");
                Int64 macinfo = new Int64();
                Int32 len = 6;
                int res = SendARP(ldest, 0, ref macinfo, ref len);
                string mac_src = macinfo.ToString("X");
                if (mac_src == "0")
                {
                    IPAddress = userip;
                    return new Tuple<string, string>(IPAddress, MacAddress);
                }
                while (mac_src.Length < 12)
                {
                    mac_src = mac_src.Insert(0, "0");
                }

                string mac_dest = "";

                for (int i = 0; i < 11; i++)
                {
                    if (0 == (i % 2))
                    {
                        if (i == 10)
                        {
                            mac_dest = mac_dest.Insert(0, mac_src.Substring(i, 2));
                        }
                        else
                        {
                            mac_dest = "-" + mac_dest.Insert(0, mac_src.Substring(i, 2));
                        }
                    }
                }
                IPAddress = userip;
                MacAddress = mac_dest;
                return new Tuple<string, string>(IPAddress, MacAddress);
            }
            catch (Exception err)
            {
                Response.WriteAsync(err.Message);
            }
            return new Tuple<string, string>(IPAddress, MacAddress);
        }
    }
}
