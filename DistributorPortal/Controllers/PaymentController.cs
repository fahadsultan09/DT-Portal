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
using Models.ViewModel;
using Newtonsoft.Json;
using RestSharp;
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
    public class PaymentController : BaseController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly PaymentBLL _PaymentBLL;
        private readonly OrderBLL _OrderBLL;
        private readonly IConfiguration _IConfiguration;
        private readonly Configuration _Configuration;
        public PaymentController(IUnitOfWork unitOfWork, IConfiguration _iconfiguration, Configuration _configuration)
        {
            _unitOfWork = unitOfWork;
            _PaymentBLL = new PaymentBLL(_unitOfWork);
            _OrderBLL = new OrderBLL(_unitOfWork);
            _IConfiguration = _iconfiguration;
            _Configuration = _configuration;
        }
        // GET: Payment
        public ActionResult Index()
        {
            new AuditLogBLL(_unitOfWork).AddAuditLog("Payment", "Index", " Form");
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
        public IActionResult Add(int id)
        {
            new AuditLogBLL(_unitOfWork).AddAuditLog("Payment", "Add", "Click on Add  Button of ");
            return View("Add", BindPaymentMaster(id));
        }
        [HttpGet]
        public IActionResult PaymentApproval(int id)
        {
            new AuditLogBLL(_unitOfWork).AddAuditLog("Payment", "PaymentApproval", "Click on Approval Button of ");
            return View("PaymentApproval", BindPaymentMaster(id));
        }
        [HttpPost]
        public JsonResult SaveEdit(PaymentMaster model)
        {
            JsonResponse jsonResponse = new JsonResponse();
            string FolderPath = _IConfiguration.GetSection("Settings").GetSection("FolderPath").Value;
            try
            {
                new AuditLogBLL(_unitOfWork).AddAuditLog("Payment", "SaveEdit", "Start Click on SaveEdit Button of ");
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
                        if (permittedExtensions.Contains(ext) && model.FormFile.Length < Convert.ToInt64(5242880))
                        {
                            Tuple<bool, string> tuple = FileUtility.UploadFile(model.FormFile, FolderName.Order, FolderPath);
                            if (tuple.Item1)
                            {
                                model.File = tuple.Item2;
                            }
                        }
                    }
                    model.Status = PaymentStatus.Unverified;
                    model.DistributorId = (int)SessionHelper.LoginUser.DistributorId;
                    _PaymentBLL.Add(model);
                    jsonResponse.Status = true;
                    jsonResponse.Message = NotificationMessage.PaymentSaved;
                }
                new AuditLogBLL(_unitOfWork).AddAuditLog("Payment", "SaveEdit", "End Click on Save Button of ");
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
                model.Distributor = new DistributorBLL(_unitOfWork).GetAllDistributor().Where(x => x.Id == model.DistributorId).FirstOrDefault();
            }
            else
            {
                model.Distributor = SessionHelper.LoginUser.Distributor;
            }

            if (SessionHelper.LoginUser.IsDistributor)
            {
                SessionHelper.SAPOrderPendingValue = _OrderBLL.GetPendingOrderValue(SessionHelper.LoginUser.Distributor.DistributorSAPCode, _Configuration).ToList();
            }
            else
            {
                SessionHelper.SAPOrderPendingValue = _OrderBLL.GetPendingOrderValue(model.Distributor.DistributorSAPCode, _Configuration).ToList();
            }
            model.PaymentValueViewModel = _PaymentBLL.GetOrderValueModel();
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
            bool result = false;
            try
            {
                new AuditLogBLL(_unitOfWork).AddAuditLog("Payment", "UpdateStatus", "Start Click on Approve Button of ");
                if (Status == PaymentStatus.Verified)
                {
                    var Client = new RestClient(_Configuration.PostPayment);
                    var request = new RestRequest(Method.POST).AddJsonBody(_PaymentBLL.AddPaymentToSAP(id), "json");
                    IRestResponse restResponse = Client.Execute(request);
                    var SAPPaymentStatus = JsonConvert.DeserializeObject<SAPPaymentStatus>(restResponse.Content);
                    var payment = _PaymentBLL.Where(e => e.Id == id).FirstOrDefault();

                    if (payment != null)
                    {
                        payment.SAPCompanyCode = SAPPaymentStatus.SAPCompanyCode;
                        payment.SAPDocumentNumber = SAPPaymentStatus.SAPDocumentNumber;
                        payment.SAPFiscalYear = SAPPaymentStatus.SAPFiscalYear;
                        payment.Status = PaymentStatus.Verified;
                        result = _PaymentBLL.Update(payment);

                        jsonResponse.Status = result;
                        jsonResponse.Message = result ? "Payment has been verified." : "Unable to verfied payment.";
                        jsonResponse.RedirectURL = Url.Action("Index", "Payment");
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
                PaymentMaster model = _PaymentBLL.GetById(id);
                if (model != null)
                {
                    _PaymentBLL.UpdateStatus(model, Status, Remarks);
                }
                _unitOfWork.Save();
                new AuditLogBLL(_unitOfWork).AddAuditLog("Payment", "UpdateStatus", "End Click on Approve Button of ");
                jsonResponse.Status = true;
                jsonResponse.Message = NotificationMessage.PaymentVerified;
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
        [HttpPost]
        public IActionResult Search(PaymentViewModel model, string Search)
        {
            new AuditLogBLL(_unitOfWork).AddAuditLog("Payment", "Search", "Start Click on Search Button of ");
            if (!string.IsNullOrEmpty(Search))
            {
                model = List(model);
            }
            else
            {
                model.PaymentMaster = GetPaymentList();
            }
            new AuditLogBLL(_unitOfWork).AddAuditLog("Payment", "Search", "End Click on Search Button of ");
            return PartialView("List", model.PaymentMaster);
        }
        public List<PaymentMaster> GetPaymentList()
        {
            var list = _PaymentBLL.GetAllPaymentMaster().Where(x => SessionHelper.LoginUser.IsDistributor == true ? x.DistributorId == SessionHelper.LoginUser.DistributorId : true).OrderByDescending(x => x.Id).ToList();
            return list;
        }
        public DistributorBalance GetDistributorBalance(string DistributorSAPCode)
        {
            try
            {

                var Client = new RestClient(_Configuration.SyncDistributorBalanceURL + "/Get?DistributorId=" + DistributorSAPCode);
                var request = new RestRequest(Method.GET);
                IRestResponse response = Client.Execute(request);
                var resp = JsonConvert.DeserializeObject<DistributorBalance>(response.Content);
                if (resp == null)
                {
                    return new DistributorBalance();
                }
                else
                {
                    return resp;
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
