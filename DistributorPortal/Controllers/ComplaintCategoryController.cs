using BusinessLogicLayer.ErrorLog;
using BusinessLogicLayer.GeneralSetup;
using DataAccessLayer.WorkProcess;
using DistributorPortal.Resource;
using Microsoft.AspNetCore.Mvc;
using Models.Application;
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
            return View(_ComplaintCategoryBLL.GetAllComplaintCategory());
        }

        public IActionResult List()
        {
            return PartialView("List", _ComplaintCategoryBLL.GetAllComplaintCategory());
        }

        [HttpGet]
        public IActionResult Add(int id)
        {
            return PartialView("Add", BindComplaintCategory(id));
        }

        [HttpPost]
        public IActionResult SaveEdit(ComplaintCategory model)
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
                    if (_ComplaintCategoryBLL.CheckComplaintCategoryName(model.Id, model.ComplaintCategoryName))
                    {
                        if (model.Id > 0)
                        {
                            _ComplaintCategoryBLL.UpdateComplaintCategory(model);
                            TempData["Message"] = NotificationMessage.UpdateSuccessfully;
                        }
                        else
                        {
                            _ComplaintCategoryBLL.AddComplaintCategory(model);
                            TempData["Message"] = NotificationMessage.SaveSuccessfully;
                        }
                    }
                    else
                    {
                        TempData["Message"] = "Complaint Category name already exist";
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
        public IActionResult Delete(int id)
        {
            try
            {
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