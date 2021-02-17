using BusinessLogicLayer.Application;
using BusinessLogicLayer.ErrorLog;
using BusinessLogicLayer.GeneralSetup;
using DataAccessLayer.WorkProcess;
using DistributorPortal.Resource;
using Microsoft.AspNetCore.Mvc;
using Models.Application;
using Models.ViewModel;
using System;
using System.Linq;

namespace DistributorPortal.Controllers
{
    public class ComplaintCategoryController : BaseController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ComplaintCategoryBLL _ComplaintCategoryBLL;
        public ComplaintCategoryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _ComplaintCategoryBLL = new ComplaintCategoryBLL(_unitOfWork);
        }
        // GET: ComplaintCategory
        public IActionResult Index()
        {
            new AuditLogBLL(_unitOfWork).AddAuditLog("ComplaintCategory", "Index", " Form");
            return View(_ComplaintCategoryBLL.GetAllComplaintCategory());
        }
        public IActionResult List()
        {
            return PartialView("List", _ComplaintCategoryBLL.GetAllComplaintCategory());
        }
        [HttpGet]
        public IActionResult Add(int id)
        {
            new AuditLogBLL(_unitOfWork).AddAuditLog("ComplaintCategory", "Add", "Click on Add  Button of ");
            return PartialView("Add", BindComplaintCategory(id));
        }
        [HttpPost]
        public IActionResult SaveEdit(ComplaintCategory model)
        {
            JsonResponse jsonResponse = new JsonResponse();
            try
            {
                new AuditLogBLL(_unitOfWork).AddAuditLog("ComplaintCategory", "SaveEdit", "Start Click on SaveEdit Button of ");
                ModelState.Remove("Id");
                if (!ModelState.IsValid)
                {
                    TempData["Message"] = NotificationMessage.RequiredFieldsValidation;
                    return PartialView("Add", model);
                }
                else
                {
                    if (_ComplaintCategoryBLL.CheckComplaintCategoryName(model.Id, model.ComplaintCategoryName))
                    {
                        if (model.Id > 0)
                        {
                            _ComplaintCategoryBLL.UpdateComplaintCategory(model);
                            jsonResponse.Status = true;
                            jsonResponse.Message = NotificationMessage.UpdateSuccessfully;
                            jsonResponse.RedirectURL = Url.Action("Index", "ComplaintCategory");
                        }
                        else
                        {
                            _ComplaintCategoryBLL.AddComplaintCategory(model);
                            jsonResponse.Status = true;
                            jsonResponse.Message = NotificationMessage.SaveSuccessfully;
                            jsonResponse.RedirectURL = Url.Action("Index", "ComplaintCategory");
                        }
                    }
                    else
                    {
                        new AuditLogBLL(_unitOfWork).AddAuditLog("ComplaintCategory", "SaveEdit", "End Click on Save Button of ");
                        jsonResponse.Status = false;
                        jsonResponse.Message = "Complaint Category name already exist";
                    }
                }
                new AuditLogBLL(_unitOfWork).AddAuditLog("ComplaintCategory", "SaveEdit", "End Click on Save Button of ");
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
        public IActionResult Delete(int id)
        {
            try
            {
                new AuditLogBLL(_unitOfWork).AddAuditLog("ComplaintCategory", "Delete", "Start Click on Delete Button of ");
                _ComplaintCategoryBLL.DeleteComplaintCategory(id);
                new AuditLogBLL(_unitOfWork).AddAuditLog("ComplaintCategory", "Delete", "End Click on Delete Button of ");
                return Json(new { Result = true });
            }
            catch (Exception ex)
            {
                new ErrorLogBLL(_unitOfWork).AddExceptionLog(ex);
                TempData["Message"] = NotificationMessage.ErrorOccurred;
                return RedirectToAction("List");
            }
        }
        private ComplaintCategory BindComplaintCategory(int Id)
        {
            ComplaintCategory model = new ComplaintCategory();
            if (Id > 0)
            {
                model = _ComplaintCategoryBLL.GetComplaintCategoryById(Id);
            }
            else
            {
                model.IsActive = true;
            }
            return model;
        }
        public JsonResult GetComplaintCategoryList()
        {
            return Json(_ComplaintCategoryBLL.GetAllComplaintCategory().ToList());
        }
    }
}