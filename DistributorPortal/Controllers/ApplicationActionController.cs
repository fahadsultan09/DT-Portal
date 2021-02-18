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
    public class ApplicationActionController : BaseController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ApplicationActionBLL _ApplicationActionBLL;
        public ApplicationActionController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _ApplicationActionBLL = new ApplicationActionBLL(_unitOfWork);
        }
        // GET: ApplicationAction
        public IActionResult Index()
        {
            return View(_ApplicationActionBLL.GetAllApplicationAction());
        }
        public IActionResult List()
        {
            return PartialView("List", _ApplicationActionBLL.GetAllApplicationAction());
        }
        [HttpGet]
        public IActionResult Add(string DPID)
        {
            int id=0;
            if (!string.IsNullOrEmpty(DPID))
            {
                int.TryParse(EncryptDecrypt.Decrypt(DPID), out id);
            }            
            return PartialView("Add", BindApplicationAction(id));
        }
        [HttpPost]
        public IActionResult SaveEdit(ApplicationAction model)
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
                    if (_ApplicationActionBLL.CheckApplicationActionName(model.Id, model.ActionName))
                    {
                        if (model.Id > 0)
                        {
                            _ApplicationActionBLL.UpdateApplicationAction(model);
                            TempData["Message"] = NotificationMessage.UpdateSuccessfully;
                        }
                        else
                        {
                            _ApplicationActionBLL.AddApplicationAction(model);
                            TempData["Message"] = NotificationMessage.SaveSuccessfully;
                        }
                    }
                    else
                    {
                        TempData["Message"] = "ApplicationAction name already exist";
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
                int id=0;
                int.TryParse(EncryptDecrypt.Decrypt(DPID), out id);
                _ApplicationActionBLL.DeleteApplicationAction(id);
                return Json(new { Result = true });
            }
            catch (Exception ex)
            {
                new ErrorLogBLL(_unitOfWork).AddExceptionLog(ex);
                TempData["Message"] = NotificationMessage.ErrorOccurred;
                return RedirectToAction("List");
            }
        }
        private ApplicationAction BindApplicationAction(int id)
        {
            ApplicationAction model = new ApplicationAction();            
            if (id > 0)
            {
                model = _ApplicationActionBLL.GetApplicationActionById(id);
            }
            else
            {
                model.IsActive = true;
            }
            return model;
        }
        public JsonResult GetApplicationActionList()
        {
            return Json(_ApplicationActionBLL.GetAllApplicationAction().ToList());
        }
    }
}