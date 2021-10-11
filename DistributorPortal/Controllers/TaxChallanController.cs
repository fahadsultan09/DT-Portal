using BusinessLogicLayer.Application;
using BusinessLogicLayer.ApplicationSetup;
using BusinessLogicLayer.ErrorLog;
using BusinessLogicLayer.GeneralSetup;
using BusinessLogicLayer.HelperClasses;
using DataAccessLayer.WorkProcess;
using DistributorPortal.Resource;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Models.Application;
using Models.UserRights;
using Models.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Utility;
using Utility.Constant;
using Utility.HelperClasses;
using static Utility.Constant.Common;

namespace DistributorPortal.Controllers
{
    public class TaxChallanController : BaseController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly TaxChallanBLL _TaxChallanBLL;
        private readonly NotificationBLL _NotificationBLL;
        private readonly IConfiguration _IConfiguration;
        private readonly Configuration _Configuration;
        public TaxChallanController(IUnitOfWork unitOfWork, IConfiguration _iconfiguration, Configuration _configuration)
        {
            _unitOfWork = unitOfWork;
            _TaxChallanBLL = new TaxChallanBLL(_unitOfWork);
            _NotificationBLL = new NotificationBLL(_unitOfWork);
            _IConfiguration = _iconfiguration;
            _Configuration = _configuration;
        }
        // GET: TaxChallan
        public ActionResult Index()
        {
            TaxChallanViewModel model = new TaxChallanViewModel();
            model.TaxChallan = GetTaxChallanList();
            model.DistributorList = new DistributorBLL(_unitOfWork).DropDownDistributorList(null);
            return View(model);
        }
        public TaxChallanViewModel List(TaxChallanViewModel model)
        {
            if (model.DistributorId is null && model.Status is null && model.FromDate is null && model.ToDate is null && model.TaxChallanNo is null)
            {
                model.TaxChallan = GetTaxChallanList();
            }
            else
            {
                model.TaxChallan = _TaxChallanBLL.Search(model).Where(x => SessionHelper.LoginUser.IsDistributor ? x.DistributorId == SessionHelper.LoginUser.DistributorId : true).ToList();
            }
            return model;
        }
        [HttpGet]
        public IActionResult Add(string DPID)
        {
            int id = 0;
            if (!string.IsNullOrEmpty(DPID))
            {
                int.TryParse(EncryptDecrypt.Decrypt(DPID), out id);
            }
            return View("Add", BindTaxChallan(id));
        }
        [HttpGet]
        public IActionResult TaxChallanApproval(string DPID)
        {
            int.TryParse(EncryptDecrypt.Decrypt(DPID), out int id);
            return View("TaxChallanApproval", BindTaxChallan(id));
        }
        [HttpGet]
        public IActionResult TaxChallanView(string DPID, string RedirectURL)
        {
            TempData["RedirectURL"] = RedirectURL;
            int.TryParse(EncryptDecrypt.Decrypt(DPID), out int id);
            return View("TaxChallanView", BindTaxChallan(id));
        }
        [HttpPost]
        public JsonResult SaveEdit(TaxChallan model)
        {
            JsonResponse jsonResponse = new JsonResponse();
            Notification notification = new Notification();
            string FolderPath = _IConfiguration.GetSection("Settings").GetSection("FolderPath").Value;
            try
            {
                ModelState.Remove("Id");
                if (!ModelState.IsValid)
                {
                    jsonResponse.Status = false;
                    jsonResponse.Message = NotificationMessage.RequiredFieldsValidation;
                }
                else
                {
                    string[] permittedExtensions = Common.permittedExtensions;
                    if (model.FormFile != null)
                    {
                        var ext = Path.GetExtension(model.FormFile.FileName).ToLowerInvariant();

                        if (string.IsNullOrEmpty(ext) || !permittedExtensions.Contains(ext))
                        {
                            jsonResponse.Status = false;
                            jsonResponse.Message = NotificationMessage.FileTypeAllowed;
                            return Json(new { data = jsonResponse });
                        }
                        if (model.FormFile.Length >= Convert.ToInt64(_Configuration.FileSize))
                        {
                            jsonResponse.Status = false;
                            jsonResponse.Message = NotificationMessage.FileSizeAllowed;
                            return Json(new { data = jsonResponse });
                        }
                        Tuple<bool, string> tuple = FileUtility.UploadFile(model.FormFile, FolderName.TaxChallan, FolderPath);
                        if (tuple.Item1)
                        {
                            model.Attachment = tuple.Item2;
                        }
                    }
                    _TaxChallanBLL.Add(model);
                    if (model.Id > 0)
                    {
                        _TaxChallanBLL.UpdateSNo(model);
                    }
                    jsonResponse.Status = true;
                    jsonResponse.Message = NotificationMessage.TaxChallanSaved;
                }
                jsonResponse.RedirectURL = Url.Action("Index", "TaxChallan");
                RolePermission rolePermission = new RolePermissionBLL(_unitOfWork).FirstOrDefault(e => e.ApplicationPageId == (int)ApplicationPages.ApproveTaxChallan && e.ApplicationActionId == (int)ApplicationActions.Approve);
                if (rolePermission != null)
                {
                    jsonResponse.SignalRResponse = new SignalRResponse() { RoleCompanyIds = rolePermission.RoleId.ToString(), Number = "Request #: " + model.SNo, Message = jsonResponse.Message, Status = Enum.GetName(typeof(TaxChallanStatus), model.Status) };
                    notification.CompanyId = SessionHelper.LoginUser.CompanyId;
                    notification.ApplicationPageId = (int)ApplicationPages.TaxChallan;
                    notification.DistributorId = model.DistributorId;
                    notification.RequestId = model.SNo;
                    notification.Status = model.Status.ToString();
                    notification.Message = jsonResponse.SignalRResponse.Message;
                    notification.URL = "/TaxChallan/TaxChallanView?DPID=" + EncryptDecrypt.Encrypt(model.Id.ToString());
                    _NotificationBLL.Add(notification);
                }
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
        private TaxChallan BindTaxChallan(int Id)
        {
            TaxChallan model = new TaxChallan();
            if (Id > 0)
            {
                model = _TaxChallanBLL.GetById(Id);
                model.Distributor = new DistributorBLL(_unitOfWork).Where(x => x.Id == model.DistributorId).FirstOrDefault();
            }
            else
            {
                model.Distributor = SessionHelper.LoginUser.Distributor;
            }
            model.PaymentSectionList = new PaymentSectionBLL(_unitOfWork).DropDownPaymentSectionList();
            return model;
        }
        [HttpPost]
        public JsonResult UpdateStatus(int id, TaxChallanStatus Status, string Remarks)
        {
            JsonResponse jsonResponse = new JsonResponse();
            Notification notification = new Notification();
            //SAPTaxChallanStatus SAPTaxChallanStatus = new SAPTaxChallanStatus();
            try
            {
                TaxChallan model = _TaxChallanBLL.GetById(id);

                if (model.Status == TaxChallanStatus.Verified || model.Status == TaxChallanStatus.Rejected)
                {
                    jsonResponse.Status = false;
                    jsonResponse.Message = "Tax Challan already " + model.Status;
                    jsonResponse.RedirectURL = Url.Action("Index", "TaxChallan");
                    return Json(new { data = jsonResponse });
                }
                if (Status == TaxChallanStatus.Verified)
                {
                    //var Client = new RestClient(_Configuration.PostTaxChallan);
                    //var request = new RestRequest(Method.POST).AddJsonBody(_TaxChallanBLL.AddTaxChallanToSAP(id), "json");
                    //IRestResponse restResponse = Client.Execute(request);
                    //var SAPTaxChallanStatus = JsonConvert.DeserializeObject<SAPTaxChallanStatus>(restResponse.Content);
                    //SAPTaxChallanViewModel sAPTaxChallanViewModel = _TaxChallanBLL.AddTaxChallanToSAP(id);
                    //Root root = new Root();
                    //using (var client = new HttpClient())
                    //{
                    //    client.DefaultRequestHeaders.Accept.Clear();
                    //    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    //    string authInfo = Convert.ToBase64String(Encoding.Default.GetBytes(_Configuration.POUserName + ":" + _Configuration.POPassword));
                    //    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authInfo);
                    //    var result = client.GetAsync(new Uri(string.Format(_Configuration.PostTaxChallan, sAPTaxChallanViewModel.REF, sAPTaxChallanViewModel.COMPANY, sAPTaxChallanViewModel.AMOUNT, sAPTaxChallanViewModel.DISTRIBUTOR, sAPTaxChallanViewModel.B_CODE, sAPTaxChallanViewModel.PAY_ID))).Result;
                    //    if (result.IsSuccessStatusCode)
                    //    {
                    //        var JsonContent = result.Content.ReadAsStringAsync().Result;
                    //        root = JsonConvert.DeserializeObject<Root>(JsonContent.ToString());
                    //    }
                    //}

                    //if (root != null && root.ZWAS_TaxChallan_BAPI_DP != null && !string.IsNullOrEmpty(root.ZWAS_TaxChallan_BAPI_DP.COMPANYY))
                    //{
                    //    model.Status = TaxChallanStatus.Verified;
                    //    bool result = _TaxChallanBLL.Update(model);
                    //    _TaxChallanBLL.UpdateStatus(model, Status, Remarks);

                    //    jsonResponse.Status = result;
                    //    jsonResponse.Message = result ? "Tax Challan has been verified." : "Unable to verified TaxChallan.";
                    //    jsonResponse.RedirectURL = Url.Action("Index", "TaxChallan");
                    //    jsonResponse.SignalRResponse = new SignalRResponse() { UserId = model.CreatedBy.ToString(), Number = "Request #: " + model.SNo, Message = jsonResponse.Message, Status = Enum.GetName(typeof(TaxChallanStatus), model.Status) };
                    //    notification.CompanyId = SessionHelper.LoginUser.CompanyId;
                    //    notification.ApplicationPageId = (int)ApplicationPages.TaxChallan;
                    //    notification.DistributorId = model.DistributorId;
                    //    notification.RequestId = model.SNo;
                    //    notification.Status = model.Status.ToString();
                    //    notification.Message = jsonResponse.SignalRResponse.Message;
                    //    notification.URL = "/TaxChallan/TaxChallanView?DPID=" + EncryptDecrypt.Encrypt(id.ToString());
                    //    _NotificationBLL.Add(notification);
                    //    return Json(new { data = jsonResponse });
                    //}
                    //else
                    //{
                    //    jsonResponse.Status = false;
                    //    jsonResponse.Message = "Unable to verified TaxChallan.";
                    //    jsonResponse.RedirectURL = Url.Action("Index", "TaxChallan");
                    //    return Json(new { data = jsonResponse });
                    //}
                }
                if (model != null)
                {
                    _TaxChallanBLL.UpdateStatus(model, Status, Remarks);
                    jsonResponse.Status = true;
                    if (model.Status == TaxChallanStatus.Rejected)
                    {
                        jsonResponse.Message = "Tax Challan has been rejected";
                    }
                    else
                    {
                        jsonResponse.Message = "Tax Challan " + Status.ToString().ToLower() + " successfully";
                    }
                    jsonResponse.RedirectURL = Url.Action("Index", "TaxChallan");

                    if (model.Status != TaxChallanStatus.Canceled)
                    {
                        jsonResponse.SignalRResponse = new SignalRResponse() { UserId = model.CreatedBy.ToString(), Number = "Request #: " + model.SNo, Message = jsonResponse.Message, Status = Enum.GetName(typeof(TaxChallanStatus), model.Status) };
                        notification.CompanyId = SessionHelper.LoginUser.CompanyId;
                        notification.ApplicationPageId = (int)ApplicationPages.TaxChallan;
                        notification.DistributorId = model.DistributorId;
                        notification.RequestId = model.SNo;
                        notification.Status = model.Status.ToString();
                        notification.Message = jsonResponse.SignalRResponse.Message;
                        notification.URL = "/TaxChallan/TaxChallanView?DPID=" + EncryptDecrypt.Encrypt(id.ToString());
                        _NotificationBLL.Add(notification);
                    }

                }
                _unitOfWork.Save();
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
        [HttpPost]
        public IActionResult Search(TaxChallanViewModel model, string Search)
        {
            if (!string.IsNullOrEmpty(Search))
            {
                model = List(model);
            }
            else
            {
                model.TaxChallan = GetTaxChallanList();
            }
            return PartialView("List", model.TaxChallan);
        }
        public List<TaxChallan> GetTaxChallanList()
        {
            var list = _TaxChallanBLL.Where(x => SessionHelper.LoginUser.IsDistributor ? x.DistributorId == SessionHelper.LoginUser.DistributorId : true).ToList();
            return list;
        }
    }
}
