using BusinessLogicLayer.Application;
using BusinessLogicLayer.ErrorLog;
using BusinessLogicLayer.GeneralSetup;
using DataAccessLayer.WorkProcess;
using DistributorPortal.Resource;
using Microsoft.AspNetCore.Mvc;
using Models.Application;
using System;
using System.Linq;
using Utility.HelperClasses;

namespace DistributorPortal.Controllers
{
    public class DesignationController : BaseController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly DesignationBLL _DesignationBLL;
        public DesignationController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _DesignationBLL = new DesignationBLL(_unitOfWork);
        }
        // GET: Designation
        public IActionResult Index()
        {
            return View(_DesignationBLL.GetAllDesignation());
        }
        public IActionResult List()
        {
            return PartialView("List", _DesignationBLL.GetAllDesignation());
        }
        [HttpGet]
        public IActionResult Add(string DPID)
        {
            int id;
            int.TryParse(EncryptDecrypt.Decrypt(DPID), out id);
            return PartialView("Add", BindDesignation(id));
        }
        [HttpPost]
        public IActionResult SaveEdit(Designation model)
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
                    if (_DesignationBLL.CheckDesignationName(model.Id, model.DesignationName))
                    {
                        if (model.Id > 0)
                        {
                            _DesignationBLL.UpdateDesignation(model);
                            TempData["Message"] = NotificationMessage.UpdateSuccessfully;
                        }
                        else
                        {
                            _DesignationBLL.AddDesignation(model);
                            TempData["Message"] = NotificationMessage.SaveSuccessfully;
                        }
                    }
                    else
                    {
                        TempData["Message"] = "Designation name already exist";
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
                _DesignationBLL.DeleteDesignation(id);
                return Json(new { Result = true });
            }
            catch (Exception ex)
            {
                new ErrorLogBLL(_unitOfWork).AddExceptionLog(ex);
                TempData["Message"] = NotificationMessage.ErrorOccurred;
                return RedirectToAction("List");
            }
        }
        private Designation BindDesignation(int Id)
        {
            Designation model = new Designation();
            if (Id > 0)
            {
                model = _DesignationBLL.GetDesignationById(Id);
            }
            else
            {
                model.IsActive = true;
            }
            return model;
        }
        public JsonResult GetDesignationList()
        {
            return Json(_DesignationBLL.GetAllDesignation().ToList());
        }
    }
}