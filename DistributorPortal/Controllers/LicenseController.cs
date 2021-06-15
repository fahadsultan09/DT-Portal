using BusinessLogicLayer.Application;
using BusinessLogicLayer.ErrorLog;
using DataAccessLayer.WorkProcess;
using DistributorPortal.Resource;
using Microsoft.AspNetCore.Mvc;
using Models.Application;
using Models.ViewModel;
using System;
using System.Linq;
using Utility.HelperClasses;

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
            return View(_LicenseControlBLL.GetAllLicenseControl());
        }
        public IActionResult List()
        {
            return PartialView("List", _LicenseControlBLL.GetAllLicenseControl());
        }
        [HttpGet]
        public IActionResult Add(string DPID)
        {
            int id = 0;
            if (!string.IsNullOrEmpty(DPID))
            {
                int.TryParse(EncryptDecrypt.Decrypt(DPID), out id);
            }
            return PartialView("Add", BindLicenseControl(id));
        }
        [HttpPost]
        public IActionResult SaveEdit(LicenseControl model)
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
                    if (_LicenseControlBLL.CheckLicenseControlName(model.Id, model.LicenseName))
                    {
                        if (model.Id > 0)
                        {
                            _LicenseControlBLL.UpdateLicenseControl(model);
                        }
                        else
                        {
                            _LicenseControlBLL.AddLicenseControl(model);
                        }
                        jsonResponse.Status = true;
                        jsonResponse.Message = NotificationMessage.SaveSuccessfully;
                        jsonResponse.RedirectURL = Url.Action("Index", "License");
                    }
                    else
                    {
                        TempData["Message"] = "License name already exist";
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
                int id = 0;
                int.TryParse(EncryptDecrypt.Decrypt(DPID), out id);
                _LicenseControlBLL.DeleteLicenseControl(id);
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