﻿using BusinessLogicLayer.Application;
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
            new AuditLogBLL(_unitOfWork).AddAuditLog("ComplaintSubCategory", "Index", " Form");
            return View(_ComplaintSubCategoryBLL.GetAllComplaintSubCategory());
        }
        public IActionResult List()
        {
            return PartialView("List", _ComplaintSubCategoryBLL.GetAllComplaintSubCategory());
        }
        [HttpGet]
        public IActionResult Add(int id)
        {
            new AuditLogBLL(_unitOfWork).AddAuditLog("ComplaintSubCategory", "Add", "Click on Add  Button of ");
            return PartialView("Add", BindComplaintSubCategory(id));
        }
        [HttpPost]
        public IActionResult SaveEdit(ComplaintSubCategory model)
        {
            JsonResponse jsonResponse = new JsonResponse();
            try
            {
                new AuditLogBLL(_unitOfWork).AddAuditLog("ComplaintSubCategory", "SaveEdit", "Start Click on SaveEdit Button of ");
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
                            jsonResponse.Status = true;
                            jsonResponse.Message = NotificationMessage.UpdateSuccessfully;
                            jsonResponse.RedirectURL = Url.Action("Index", "ComplaintSubCategory");
                        }
                        else
                        {
                            _ComplaintSubCategoryBLL.AddComplaintSubCategory(model);
                            jsonResponse.Status = true;
                            jsonResponse.Message = NotificationMessage.SaveSuccessfully;
                            jsonResponse.RedirectURL = Url.Action("Index", "ComplaintSubCategory");
                        }
                    }
                    else
                    {
                        new AuditLogBLL(_unitOfWork).AddAuditLog("ComplaintSubCategory", "SaveEdit", "End Click on Save Button of ");
                        jsonResponse.Status = false;
                        jsonResponse.Message = "Complaint Sub Category name already exist";
                    }
                }
                new AuditLogBLL(_unitOfWork).AddAuditLog("ComplaintSubCategory", "SaveEdit", "End Click on Save Button of ");
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
                new AuditLogBLL(_unitOfWork).AddAuditLog("ComplaintSubCategory", "Delete", "Start Click on Delete Button of ");
                _ComplaintSubCategoryBLL.DeleteComplaintSubCategory(id);
                new AuditLogBLL(_unitOfWork).AddAuditLog("ComplaintSubCategory", "Delete", "End Click on Delete Button of ");
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
        public IActionResult DropDownComplaintSubCategoryList(int ComplaintCategoryId)
        {
            return Json(_ComplaintSubCategoryBLL.DropDownComplaintSubCategoryList(ComplaintCategoryId, 0));
        }
    }
}