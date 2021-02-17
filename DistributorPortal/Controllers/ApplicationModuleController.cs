using BusinessLogicLayer.Application;
using BusinessLogicLayer.ErrorLog;
using BusinessLogicLayer.GeneralSetup;
using DataAccessLayer.WorkProcess;
using DistributorPortal.Resource;
using Microsoft.AspNetCore.Mvc;
using Models.UserRights;
using System;
using System.Linq;
using Utility.HelperClasses;

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
            return View(_ApplicationModuleBLL.GetAllApplicationModule());
        }
        public IActionResult List()
        {
            return PartialView("List", _ApplicationModuleBLL.GetAllApplicationModule());
        }
        [HttpGet]
        public IActionResult Add(string DPID)
        {
            int id;
            int.TryParse(EncryptDecrypt.Decrypt(DPID), out id);
            return PartialView("Add", BindApplicationModule(id));
        }
        [HttpPost]
        public IActionResult SaveEdit(ApplicationModule model)
        {
            try
            {
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
                        TempData["Message"] = "Application Module name already exist";
                        return PartialView("Add", model);
                    }
                }
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
        public IActionResult Delete(string DPID)
        {
            try
            {
                int id;
                int.TryParse(EncryptDecrypt.Decrypt(DPID), out id);
                _ApplicationModuleBLL.DeleteApplicationModule(id);
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