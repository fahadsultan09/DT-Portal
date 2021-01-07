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
        private readonly PaymentBLL _PaymentBLL;
        private readonly IConfiguration _IConfiguration;
        public PaymentController(IUnitOfWork unitOfWork, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _PaymentBLL = new PaymentBLL(_unitOfWork);
            _IConfiguration = configuration;
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
            if (model.DistributorId == 0 && model.Status is null && model.FromDate is null && model.ToDate is null)
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
            return PartialView("Add", BindPaymentMaster(id));
        }
        [HttpGet]
        public IActionResult PaymentApproval(int id)
        {
            return PartialView("PaymentApproval", BindPaymentMaster(id));
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
                }
                jsonResponse.Status = true;
                jsonResponse.Message = NotificationMessage.PaymentSaved;
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
            model.BankList = new BankBLL(_unitOfWork).DropDownBankList();
            model.CompanyList = new CompanyBLL(_unitOfWork).DropDownCompanyList();
            return model;
        }
        [HttpPost]
        public JsonResult UpdateStatus(int id, PaymentStatus Status)
        {
            JsonResponse jsonResponse = new JsonResponse();
            try
            {
                PaymentMaster model = _PaymentBLL.GetById(id);
                if (model != null)
                {
                    _PaymentBLL.UpdateStatus(model, Status);
                }
                _unitOfWork.Save();
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
            return _PaymentBLL.GetAllPaymentMaster().Where(x => SessionHelper.LoginUser.IsDistributor == true ? x.DistributorId == SessionHelper.LoginUser.DistributorId : true).ToList();
        }
    }
}
