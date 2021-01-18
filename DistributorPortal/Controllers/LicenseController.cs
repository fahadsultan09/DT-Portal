using BusinessLogicLayer.Application;
using BusinessLogicLayer.ErrorLog;
using DataAccessLayer.WorkProcess;
using DistributorPortal.BusinessLogicLayer.ApplicationSetup;
using DistributorPortal.Resource;
using Microsoft.AspNetCore.Mvc;
using Models.Application;
using System;
using System.Linq;

namespace DistributorPortal.Controllers
{
    public class LicenseController : BaseController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly LicenseControlBLL _LicenseControlBLL;
        public LicenseController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _LicenseControlBLL = new LicenseControlBLL(_unitOfWork);
        }
        // GET: LicenseControl
        public IActionResult Index()
        {
            new AuditTrailBLL(_unitOfWork).AddAuditTrail("LicenseControl", "Index", " Form");
            return View(_LicenseControlBLL.GetAllLicenseControl());
        }
        public IActionResult List()
        {
            return PartialView("List", _LicenseControlBLL.GetAllLicenseControl());
        }
        [HttpGet]
        public IActionResult Add(int id)
        {
            new AuditTrailBLL(_unitOfWork).AddAuditTrail("LicenseControl", "Add", "Click on Add  Button of ");
            return PartialView("Add", BindLicenseControl(id));
        }
        [HttpPost]
        public IActionResult SaveEdit(LicenseControl model)
        {
            try
            {
                new AuditTrailBLL(_unitOfWork).AddAuditTrail("LicenseControl", "SaveEdit", "Start Click on SaveEdit Button of ");
                ModelState.Remove("Id");
                if (!ModelState.IsValid)
                {
                    TempData["Message"] = NotificationMessage.RequiredFieldsValidation;
                    return PartialView("Add", model);
                }
                else
                {
                    if (_LicenseControlBLL.CheckLicenseControlName(model.Id, model.LicenseName))
                    {
                        if (model.Id > 0)
                        {
                            _LicenseControlBLL.UpdateLicenseControl(model);
                            TempData["Message"] = NotificationMessage.UpdateSuccessfully;
                        }
                        else
                        {
                            _LicenseControlBLL.AddLicenseControl(model);
                            TempData["Message"] = NotificationMessage.SaveSuccessfully;
                        }
                    }
                    else
                    {
                        new AuditTrailBLL(_unitOfWork).AddAuditTrail("LicenseControl", "SaveEdit", "End Click on Save Button of ");
                        TempData["Message"] = "LicenseControl name already exist";
                        return PartialView("Add", model);
                    }
                }
                new AuditTrailBLL(_unitOfWork).AddAuditTrail("LicenseControl", "SaveEdit", "End Click on Save Button of ");
                return RedirectToAction("List");
            }
            catch (Exception ex)
            {
                new ErrorLogBLL(_unitOfWork).AddExceptionLog(ex);
                TempData["Message"] = NotificationMessage.ErrorOccurred;
                return RedirectToAction("List");
            }
        }
        [HttpPost]
        public IActionResult Delete(int id)
        {
            try
            {
                new AuditTrailBLL(_unitOfWork).AddAuditTrail("LicenseControl", "Delete", "Start Click on Delete Button of ");
                _LicenseControlBLL.DeleteLicenseControl(id);
                new AuditTrailBLL(_unitOfWork).AddAuditTrail("LicenseControl", "Delete", "End Click on Delete Button of ");
                return Json(new { Result = true });
            }
            catch (Exception ex)
            {
                new ErrorLogBLL(_unitOfWork).AddExceptionLog(ex);
                TempData["Message"] = NotificationMessage.ErrorOccurred;
                return RedirectToAction("List");
            }
        }
        private LicenseControl BindLicenseControl(int Id)
        {
            LicenseControl model = new LicenseControl();
            if (Id > 0)
            {
                model = _LicenseControlBLL.GetLicenseControlById(Id);
            }
            else
            {
                model.IsActive = true;
            }
            return model;
        }
        public JsonResult GetLicenseControlList()
        {
            return Json(_LicenseControlBLL.GetAllLicenseControl().ToList());
        }
    }
}