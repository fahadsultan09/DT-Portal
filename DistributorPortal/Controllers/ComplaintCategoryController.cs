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
using Utility.HelperClasses;

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
        public IActionResult Add(string DPID)
        {
            int id;
            int.TryParse(EncryptDecrypt.Decrypt(DPID), out id);
            return PartialView("Add", BindComplaintCategory(id));
        }
        [HttpPost]
        public IActionResult SaveEdit(ComplaintCategory model)
        {
            JsonResponse jsonResponse = new JsonResponse();
            try
            {                
                ModelState.Remove("Id");
                if (!ModelState.IsValid)
                {
                    jsonResponse.Status = false;
                    jsonResponse.Message = NotificationMessage.RequiredFieldsValidation;
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
                        jsonResponse.Status = true;
                        jsonResponse.Message = "Complaint Category name already exist";
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
                int id;
                int.TryParse(EncryptDecrypt.Decrypt(DPID), out id);
                _ComplaintCategoryBLL.DeleteComplaintCategory(id);
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