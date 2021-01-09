using BusinessLogicLayer.Application;
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
    public class ComplaintController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ComplaintBLL _ComplaintBLL;
        private readonly IConfiguration _IConfiguration;
        public ComplaintController(IUnitOfWork unitOfWork, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _ComplaintBLL = new ComplaintBLL(_unitOfWork);
            _IConfiguration = configuration;
        }
        // GET: Complaint
        public IActionResult Index()
        {
            new AuditTrailBLL(_unitOfWork).AddAuditTrail("Complaint", "Index", " Form");
            return View(GetComplaintList());
        }
        public IActionResult List()
        {
            return PartialView("List", GetComplaintList());
        }
        [HttpGet]
        public IActionResult Add(int id)
        {
            new AuditTrailBLL(_unitOfWork).AddAuditTrail("Complaint", "Add", "Click on Add  Button of ");
            return PartialView("Add", BindComplaint(id));
        }
        [HttpGet]
        public IActionResult ComplaintApproval(int id)
        {
            return PartialView("ComplaintApproval", BindComplaint(id));
        }
        [HttpPost]
        public JsonResult SaveEdit(Complaint model)
        {
            JsonResponse jsonResponse = new JsonResponse();
            string FolderPath = _IConfiguration.GetSection("Settings").GetSection("FolderPath").Value;
            try
            {
                new AuditTrailBLL(_unitOfWork).AddAuditTrail("Complaint", "SaveEdit", "Start Click on SaveEdit Button of ");
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
                    model.Status = ComplaintStatus.Pending;
                    model.DistributorId = (int)SessionHelper.LoginUser.DistributorId;
                    _ComplaintBLL.Add(model);
                }
                new AuditTrailBLL(_unitOfWork).AddAuditTrail("Complaint", "SaveEdit", "End Click on Save Button of ");
                jsonResponse.Status = true;
                jsonResponse.Message = NotificationMessage.SaveSuccessfully;
                jsonResponse.RedirectURL = Url.Action("Index", "Complaint");
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
        private Complaint BindComplaint(int Id)
        {
            Complaint model = new Complaint();
            if (Id > 0)
            {
                model = _ComplaintBLL.GetById(Id);
            }
            else
            {

            }
            model.Distributor = SessionHelper.LoginUser.Distributor;
            model.ComplaintCategoryList = new ComplaintCategoryBLL(_unitOfWork).DropDownComplaintCategoryList(model.ComplaintCategoryId);
            model.ComplaintSubCategoryList = new ComplaintSubCategoryBLL(_unitOfWork).DropDownComplaintSubCategoryList(model.ComplaintSubCategoryId);
            return model;
        }
        [HttpPost]
        public JsonResult UpdateStatus(int id, ComplaintStatus Status, string Remarks)
        {
            JsonResponse jsonResponse = new JsonResponse();
            try
            {
                new AuditTrailBLL(_unitOfWork).AddAuditTrail("Complaint", "UpdateStatus", "Start Click on Resolve Button of ");
                Complaint model = _ComplaintBLL.GetById(id);
                if (model != null)
                {
                    _ComplaintBLL.UpdateStatus(model, Status, Remarks);
                }
                _unitOfWork.Save();
                new AuditTrailBLL(_unitOfWork).AddAuditTrail("Complaint", "UpdateStatus", "End Click on Resolve Button of ");
                jsonResponse.Status = true;
                jsonResponse.Message = NotificationMessage.Resolved;
                jsonResponse.RedirectURL = Url.Action("Index", "Complaint");
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
        public List<Complaint> GetComplaintList()
        {
            if (SessionHelper.LoginUser.IsDistributor)
            {
                return _ComplaintBLL.GetAllComplaint().Where(x => x.DistributorId == SessionHelper.LoginUser.DistributorId).OrderByDescending(x => x.Id).ToList();
            }
            else
            {
                return _ComplaintBLL.GetAllComplaint().OrderByDescending(x => x.Id).ToList();
            }
        }
    }
}
