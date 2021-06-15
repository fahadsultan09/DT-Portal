using BusinessLogicLayer.ErrorLog;
using BusinessLogicLayer.GeneralSetup;
using DataAccessLayer.WorkProcess;
using DistributorPortal.Resource;
using Microsoft.AspNetCore.Mvc;
using Models.UserRights;
using Models.ViewModel;
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
            JsonResponse jsonResponse = new JsonResponse();
            try
            {
                TempData["Message"] = string.Empty;
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
                            jsonResponse.Status = true;
                            jsonResponse.Message = NotificationMessage.UpdateSuccessfully;
                            jsonResponse.RedirectURL = Url.Action("Index", "ApplicationAction");
                        }
                        else
                        {
                            _ApplicationActionBLL.AddApplicationAction(model);
                            jsonResponse.Status = true;
                            jsonResponse.Message = NotificationMessage.SaveSuccessfully;
                            jsonResponse.RedirectURL = Url.Action("Index", "ApplicationAction");
                        }
                    }
                    else
                    {
                        TempData["Message"] = "Application Action name already exist";
                        return PartialView("Add", model);
                    }
                }
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