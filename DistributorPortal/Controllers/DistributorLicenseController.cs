using BusinessLogicLayer.Application;
using BusinessLogicLayer.ApplicationSetup;
using BusinessLogicLayer.ErrorLog;
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
using Utility;
using Utility.Constant;
using Utility.HelperClasses;
using static Utility.Constant.Common;

namespace DistributorPortal.Controllers
{
    public class DistributorLicenseController : BaseController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly DistributorLicenseBLL _DistributorLicenseBLL;
        private readonly DistributorBLL _distributorBll;
        private readonly LicenseControlBLL _licenseControlBLL;
        private readonly IConfiguration _IConfiguration;
        private readonly AuditTrailBLL<DistributorLicense> _AuditTrailDistributorLicense;
        private readonly Configuration _Configuration;
        public DistributorLicenseController(IUnitOfWork unitOfWork, IConfiguration configuration, Configuration _configuration)
        {
            _unitOfWork = unitOfWork;
            _DistributorLicenseBLL = new DistributorLicenseBLL(_unitOfWork);
            _distributorBll = new DistributorBLL(_unitOfWork);
            _licenseControlBLL = new LicenseControlBLL(_unitOfWork);
            _IConfiguration = configuration;
            _AuditTrailDistributorLicense = new AuditTrailBLL<DistributorLicense>(_unitOfWork);
            _Configuration = _configuration;
        }
        // GET: DistributorLicense
        public IActionResult Index()
        {
            DistributorLicenseViewModel model = new DistributorLicenseViewModel();
            model.LicenseControlList = _licenseControlBLL.DropDownLicenseControlList(0);
            model.DistributorList = new DistributorBLL(_unitOfWork).DropDownDistributorList(null);
            model.DistributorLicenseList = GetDistributorLicenseList();
            return View(model);
        }
        public IActionResult Add()
        {
            var licenseControl = _licenseControlBLL.Where(x => x.IsActive && !x.IsDeleted);
            var DistributorLicenseList = new List<DistributorLicense>();
            List<DistributorLicense> distributorLicensesHistory = new List<DistributorLicense>();
            var License = _DistributorLicenseBLL.Where(e => e.DistributorId == SessionHelper.LoginUser.DistributorId).ToList();
            if (License.Count > 0)
            {
                DistributorLicenseList.AddRange(License);
                DistributorLicenseList.ForEach(x => x = x.Expiry.Date <= DateTime.Now.Date ? new DistributorLicense() : x);
            }
            foreach (var item in licenseControl)
            {
                if (!DistributorLicenseList.Any(e => e.LicenseId == item.Id))
                {
                    DistributorLicenseList.Add(new DistributorLicense()
                    {
                        LicenseControl = item
                    });
                }
            }
            List<AuditTrail> ATBLLDistributorLicense = _AuditTrailDistributorLicense.Where(x => x.PageId == (int)ApplicationPages.DistributorLicense && (x.ActionId == (int)ApplicationActions.Update || x.ActionId == (int)ApplicationActions.Insert));
            foreach (var item in ATBLLDistributorLicense)
            {
                var license = JsonConvert.DeserializeObject<DistributorLicense>(item.JsonObject.ToString());
                license.LicenseControl = new LicenseControl { LicenseName = licenseControl.FirstOrDefault(x => x.Id == license.LicenseId).LicenseName };
                distributorLicensesHistory.Add(license);
            }
            ViewBag.distributorLicensesHistory = distributorLicensesHistory.Where(x => x.DistributorId == SessionHelper.LoginUser.DistributorId).ToList();
            return View(DistributorLicenseList);
        }
        [HttpPost]
        public JsonResult SaveEdit(DistributorLicense model)
        {
            JsonResponse jsonResponse = new JsonResponse();
            string FolderPath = _IConfiguration.GetSection("Settings").GetSection("LicenseFolderPath").Value;
            string[] permittedExtensions = Common.permittedExtensions;
            try
            {
                if (model.File is null)
                {
                    jsonResponse.Status = false;
                    jsonResponse.Message = "Add Attachment";
                    return Json(new { data = jsonResponse });
                }
                if (!ModelState.IsValid)
                {
                    jsonResponse.Status = false;
                    var message = string.Join(" <br />", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                    jsonResponse.Message = message.Replace("'--Select option--'", "");
                    return Json(new { data = jsonResponse });
                }
                if (model.File != null)
                {
                    var ext = Path.GetExtension(model.File.FileName).ToLowerInvariant();

                    if (string.IsNullOrEmpty(ext) || !permittedExtensions.Contains(ext))
                    {
                        jsonResponse.Status = false;
                        jsonResponse.Message = NotificationMessage.FileTypeAllowed;
                        return Json(new { data = jsonResponse });
                    }
                    if (model.File.Length >= Convert.ToInt64(_Configuration.FileSize))
                    {
                        jsonResponse.Status = false;
                        jsonResponse.Message = NotificationMessage.FileSizeAllowed;
                        return Json(new { data = jsonResponse });
                    }
                    Tuple<bool, string> tuple = FileUtility.UploadFile(model.File, FolderName.Complaint, FolderPath);
                    if (tuple.Item1)
                    {
                        model.Attachment = tuple.Item2;
                    }
                }
                if (model.Id > 0)
                {
                    if (model.Attachment != null)
                    {
                        _DistributorLicenseBLL.Update(model);
                    }
                }
                else
                {
                    if (model.Attachment != null)
                    {
                        _DistributorLicenseBLL.Add(model);
                    }
                }
                jsonResponse.Status = true;
                jsonResponse.Message = NotificationMessage.AddLicense;
                jsonResponse.RedirectURL = Url.Action("Add", "DistributorLicense");
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
        public JsonResult UpdateStatus(int Id, LicenseStatus Status, string Remarks)
        {
            JsonResponse jsonResponse = new JsonResponse();
            try
            {
                DistributorLicense model = _DistributorLicenseBLL.GetById(Id);
                if (model != null)
                {
                    _DistributorLicenseBLL.UpdateStatus(model, Status, Remarks);
                }

                if (Status == LicenseStatus.Verified)
                {
                    jsonResponse.Status = true;
                    jsonResponse.Message = model.LicenseControl.LicenseName + " License approved successfully.";
                }
                else
                {
                    jsonResponse.Status = false;
                    jsonResponse.Message = model.LicenseControl.LicenseName + " License has been rejected.";
                }
                _unitOfWork.Save();

                jsonResponse.RedirectURL = Url.Action("Index", "DistributorLicense");
                jsonResponse.SignalRResponse = new SignalRResponse() { UserId = model.CreatedBy.ToString(), Number = "License #: " + string.Format("{0:1000000000}", model.Id), Message = jsonResponse.Message, Status = Enum.GetName(typeof(LicenseStatus), model.Status) };
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
        public DistributorLicenseViewModel List(DistributorLicenseViewModel model)
        {
            if (model.FromDate is null && model.ToDate is null && model.Status is null && model.DistributorId is null && model.LicenseId is null)
            {
                model.DistributorLicenseList = GetDistributorLicenseList();
            }
            else
            {
                model.DistributorLicenseList = _DistributorLicenseBLL.Search(model).Where(x => SessionHelper.LoginUser.IsDistributor == true ? x.DistributorId == SessionHelper.LoginUser.DistributorId : true).ToList();
            }
            return model;
        }
        public List<DistributorLicense> GetDistributorLicenseList()
        {
            var list = _DistributorLicenseBLL.Where(x => SessionHelper.LoginUser.IsDistributor == true ? x.DistributorId == SessionHelper.LoginUser.DistributorId : true).OrderByDescending(x => x.Id).ToList();
            return list;
        }
        [HttpPost]
        public IActionResult Search(DistributorLicenseViewModel model, string Search)
        {
            if (!string.IsNullOrEmpty(Search))
            {
                model = List(model);
            }
            else
            {
                model.DistributorLicenseList = GetDistributorLicenseList();
            }
            return PartialView("List", model.DistributorLicenseList);
        }
    }
}
