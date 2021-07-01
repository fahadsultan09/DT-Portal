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
            return View(_CompanyBLL.GetAllCompany());
        }
        public IActionResult List()
        {
            return PartialView("List", _CompanyBLL.GetAllCompany());
        }
        [HttpGet]
        public IActionResult Add(string DPID)
        {
            int id=0;
            if (!string.IsNullOrEmpty(DPID))
            {
                int.TryParse(EncryptDecrypt.Decrypt(DPID), out id);
            }
            return PartialView("Add", BindCompany(id));
        }
        [HttpPost]
        public IActionResult SaveEdit(Company model)
        {
            JsonResponse jsonResponse = new JsonResponse();
            try
            {
                TempData["Message"] = string.Empty;
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
                            jsonResponse.Status = true;
                            jsonResponse.Message = NotificationMessage.UpdateSuccessfully;
                            jsonResponse.RedirectURL = Url.Action("Index", "Company");
                        }
                        else
                        {
                            _CompanyBLL.AddCompany(model);
                            jsonResponse.Status = true;
                            jsonResponse.Message = NotificationMessage.SaveSuccessfully;
                            jsonResponse.RedirectURL = Url.Action("Index", "Company");
                        }
                    }
                    else
                    {
                        TempData["Message"] = "Company name already exist";
                        return PartialView("Add", model);
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
                int.TryParse(EncryptDecrypt.Decrypt(DPID), out int id);
                _CompanyBLL.DeleteCompany(id);
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