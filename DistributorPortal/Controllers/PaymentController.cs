﻿using BusinessLogicLayer.Application;
using BusinessLogicLayer.ApplicationSetup;
using BusinessLogicLayer.ErrorLog;
using BusinessLogicLayer.GeneralSetup;
using BusinessLogicLayer.HelperClasses;
using DataAccessLayer.WorkProcess;
using DistributorPortal.Resource;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Models.Application;
using Models.ViewModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Utility;
using Utility.Constant;
using Utility.HelperClasses;
using static Utility.Constant.Common;

namespace DistributorPortal.Controllers
{
    public class PaymentController : BaseController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly PaymentBLL _PaymentBLL;
        private readonly OrderBLL _OrderBLL;
        private readonly NotificationBLL _NotificationBLL;
        private readonly IConfiguration _IConfiguration;
        private readonly Configuration _Configuration;
        public PaymentController(IUnitOfWork unitOfWork, IConfiguration _iconfiguration, Configuration _configuration)
        {
            _unitOfWork = unitOfWork;
            _PaymentBLL = new PaymentBLL(_unitOfWork);
            _OrderBLL = new OrderBLL(_unitOfWork);
            _NotificationBLL = new NotificationBLL(_unitOfWork);
            _IConfiguration = _iconfiguration;
            _Configuration = _configuration;
        }
        // GET: Payment
        public ActionResult Index()
        {
            PaymentViewModel model = new PaymentViewModel();
            model.PaymentMaster = GetPaymentList();
            model.DistributorList = new DistributorBLL(_unitOfWork).DropDownDistributorList(null);
            return View(model);
        }
        public PaymentViewModel List(PaymentViewModel model)
        {
            if (model.DistributorId is null && model.Status is null && model.FromDate is null && model.ToDate is null && model.PaymentNo is null)
            {
                model.PaymentMaster = GetPaymentList();
            }
            else
            {
                model.PaymentMaster = _PaymentBLL.Search(model).Where(x => SessionHelper.LoginUser.IsDistributor == true ? x.DistributorId == SessionHelper.LoginUser.DistributorId : true).ToList();
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
            return View("Add", BindPaymentMaster(id));
        }
        [HttpGet]
        public IActionResult PaymentApproval(string DPID)
        {
            int.TryParse(EncryptDecrypt.Decrypt(DPID), out int id);
            return View("PaymentApproval", BindPaymentMaster(id));
        }
        [HttpGet]
        public IActionResult PaymentView(string DPID, string RedirectURL)
        {
            TempData["RedirectURL"] = RedirectURL;
            int.TryParse(EncryptDecrypt.Decrypt(DPID), out int id);
            return View("PaymentView", BindPaymentMaster(id));
        }
        [HttpPost]
        public JsonResult SaveEdit(PaymentMaster model)
        {
            JsonResponse jsonResponse = new JsonResponse();
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
                        Tuple<bool, string> tuple = FileUtility.UploadFile(model.FormFile, FolderName.Payment, FolderPath);
                        if (tuple.Item1)
                        {
                            model.File = tuple.Item2;
                        }
                    }
                    model.Status = PaymentStatus.Unverified;
                    model.DistributorId = (int)SessionHelper.LoginUser.DistributorId;
                    _PaymentBLL.Add(model);
                    if (model.Id > 0)
                    {
                        _PaymentBLL.UpdateSNo(model);
                    }
                    jsonResponse.Status = true;
                    jsonResponse.Message = NotificationMessage.PaymentSaved;
                }
                jsonResponse.RedirectURL = Url.Action("Index", "Payment");
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
        private PaymentMaster BindPaymentMaster(int Id)
        {
            PaymentMaster model = new PaymentMaster();
            if (Id > 0)
            {
                model = _PaymentBLL.GetById(Id);
                SessionHelper.DistributorBalance = GetDistributorBalance(model.Distributor.DistributorSAPCode);
                model.Distributor = new DistributorBLL(_unitOfWork).Where(x => x.Id == model.DistributorId).FirstOrDefault();
            }
            else
            {
                model.Distributor = SessionHelper.LoginUser.Distributor;
            }

            if (SessionHelper.LoginUser.IsDistributor)
            {
                if (SessionHelper.SAPOrderPendingValue == null)
                {
                    SessionHelper.SAPOrderPendingValue = _OrderBLL.GetPendingOrderValue(SessionHelper.LoginUser.Distributor.DistributorSAPCode, _Configuration).ToList();
                }
                model.PaymentValueViewModel = _PaymentBLL.GetOrderValueModel(SessionHelper.LoginUser.Distributor.Id);
            }
            else
            {
                if (SessionHelper.SAPOrderPendingValue == null)
                {
                    SessionHelper.SAPOrderPendingValue = _OrderBLL.GetPendingOrderValue(model.Distributor.DistributorSAPCode, _Configuration).ToList();
                }
                model.PaymentValueViewModel = _PaymentBLL.GetOrderValueModel(model.Distributor.Id);
            }
            model.PaymentModeList = new PaymentModeBLL(_unitOfWork).DropDownPaymentModeList();
            model.CompanyList = new CompanyBLL(_unitOfWork).DropDownCompanyList(model.CompanyId, true);
            model.DepostitorBankList = new BankBLL(_unitOfWork).DropDownBankList(model.CompanyId, model.DepositorBankName);
            model.CompanyBankList = new BankBLL(_unitOfWork).DropDownBankList(model.CompanyId, model.CompanyBankName);
            return model;
        }
        [HttpPost]
        public JsonResult UpdateStatus(int id, PaymentStatus Status, string Remarks)
        {
            JsonResponse jsonResponse = new JsonResponse();
            Notification notification = new Notification();
            SAPPaymentStatus SAPPaymentStatus = new SAPPaymentStatus();
            try
            {
                PaymentMaster model = _PaymentBLL.GetById(id);

                if (model.Status == PaymentStatus.Verified || model.Status == PaymentStatus.Rejected)
                {
                    jsonResponse.Status = false;
                    jsonResponse.Message = "Payment already " + model.Status;
                    jsonResponse.RedirectURL = Url.Action("Index", "Payment");
                    return Json(new { data = jsonResponse });
                }
                if (Status == PaymentStatus.Verified)
                {
                    //var Client = new RestClient(_Configuration.PostPayment);
                    //var request = new RestRequest(Method.POST).AddJsonBody(_PaymentBLL.AddPaymentToSAP(id), "json");
                    //IRestResponse restResponse = Client.Execute(request);
                    //var SAPPaymentStatus = JsonConvert.DeserializeObject<SAPPaymentStatus>(restResponse.Content);
                    SAPPaymentViewModel sAPPaymentViewModel = _PaymentBLL.AddPaymentToSAP(id);
                    Root root = new Root();
                    using (var client = new HttpClient())
                    {
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        string authInfo = Convert.ToBase64String(Encoding.Default.GetBytes(_Configuration.POUserName + ":" + _Configuration.POPassword));
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authInfo);
                        var result = client.GetAsync(new Uri(string.Format("http://10.0.3.35:51000/RESTAdapter/payment?REF={0}&COMPANY={1}&AMOUNT={2}&DISTRIBUTOR={3}&B_CODE={4}&PAY_ID={5}", sAPPaymentViewModel.REF, sAPPaymentViewModel.COMPANY, sAPPaymentViewModel.AMOUNT, sAPPaymentViewModel.DISTRIBUTOR, sAPPaymentViewModel.B_CODE, sAPPaymentViewModel.PAY_ID))).Result;
                        if (result.IsSuccessStatusCode)
                        {
                            var JsonContent = result.Content.ReadAsStringAsync().Result;
                            root = JsonConvert.DeserializeObject<Root>(JsonContent.ToString());
                        }
                    }

                    if (root != null && root.ZWAS_PAYMENT_BAPI_DP != null && !string.IsNullOrEmpty(root.ZWAS_PAYMENT_BAPI_DP.COMPANYY))
                    {
                        model.SAPCompanyCode = root.ZWAS_PAYMENT_BAPI_DP.COMPANYY;
                        model.SAPDocumentNumber = root.ZWAS_PAYMENT_BAPI_DP.DOCUMENT;
                        model.SAPFiscalYear = root.ZWAS_PAYMENT_BAPI_DP.FISCAL;
                        model.Status = PaymentStatus.Verified;
                        bool result = _PaymentBLL.Update(model);
                        _PaymentBLL.UpdateStatus(model, Status, Remarks);

                        jsonResponse.Status = result;
                        jsonResponse.Message = result ? "Payment has been verified." : "Unable to verfied payment.";
                        jsonResponse.RedirectURL = Url.Action("Index", "Payment");
                        jsonResponse.SignalRResponse = new SignalRResponse() { UserId = model.CreatedBy.ToString(), Number = "Request #: " + model.SNo, Message = jsonResponse.Message, Status = Enum.GetName(typeof(PaymentStatus), model.Status) };
                        notification.ApplicationPageId = (int)ApplicationPages.Payment;
                        notification.DistributorId = model.DistributorId;
                        notification.RequestId = model.SNo;
                        notification.Status = model.Status.ToString();
                        notification.Message = jsonResponse.SignalRResponse.Message;
                        notification.URL = "/Payment/PaymentView?DPID=" + EncryptDecrypt.Encrypt(id.ToString());
                        _NotificationBLL.Add(notification);
                        return Json(new { data = jsonResponse });
                    }
                    else
                    {
                        jsonResponse.Status = false;
                        jsonResponse.Message = "Unable to verfied payment.";
                        jsonResponse.RedirectURL = Url.Action("Index", "Payment");
                        return Json(new { data = jsonResponse });
                    }
                }
                if (model != null)
                {
                    _PaymentBLL.UpdateStatus(model, Status, Remarks);
                    jsonResponse.Status = true;
                    jsonResponse.Message = "Payment " + Status.ToString().ToLower() + " successfully.";
                    jsonResponse.RedirectURL = Url.Action("Index", "Payment");
                    jsonResponse.SignalRResponse = new SignalRResponse() { UserId = model.CreatedBy.ToString(), Number = "Request #: " + model.SNo, Message = jsonResponse.Message, Status = Enum.GetName(typeof(PaymentStatus), model.Status) };
                    notification.ApplicationPageId = (int)ApplicationPages.Payment;
                    notification.DistributorId = model.DistributorId;
                    notification.RequestId = model.SNo;
                    notification.Status = model.Status.ToString();
                    notification.Message = jsonResponse.SignalRResponse.Message;
                    notification.URL = "/Payment/PaymentView?DPID=" + EncryptDecrypt.Encrypt(id.ToString());
                    _NotificationBLL.Add(notification);
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
        public IActionResult Search(PaymentViewModel model, string Search)
        {
            if (!string.IsNullOrEmpty(Search))
            {
                model = List(model);
            }
            else
            {
                model.PaymentMaster = GetPaymentList();
            }
            return PartialView("List", model.PaymentMaster);
        }
        public List<PaymentMaster> GetPaymentList()
        {
            var list = _PaymentBLL.Where(x => SessionHelper.LoginUser.IsDistributor == true ? x.DistributorId == SessionHelper.LoginUser.DistributorId : true).ToList();
            return list;
        }
        public DistributorBalance GetDistributorBalance(string DistributorSAPCode)
        {
            try
            {
                DistributorBalance distributorBalance = new DistributorBalance();
                Root root = new Root();
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    string authInfo = Convert.ToBase64String(Encoding.Default.GetBytes(_Configuration.POUserName + ":" + _Configuration.POPassword));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authInfo);
                    var result = client.GetAsync(new Uri("http://10.0.3.35:51000/RESTAdapter/DistBal?DISTRIBUTOR=" + DistributorSAPCode)).Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var JsonContent = result.Content.ReadAsStringAsync().Result;
                        root = JsonConvert.DeserializeObject<Root>(JsonContent);
                    }
                }
                if (distributorBalance == null)
                {
                    return new DistributorBalance();
                }
                else
                {
                    distributorBalance.SAMI = root.ZWASITDPDISTBALANCEBAPIResponse.BAL_SAMI;
                    distributorBalance.HealthTek = root.ZWASITDPDISTBALANCEBAPIResponse.BAL_HTL;
                    return distributorBalance;
                }
            }
            catch (Exception ex)
            {
                new ErrorLogBLL(_unitOfWork).AddExceptionLog(ex);
                return new DistributorBalance();
            }
        }
    }
}
