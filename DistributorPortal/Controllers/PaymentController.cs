﻿using BusinessLogicLayer.Application;
using BusinessLogicLayer.ApplicationSetup;
using BusinessLogicLayer.ErrorLog;
using BusinessLogicLayer.GeneralSetup;
using BusinessLogicLayer.HelperClasses;
using DataAccessLayer.WorkProcess;
using DistributorPortal.BusinessLogicLayer.ApplicationSetup;
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
    public class PaymentController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly OrderDetailBLL _OrderDetailBLL;
        private readonly PaymentBLL _PaymentBLL;
        private readonly IConfiguration _IConfiguration;
        private readonly Configuration _Configuration;
        public PaymentController(IUnitOfWork unitOfWork, IConfiguration _iconfiguration, Configuration _configuration)
        {
            _unitOfWork = unitOfWork;
            _PaymentBLL = new PaymentBLL(_unitOfWork);
            _OrderDetailBLL = new OrderDetailBLL(_unitOfWork);
            _IConfiguration = _iconfiguration;
            _Configuration = _configuration;
        }
        // GET: Payment
        public ActionResult Index()
        {
            new AuditTrailBLL(_unitOfWork).AddAuditTrail("Payment", "Index", " Form");
            PaymentViewModel model = new PaymentViewModel();
            model.PaymentMaster = GetPaymentList();
            model.DistributorList = new DistributorBLL(_unitOfWork).DropDownDistributorList(null);
            return View(model);
        }
        public PaymentViewModel List(PaymentViewModel model)
        {
            if (model.DistributorId is null && model.Status is null && model.FromDate is null && model.ToDate is null)
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
            new AuditTrailBLL(_unitOfWork).AddAuditTrail("Payment", "Add", "Click on Add  Button of ");
            return PartialView("Add", BindPaymentMaster(id));
        }
        [HttpGet]
        public IActionResult PaymentApproval(int id)
        {
            new AuditTrailBLL(_unitOfWork).AddAuditTrail("Payment", "PaymentApproval", "Click on Approval Button of ");
            return PartialView("PaymentApproval", BindPaymentMaster(id));
        }
        [HttpPost]
        public JsonResult SaveEdit(PaymentMaster model)
        {
            JsonResponse jsonResponse = new JsonResponse();
            string FolderPath = _IConfiguration.GetSection("Settings").GetSection("FolderPath").Value;
            try
            {
                new AuditTrailBLL(_unitOfWork).AddAuditTrail("Payment", "SaveEdit", "Start Click on SaveEdit Button of ");
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
                new AuditTrailBLL(_unitOfWork).AddAuditTrail("Payment", "SaveEdit", "End Click on Save Button of ");
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
            }
            else
            {

            }
            model.Distributor = SessionHelper.LoginUser.Distributor;
            model.PaymentModeList = new PaymentModeBLL(_unitOfWork).DropDownPaymentModeList();
            model.CompanyList = new CompanyBLL(_unitOfWork).DropDownCompanyList(model.CompanyId);
            model.DepostitorBankList = new BankBLL(_unitOfWork).DropDownBankList(model.CompanyId, model.DepositorBankName);
            model.CompanyListBankList = new BankBLL(_unitOfWork).DropDownBankList(model.CompanyId, model.CompanyBankName);
            model.SAMITotalPendingValue = (from od in _OrderDetailBLL.GetAllOrderDetail()
                                           join p in new ProductDetailBLL(_unitOfWork).GetAllProductDetail() on od.ProductId equals p.ProductMasterId
                                           where od.ProductId == p.ProductMasterId && p.CompanyId == 1
                                           group new { od, p } by new { od.OrderId, p.CompanyId } into odp
                                           let Amount = odp.Sum(m => m.od.Amount)
                                           select Amount).Sum(x => x);
            model.HealthTekTotalPendingValue = (from od in _OrderDetailBLL.GetAllOrderDetail()
                                                join p in new ProductDetailBLL(_unitOfWork).GetAllProductDetail() on od.ProductId equals p.ProductMasterId
                                                where od.ProductId == p.ProductMasterId && p.CompanyId == 3
                                                group new { od, p } by new { od.OrderId, p.CompanyId } into odp
                                                let Amount = odp.Sum(m => m.od.Amount)
                                                select Amount).Sum(x => x);
            model.PhytekTotalPendingValue = (from od in _OrderDetailBLL.GetAllOrderDetail()
                                             join p in new ProductDetailBLL(_unitOfWork).GetAllProductDetail() on od.ProductId equals p.ProductMasterId
                                             where od.ProductId == p.ProductMasterId && p.CompanyId == 2
                                             group new { od, p } by new { od.OrderId, p.CompanyId } into odp
                                             let Amount = odp.Sum(m => m.od.Amount)
                                             select Amount).Sum(x => x);
            return model;
        }
        [HttpPost]
        public JsonResult UpdateStatus(int id, PaymentStatus Status, string Remarks)
        {
            JsonResponse jsonResponse = new JsonResponse();
            bool result = false;
            try
            {
                new AuditTrailBLL(_unitOfWork).AddAuditTrail("Payment", "UpdateStatus", "Start Click on Approve Button of ");
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
                new AuditTrailBLL(_unitOfWork).AddAuditTrail("Payment", "UpdateStatus", "End Click on Approve Button of ");
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
            new AuditTrailBLL(_unitOfWork).AddAuditTrail("Payment", "Search", "Start Click on Search Button of ");
            if (!string.IsNullOrEmpty(Search))
            {
                model = List(model);
            }
            else
            {
                model.PaymentMaster = GetPaymentList();
            }
            new AuditTrailBLL(_unitOfWork).AddAuditTrail("Payment", "Search", "End Click on Search Button of ");
            return PartialView("List", model.PaymentMaster);
        }
        public List<PaymentMaster> GetPaymentList()
        {
            var list = _PaymentBLL.GetAllPaymentMaster().Where(x => SessionHelper.LoginUser.IsDistributor == true ? x.DistributorId == SessionHelper.LoginUser.DistributorId : true).OrderByDescending(x => x.Id).ToList();
            return list;
        }
    }
}
