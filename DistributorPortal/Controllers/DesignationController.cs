using BusinessLogicLayer.ErrorLog;
using BusinessLogicLayer.GeneralSetup;
using DataAccessLayer.WorkProcess;
using DistributorPortal.BusinessLogicLayer.ApplicationSetup;
using DistributorPortal.Resource;
using Microsoft.AspNetCore.Mvc;
using Models.Application;
using System;
using System.Linq;

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
            new AuditTrailBLL(_unitOfWork).AddAuditTrail("ComplaintSubCategory", "Index", " Form");
            return View(_DesignationBLL.GetAllDesignation());
        }
        public IActionResult List()
        {
            return PartialView("List", _DesignationBLL.GetAllDesignation());
        }
        [HttpGet]
        public IActionResult Add(int id)
        {
            new AuditTrailBLL(_unitOfWork).AddAuditTrail("ComplaintSubCategory", "Add", "Click on Add  Button of ");
            return PartialView("Add", BindDesignation(id));
        }
        [HttpPost]
        public IActionResult SaveEdit(Designation model)
        {
            try
            {
                new AuditTrailBLL(_unitOfWork).AddAuditTrail("ComplaintSubCategory", "SaveEdit", "Start Click on SaveEdit Button of ");
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
                        new AuditTrailBLL(_unitOfWork).AddAuditTrail("ComplaintSubCategory", "SaveEdit", "End Click on Save Button of ");
                        TempData["Message"] = "Designation name already exist";
                        return PartialView("Add", model);
                    }
                }
                new AuditTrailBLL(_unitOfWork).AddAuditTrail("ComplaintSubCategory", "SaveEdit", "End Click on Save Button of ");
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
                new AuditTrailBLL(_unitOfWork).AddAuditTrail("ComplaintSubCategory", "Delete", "Start Click on Delete Button of ");
                _DesignationBLL.DeleteDesignation(id);
                new AuditTrailBLL(_unitOfWork).AddAuditTrail("ComplaintSubCategory", "Delete", "End Click on Delete Button of ");
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