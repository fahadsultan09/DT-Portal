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
    public class ComplaintSubCategoryController : BaseController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ComplaintSubCategoryBLL _ComplaintSubCategoryBLL;
        public ComplaintSubCategoryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _ComplaintSubCategoryBLL = new ComplaintSubCategoryBLL(_unitOfWork);
        }
        // GET: ComplaintSubCategory
        public IActionResult Index()
        {
            new AuditTrailBLL(_unitOfWork).AddAuditTrail("ComplaintSubCategory", "Index", " Form");
            return View(_ComplaintSubCategoryBLL.GetAllComplaintSubCategory());
        }
        public IActionResult List()
        {
            return PartialView("List", _ComplaintSubCategoryBLL.GetAllComplaintSubCategory());
        }
        [HttpGet]
        public IActionResult Add(int id)
        {
            new AuditTrailBLL(_unitOfWork).AddAuditTrail("ComplaintSubCategory", "Add", "Click on Add  Button of ");
            return PartialView("Add", BindComplaintSubCategory(id));
        }
        [HttpPost]
        public IActionResult SaveEdit(ComplaintSubCategory model)
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
                    if (_ComplaintSubCategoryBLL.CheckComplaintSubCategoryName(model.Id, model.ComplaintSubCategoryName))
                    {
                        if (model.Id > 0)
                        {
                            _ComplaintSubCategoryBLL.UpdateComplaintSubCategory(model);
                            TempData["Message"] = NotificationMessage.UpdateSuccessfully;
                        }
                        else
                        {
                            _ComplaintSubCategoryBLL.AddComplaintSubCategory(model);
                            TempData["Message"] = NotificationMessage.SaveSuccessfully;
                        }
                    }
                    else
                    {
                        new AuditTrailBLL(_unitOfWork).AddAuditTrail("ComplaintSubCategory", "SaveEdit", "End Click on Save Button of ");
                        TempData["Message"] = "Complaint Sub Category name already exist";
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
                _ComplaintSubCategoryBLL.DeleteComplaintSubCategory(id);
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
        private ComplaintSubCategory BindComplaintSubCategory(int Id)
        {
            ComplaintSubCategory model = new ComplaintSubCategory();
            if (Id > 0)
            {
                model = _ComplaintSubCategoryBLL.GetComplaintSubCategoryById(Id);
            }
            else
            {
                model.IsActive = true;
            }
            model.ComplaintCategoryList = new ComplaintCategoryBLL(_unitOfWork).DropDownComplaintCategoryList(model.ComplaintCategoryId);
            return model;
        }
        public JsonResult GetComplaintSubCategoryList()
        {
            return Json(_ComplaintSubCategoryBLL.GetAllComplaintSubCategory().ToList());
        }
    }
}