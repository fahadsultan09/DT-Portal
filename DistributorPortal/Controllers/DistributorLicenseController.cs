using BusinessLogicLayer.Application;
using BusinessLogicLayer.ApplicationSetup;
using BusinessLogicLayer.ErrorLog;
using BusinessLogicLayer.HelperClasses;
using DataAccessLayer.WorkProcess;
using DistributorPortal.BusinessLogicLayer.ApplicationSetup;
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
    public class DistributorLicenseController : BaseController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly DistributorLicenseBLL _DistributorLicenseBLL;
        private readonly DistributorBLL _distributorBll;
        private readonly LicenseControlBLL _licenseControlBLL;
        private readonly IConfiguration _IConfiguration;
        public DistributorLicenseController(IUnitOfWork unitOfWork, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _DistributorLicenseBLL = new DistributorLicenseBLL(_unitOfWork);
            _distributorBll = new DistributorBLL(_unitOfWork);
            _licenseControlBLL = new LicenseControlBLL(_unitOfWork);
            _IConfiguration = configuration;
        }
        // GET: DistributorLicense
        public IActionResult Index()
        {
            ViewBag.Distributor = _distributorBll.DropDownDistributorList(null);
            ViewBag.License = _licenseControlBLL.DropDownLicenseControlList(0);
            var model = _DistributorLicenseBLL.GetAllDistributorLicense();
            return View(model);
        }
        public IActionResult Add()
        {
            var licenseControl = _licenseControlBLL.GetAllLicenseControl();
            var model = new List<DistributorLicense>();
            var License = _DistributorLicenseBLL.Where(e => e.DistributorId == SessionHelper.LoginUser.DistributorId).ToList();
            if (License.Count > 0)
            {
                model.AddRange(License);
            }
            foreach (var item in licenseControl)
            {
                if (!model.Any(e => e.LicenseId == item.Id))
                {
                    model.Add(new DistributorLicense()
                    {
                        LicenseControl = item
                    });
                }
            }
            return View(model);
        }
        [HttpPost]
        public IActionResult SaveEdit(List<DistributorLicense> model)
        {
            JsonResponse jsonResponse = new JsonResponse();
            string FolderPath = _IConfiguration.GetSection("Settings").GetSection("LicenseFolderPath").Value;
            try
            {
                foreach (var item in model)
                {
                    string[] permittedExtensions = Common.permittedExtensions;
                    if (item.File != null)
                    {
                        var ext = Path.GetExtension(item.File.FileName).ToLowerInvariant();
                        if (permittedExtensions.Contains(ext) && item.File.Length < Convert.ToInt64(5242880))
                        {
                            Tuple<bool, string> tuple = FileUtility.UploadFile(item.File, FolderName.Order, FolderPath);
                            if (tuple.Item1)
                            {
                                item.Attachment = tuple.Item2;
                            }
                        }
                        item.Status = LicenseStatus.Submitted;
                        item.DistributorId = (int)SessionHelper.LoginUser.DistributorId;
                        item.IsActive = true;
                        item.IsDeleted = false;
                        if (item.Id > 0)
                        {
                            if (item.Attachment != null)
                            {
                                _DistributorLicenseBLL.Update(item);
                            }
                        }
                        else
                        {
                            if (item.Attachment != null)
                            {
                                _DistributorLicenseBLL.Add(item);
                            }

                        }
                    }
                    
                }
                jsonResponse.Status = true;
                jsonResponse.Message = NotificationMessage.AddLicense;
                jsonResponse.RedirectURL = Url.Action("Index", "DistributorLicense");
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
                    jsonResponse.Message = "License approved successfully.";
                }
                else
                {
                    jsonResponse.Message = "License has been rejected.";
                }
                _unitOfWork.Save();

                jsonResponse.Status = true;
                jsonResponse.RedirectURL = Url.Action("Index", "DistributorLicense");
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

    }
}
