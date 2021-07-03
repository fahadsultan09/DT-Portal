using BusinessLogicLayer.Application;
using BusinessLogicLayer.ErrorLog;
using BusinessLogicLayer.GeneralSetup;
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
        private readonly DisclaimerBLL _DisclaimerBLL;

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
            _DisclaimerBLL = new DisclaimerBLL(_unitOfWork);
        }
        public IActionResult Index()
        {
            User user = new User();
            LoginBLL login = new LoginBLL(_unitOfWork);
            ViewBag.msg = string.Empty;

            string AccessToken = HttpContext.Request.Query["AccessToken"].ToString();
            if (string.IsNullOrEmpty(AccessToken))
            {
                return View();
            }
            else
            {
                AccessToken = EncryptDecrypt.Decrypt(AccessToken);
                user = JsonConvert.DeserializeObject<User>(AccessToken);
                user.AccessToken = AccessToken;
                new AuditLogBLL(_unitOfWork).AddAuditLog("", user.UserName, user.MacAddresses);
            }
            if (string.IsNullOrEmpty(Convert.ToString(user.UpdatedDate)))
            {
                return View();
            }
            else
            {
                if (Convert.ToDateTime(user.UpdatedDate).Date != DateTime.Now.Date)
                {
                    return View();
                }
            }
            if (login.CheckLogin(user) == LoginStatus.Success)
            {
                return RedirectPermanent("/Home/Index");
            }
            else
            {
                ViewBag.msg = "Invalid username or password";
                return View();
            }
        }
        [HttpPost]
        public JsonResult Index(User model)
        {
            //Tuple<string, string> item = GetClientIPAndMacAddress();
            //ViewBag.MacAddress = model.RegisteredAddress = item.Item2;
            //ViewBag.IPAddress = item.Item1;
            //string mac = GetMacAddress(item.Item1);
            //new AuditLogBLL(_unitOfWork).AddAuditLog(item.Item2.ToString(), item.Item1.ToString(), "");
            //new AuditLogBLL(_unitOfWork).AddAuditLog(mac, "", "");
            JsonResponse jsonResponse = new JsonResponse();
            if (string.IsNullOrEmpty(model.UserName) && string.IsNullOrEmpty(model.Password))
            {
                jsonResponse.Status = false;
                jsonResponse.Message = "Your session has expired.";
                jsonResponse.RedirectURL = Url.Action("Index", "Login");
                return Json(new { data = jsonResponse });
            }
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
                if (_DisclaimerBLL.GetAllDisclaimer().FirstOrDefault() != null)
                {
                    SessionHelper.Disclaimer = _DisclaimerBLL.GetAllDisclaimer().FirstOrDefault(x => x.IsActive)?.Description;
                }
                else
                {
                    SessionHelper.Disclaimer = string.Empty;
                }
                SessionHelper.DistributorBalance = SessionHelper.LoginUser.IsDistributor ? orderBLL.GetBalance(SessionHelper.LoginUser.Distributor.DistributorSAPCode, _configuration) : null;
                SessionHelper.URL = _configuration.URL.ToString();
                jsonResponse.Status = true;
                jsonResponse.Message = "Login successfully";
                jsonResponse.RedirectURL = Url.Action("Index", "Home");
                new AuditLogBLL(_unitOfWork).AddAuditLog("Login", "Index", "End Click on Login Button of ");
                return Json(new { data = jsonResponse });
            }
            else if (login.CheckLogin(model) == LoginStatus.NotRegistered)
            {
                jsonResponse.Status = false;
                jsonResponse.Message = "You are not registered.";
                jsonResponse.RedirectURL = string.Empty;
                return Json(new { data = jsonResponse });
            }
            else
            {
                jsonResponse.Status = false;
                jsonResponse.Message = "Invalid username or password.";
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
                    jsonResponse.Message = "Login successfully";
                    jsonResponse.RedirectURL = Url.Action("Index", "Home");
                    return Json(new { data = jsonResponse });
                }
                jsonResponse.Status = true;
                jsonResponse.Message = "Password changed successfully.";
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
            if (SessionHelper.LoginUser != null  && SessionHelper.LoginUser.IsDistributor)
            {
                HttpContext.Session.Clear();
                SessionHelper.LoginUser = null;
                SessionHelper.NavigationMenu = null;
                SessionHelper.AddProduct = null;
                SessionHelper.AddDistributorWiseProduct = null;
                SessionHelper.AddReturnProduct = null;
                SessionHelper.DistributorBalance = null;
                SessionHelper.SAPOrderPendingQuantity = null;
                SessionHelper.SAPOrderPendingValue = null;
                return View();
            }
            else
            {
                new AuditLogBLL(_unitOfWork).AddAuditLog("Login", "Logout", "Start Click on Logout Button of ");
                HttpContext.Session.Clear();
                SessionHelper.LoginUser = null;
                SessionHelper.NavigationMenu = null;
                SessionHelper.AddProduct = null;
                SessionHelper.AddDistributorWiseProduct = null;
                SessionHelper.AddReturnProduct = null;
                SessionHelper.DistributorBalance = null;
                SessionHelper.SAPOrderPendingQuantity = null;
                SessionHelper.SAPOrderPendingValue = null;
                return RedirectToAction("Index", "Login");
            }
            
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
                string userip = Response.HttpContext.Connection.RemoteIpAddress.ToString();
                string strClientIP = Response.HttpContext.Connection.RemoteIpAddress.ToString();
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
        public string GetMacAddress(string ipAddress)
        {
            string macAddress = string.Empty;
            System.Diagnostics.Process pProcess = new System.Diagnostics.Process();
            pProcess.StartInfo.FileName = "arp";
            pProcess.StartInfo.Arguments = "-a " + ipAddress;
            pProcess.StartInfo.UseShellExecute = false;
            pProcess.StartInfo.RedirectStandardOutput = true;
            pProcess.StartInfo.CreateNoWindow = true;
            pProcess.Start();
            string strOutput = pProcess.StandardOutput.ReadToEnd();
            string[] substrings = strOutput.Split('-');
            if (substrings.Length >= 8)
            {
                macAddress = substrings[3].Substring(Math.Max(0, substrings[3].Length - 2))
                    + "-" + substrings[4] + "-" + substrings[5] + "-" + substrings[6]
                    + "-" + substrings[7] + "-"
                    + substrings[8].Substring(0, 2);
                return macAddress;
            }
            else
            {
                return "not found";
            }
        }
    }
}
