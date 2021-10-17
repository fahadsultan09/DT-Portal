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
    public class PaymentSectionController : BaseController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly PaymentSectionBLL _PaymentSectionBLL;
        public PaymentSectionController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _PaymentSectionBLL = new PaymentSectionBLL(_unitOfWork);
        }
        public IActionResult Index()
        {
            return View(_PaymentSectionBLL.GetAllPaymentSection());
        }
        public IActionResult List()
        {
            return PartialView("List", _PaymentSectionBLL.GetAllPaymentSection());
        }
        [HttpGet]
        public IActionResult Add(string DPID)
        {
            int id = 0;
            if (!string.IsNullOrEmpty(DPID))
            {
                int.TryParse(EncryptDecrypt.Decrypt(DPID), out id);
            }
            return PartialView("Add", BindPaymentSection(id));
        }
        [HttpPost]
        public IActionResult SaveEdit(PaymentSection model)
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
                    if (_PaymentSectionBLL.CheckPaymentSectionName(model.Id, model.FormNo, model.CompanyId))
                    {
                        if (model.Id > 0)
                        {
                            _PaymentSectionBLL.UpdatePaymentSection(model);
                            jsonResponse.Status = true;
                            jsonResponse.Message = NotificationMessage.UpdateSuccessfully;
                            jsonResponse.RedirectURL = Url.Action("Index", "PaymentSection");
                        }
                        else
                        {
                            _PaymentSectionBLL.AddPaymentSection(model);
                            jsonResponse.Status = true;
                            jsonResponse.Message = NotificationMessage.SaveSuccessfully;
                            jsonResponse.RedirectURL = Url.Action("Index", "PaymentSection");
                        }
                    }
                    else
                    {
                        TempData["Message"] = "PaymentSection name already exist";
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
                int id = 0;
                int.TryParse(EncryptDecrypt.Decrypt(DPID), out id);
                _PaymentSectionBLL.DeletePaymentSection(id);
                return Json(new { Result = true });
            }
            catch (Exception ex)
            {
                new ErrorLogBLL(_unitOfWork).AddExceptionLog(ex);
                TempData["Message"] = NotificationMessage.ErrorOccurred;
                return RedirectToAction("List");
            }
        }
        private PaymentSection BindPaymentSection(int Id)
        {
            PaymentSection model = new PaymentSection();
            if (Id > 0)
            {
                model = _PaymentSectionBLL.GetPaymentSectionById(Id);
            }
            else
            {
                model.IsActive = true;
            }
            model.CompanyList = new CompanyBLL(_unitOfWork).DropDownCompanyList(model.CompanyId, true);
            return model;
        }
        public JsonResult GetPaymentSectionList()
        {
            return Json(_PaymentSectionBLL.GetAllPaymentSection().ToList());
        }
        public IActionResult DropDownPaymentSectionList(int id)
        {
            return Json(_PaymentSectionBLL.DropDownPaymentSectionList(id, 0));
        }
        public JsonResult GetBranchCode(int id)
        {
            return Json(_PaymentSectionBLL.GetAllPaymentSection().Where(x => x.Id == id).Select(x => x.FormNo));
        }

    }
}