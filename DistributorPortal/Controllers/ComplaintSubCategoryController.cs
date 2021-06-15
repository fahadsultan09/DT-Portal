using BusinessLogicLayer.Application;
using BusinessLogicLayer.ErrorLog;
using BusinessLogicLayer.GeneralSetup;
using DataAccessLayer.WorkProcess;
using DistributorPortal.Resource;
using Microsoft.AspNetCore.Mvc;
using Models.Application;
using Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using Utility;
using Utility.HelperClasses;

namespace DistributorPortal.Controllers
{
    public class ComplaintSubCategoryController : BaseController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ComplaintSubCategoryBLL _ComplaintSubCategoryBLL;
        private readonly ComplaintUserEmailBLL _ComplaintUserEmailBLL;
        private readonly UserBLL _UserBLL;
        public ComplaintSubCategoryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _ComplaintSubCategoryBLL = new ComplaintSubCategoryBLL(_unitOfWork);
            _ComplaintUserEmailBLL = new ComplaintUserEmailBLL(_unitOfWork);
            _UserBLL = new UserBLL(_unitOfWork);
        }
        // GET: ComplaintSubCategory
        public IActionResult Index()
        {
            return View(_ComplaintSubCategoryBLL.GetAllComplaintSubCategory());
        }
        public List<ComplaintSubCategory> List()
        {
            return _ComplaintSubCategoryBLL.GetAllComplaintSubCategory();
        }
        [HttpGet]
        public IActionResult Add(string DPID)
        {
            int id = 0;
            if (!string.IsNullOrEmpty(DPID))
            {
                int.TryParse(EncryptDecrypt.Decrypt(DPID), out id);
            }
            return View("Add", BindComplaintSubCategory(id));
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
                            var UserEmailCC = _ComplaintUserEmailBLL.Where(e => e.ComplaintSubCategoryId == model.Id).ToList();
                            UserEmailCC.ForEach(x => x.ComplaintSubCategory = null);
                            _ComplaintUserEmailBLL.DeleteRange(UserEmailCC);

                            var UserEmailKPI = _ComplaintUserEmailBLL.Where(e => e.ComplaintSubCategoryId == model.Id).ToList();
                            UserEmailKPI.ForEach(x => x.ComplaintSubCategory = null);
                            _ComplaintUserEmailBLL.DeleteRange(UserEmailKPI);

                            _ComplaintSubCategoryBLL.UpdateComplaintSubCategory(model);
                        }
                        else
                        {
                            _ComplaintSubCategoryBLL.AddComplaintSubCategory(model);
                        }

                        if (model.UserEmailCC != null)
                        {
                            foreach (var item in model.UserEmailCC)
                            {
                                if (item != null)
                                {
                                    foreach (var email in item.Split(','))
                                    {

                                        ComplaintUserEmail complaintUserEmail = new ComplaintUserEmail()
                                        {
                                            ComplaintSubCategoryId = model.Id,
                                            EmailType = EmailType.CC,
                                            UserEmailId = email,
                                        };
                                        _ComplaintUserEmailBLL.Add(complaintUserEmail);
                                    }
                                }
                            }
                        }
                        if (model.UserEmailKPI != null)
                        {
                            foreach (var item in model.UserEmailKPI)
                            {
                                if (item != null)
                                {
                                    foreach (var email in item.Split(','))
                                    {

                                        ComplaintUserEmail complaintUserEmail = new ComplaintUserEmail()
                                        {
                                            ComplaintSubCategoryId = model.Id,
                                            EmailType = EmailType.KPI,
                                            UserEmailId = email,
                                        };
                                        _ComplaintUserEmailBLL.Add(complaintUserEmail);
                                    }
                                }
                            }
                        }
                        jsonResponse.Status = true;
                        jsonResponse.Message = NotificationMessage.SaveSuccessfully;
                        jsonResponse.RedirectURL = Url.Action("Index", "ComplaintSubCategory");
                    }
                    else
                    {
                        jsonResponse.Status = false;
                        jsonResponse.Message = "Complaint sub category name already exist";
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
            model.UserList = _UserBLL.DropDownUserList(model.UserEmailTo);
            model.UserEmailCC = _ComplaintUserEmailBLL.GetAllAComplaintUserEmailByComplaintSubCategoryId(model.Id, EmailType.CC).ToArray();
            model.UserEmailKPI = _ComplaintUserEmailBLL.GetAllAComplaintUserEmailByComplaintSubCategoryId(model.Id, EmailType.KPI).ToArray();
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