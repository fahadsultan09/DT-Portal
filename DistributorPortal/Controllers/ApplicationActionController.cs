using BusinessLogicLayer.ErrorLog;
using BusinessLogicLayer.GeneralSetup;
using DataAccessLayer.WorkProcess;
using DistributorPortal.Resource;
using Microsoft.AspNetCore.Mvc;
using Models.UserRights;
using System;
using System.Linq;

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
        public ActionResult Index()
        {
            return View(_ApplicationActionBLL.GetAllApplicationAction());
        }
        public IActionResult List()
        {
            return PartialView("List", _ApplicationActionBLL.GetAllApplicationAction());
        }
        [HttpGet]
        public IActionResult Add(int id)
        {
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
        public IActionResult Delete(int id)
        {
            try
            {
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
        private ApplicationAction BindApplicationAction(int Id)
        {
            ApplicationAction model = new ApplicationAction();
            if (Id > 0)
            {
                model = _ApplicationActionBLL.GetApplicationActionById(Id);
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