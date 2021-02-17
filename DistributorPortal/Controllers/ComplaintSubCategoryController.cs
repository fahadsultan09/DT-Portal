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
using Utility.HelperClasses;

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
            return View(_ComplaintSubCategoryBLL.GetAllComplaintSubCategory());
        }
        public IActionResult List()
        {
            return PartialView("List", _ComplaintSubCategoryBLL.GetAllComplaintSubCategory());
        }
        [HttpGet]
        public IActionResult Add(string DPID)
        {
            int id;
            int.TryParse(EncryptDecrypt.Decrypt(DPID), out id);
            return PartialView("Add", BindComplaintSubCategory(id));
        }
        [HttpPost]
        public IActionResult SaveEdit(ComplaintSubCategory model)
        {
            JsonResponse jsonResponse = new JsonResponse();
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
                        TempData["Message"] = "Complaint Sub Category name already exist";
                        return PartialView("Add", model);
                    }
                }
                return RedirectToAction("List");
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
                int id;
                int.TryParse(EncryptDecrypt.Decrypt(DPID), out id);
                _ComplaintSubCategoryBLL.DeleteComplaintSubCategory(id);
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
        public IActionResult DropDownComplaintSubCategoryList(string DPID)
        {
            int id;
            int.TryParse(EncryptDecrypt.Decrypt(DPID), out id);
            return Json(_ComplaintSubCategoryBLL.DropDownComplaintSubCategoryList(id, 0));
        }
    }
}