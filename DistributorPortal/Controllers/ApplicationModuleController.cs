using BusinessLogicLayer.ErrorLog;
using BusinessLogicLayer.GeneralSetup;
using DataAccessLayer.WorkProcess;
using DistributorPortal.BusinessLogicLayer.ApplicationSetup;
using DistributorPortal.Resource;
using Microsoft.AspNetCore.Mvc;
using Models.UserRights;
using System;
using System.Linq;

namespace DistributorPortal.Controllers
{
    public class ApplicationModuleController : BaseController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ApplicationModuleBLL _ApplicationModuleBLL;
        public ApplicationModuleController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _ApplicationModuleBLL = new ApplicationModuleBLL(_unitOfWork);
        }
        // GET: ApplicationModule
        public IActionResult Index()
        {
            new AuditTrailBLL(_unitOfWork).AddAuditTrail("ApplicationModule", "Index", " Form");
            return View(_ApplicationModuleBLL.GetAllApplicationModule());
        }
        public IActionResult List()
        {
            return PartialView("List", _ApplicationModuleBLL.GetAllApplicationModule());
        }
        [HttpGet]
        public IActionResult Add(int id)
        {
            new AuditTrailBLL(_unitOfWork).AddAuditTrail("ApplicationModule", "Add", "Click on Add  Button of ");
            return PartialView("Add", BindApplicationModule(id));
        }
        [HttpPost]
        public IActionResult SaveEdit(ApplicationModule model)
        {
            try
            {
                new AuditTrailBLL(_unitOfWork).AddAuditTrail("ApplicationModule", "SaveEdit", "Start Click on SaveEdit Button of ");
                ModelState.Remove("Id");
                if (!ModelState.IsValid)
                {
                    TempData["Message"] = NotificationMessage.RequiredFieldsValidation;
                    return PartialView("Add", model);
                }
                else
                {
                    if (_ApplicationModuleBLL.CheckApplicationModuleName(model.Id, model.ModuleName))
                    {
                        if (model.Id > 0)
                        {
                            _ApplicationModuleBLL.UpdateApplicationModule(model);
                            TempData["Message"] = NotificationMessage.UpdateSuccessfully;
                        }
                        else
                        {
                            _ApplicationModuleBLL.AddApplicationModule(model);
                            TempData["Message"] = NotificationMessage.SaveSuccessfully;
                        }
                    }
                    else
                    {
                        new AuditTrailBLL(_unitOfWork).AddAuditTrail("ApplicationModule", "SaveEdit", "End Click on Save Button of ");
                        TempData["Message"] = "Application Module name already exist";
                        return PartialView("Add", model);
                    }
                }
                new AuditTrailBLL(_unitOfWork).AddAuditTrail("ApplicationModule", "SaveEdit", "End Click on Save Button of ");
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
                new AuditTrailBLL(_unitOfWork).AddAuditTrail("ApplicationModule", "Delete", "Start Click on Delete Button of ");
                _ApplicationModuleBLL.DeleteApplicationModule(id);
                new AuditTrailBLL(_unitOfWork).AddAuditTrail("ApplicationModule", "Delete", "End Click on Delete Button of ");
                return Json(new { Result = true });
            }
            catch (Exception ex)
            {
                new ErrorLogBLL(_unitOfWork).AddExceptionLog(ex);
                TempData["Message"] = NotificationMessage.ErrorOccurred;
                return RedirectToAction("List");
            }
        }
        private ApplicationModule BindApplicationModule(int Id)
        {
            ApplicationModule model = new ApplicationModule();
            if (Id > 0)
            {
                model = _ApplicationModuleBLL.GetApplicationModuleById(Id);
            }
            else
            {
                model.IsActive = true;
            }
            return model;
        }
        public JsonResult GetApplicationModuleList()
        {
            return Json(_ApplicationModuleBLL.GetAllApplicationModule().ToList());
        }
    }
}