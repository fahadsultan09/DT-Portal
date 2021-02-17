using BusinessLogicLayer.Application;
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
    public class CompanyController : BaseController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly CompanyBLL _CompanyBLL;
        public CompanyController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _CompanyBLL = new CompanyBLL(_unitOfWork);
        }
        // GET: Company
        public IActionResult Index()
        {
            new AuditLogBLL(_unitOfWork).AddAuditLog("Company", "Index", " Form");
            return View(_CompanyBLL.GetAllCompany());
        }
        public IActionResult List()
        {
            return PartialView("List", _CompanyBLL.GetAllCompany());
        }
        [HttpGet]
        public IActionResult Add(int id)
        {
            new AuditLogBLL(_unitOfWork).AddAuditLog("Company", "Add", "Click on Add  Button of ");
            return PartialView("Add", BindCompany(id));
        }
        [HttpPost]
        public IActionResult SaveEdit(Company model)
        {
            try
            {
                new AuditLogBLL(_unitOfWork).AddAuditLog("Company", "SaveEdit", "Start Click on SaveEdit Button of ");
                ModelState.Remove("Id");
                if (!ModelState.IsValid)
                {
                    TempData["Message"] = NotificationMessage.RequiredFieldsValidation;
                    return PartialView("Add", model);
                }
                else
                {
                    if (_CompanyBLL.CheckCompanyName(model.Id, model.CompanyName))
                    {
                        if (model.Id > 0)
                        {
                            _CompanyBLL.UpdateCompany(model);
                            TempData["Message"] = NotificationMessage.UpdateSuccessfully;
                        }
                        else
                        {
                            _CompanyBLL.AddCompany(model);
                            TempData["Message"] = NotificationMessage.SaveSuccessfully;
                        }
                    }
                    else
                    {
                        new AuditLogBLL(_unitOfWork).AddAuditLog("Company", "SaveEdit", "End Click on Save Button of ");
                        TempData["Message"] = "Application Module name already exist";
                        return PartialView("Add", model);
                    }
                }
                new AuditLogBLL(_unitOfWork).AddAuditLog("Company", "SaveEdit", "End Click on Save Button of ");
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
                new AuditLogBLL(_unitOfWork).AddAuditLog("Company", "Delete", "Start Click on Delete Button of ");
                _CompanyBLL.DeleteCompany(id);
                new AuditLogBLL(_unitOfWork).AddAuditLog("Company", "Delete", "End Click on Delete Button of ");
                return Json(new { Result = true });
            }
            catch (Exception ex)
            {
                new ErrorLogBLL(_unitOfWork).AddExceptionLog(ex);
                TempData["Message"] = NotificationMessage.ErrorOccurred;
                return RedirectToAction("List");
            }
        }
        private Company BindCompany(int Id)
        {
            Company model = new Company();
            if (Id > 0)
            {
                model = _CompanyBLL.GetCompanyById(Id);
            }
            else
            {
                model.IsActive = true;
            }
            return model;
        }
        public JsonResult GetCompanyList()
        {
            return Json(_CompanyBLL.GetAllCompany().ToList());
        }
    }
}