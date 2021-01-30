using BusinessLogicLayer.ErrorLog;
using BusinessLogicLayer.GeneralSetup;
using DataAccessLayer.WorkProcess;
using DistributorPortal.BusinessLogicLayer.ApplicationSetup;
using DistributorPortal.Resource;
using Microsoft.AspNetCore.Mvc;
using Models.Application;
using Models.UserRights;
using Models.ViewModel;
using System;
using System.Linq;

namespace DistributorPortal.Controllers
{
    public class BankController : BaseController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly BankBLL _BankBLL;
        public BankController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _BankBLL = new BankBLL(_unitOfWork);
        }
        // GET: Bank
        public IActionResult Index()
        {
            new AuditTrailBLL(_unitOfWork).AddAuditTrail("Bank", "Index", " Form");
            return View(_BankBLL.GetAllBank());
        }
        public IActionResult List()
        {
            return PartialView("List", _BankBLL.GetAllBank());
        }
        [HttpGet]
        public IActionResult Add(int id)
        {
            new AuditTrailBLL(_unitOfWork).AddAuditTrail("Bank", "Add", "Click on Add  Button of ");
            return PartialView("Add", BindBank(id));
        }
        [HttpPost]
        public IActionResult SaveEdit(Bank model)
        {
            JsonResponse jsonResponse = new JsonResponse();
            try
            {
                new AuditTrailBLL(_unitOfWork).AddAuditTrail("Bank", "SaveEdit", "Start Click on SaveEdit Button of ");
                ModelState.Remove("Id");
                if (!ModelState.IsValid)
                {
                    jsonResponse.Status = false;
                    jsonResponse.Message = NotificationMessage.RequiredFieldsValidation;
                }
                else
                {
                    if (_BankBLL.CheckBankName(model.Id, model.BankName, model.CompanyId))
                    {
                        if (model.Id > 0)
                        {
                            _BankBLL.UpdateBank(model);
                            jsonResponse.Status = true;
                            jsonResponse.Message = NotificationMessage.UpdateSuccessfully;
                            jsonResponse.RedirectURL = Url.Action("Index", "Bank");
                        }
                        else
                        {
                            _BankBLL.AddBank(model);
                            jsonResponse.Status = true;
                            jsonResponse.Message = NotificationMessage.SaveSuccessfully;
                            jsonResponse.RedirectURL = Url.Action("Index", "Bank");
                        }
                    }
                    else
                    {
                        jsonResponse.Status = false;
                        jsonResponse.Message = "Bank name already exist";
                    }
                }
                new AuditTrailBLL(_unitOfWork).AddAuditTrail("Bank", "SaveEdit", "End Click on Save Button of ");
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
                new AuditTrailBLL(_unitOfWork).AddAuditTrail("Bank", "Delete", "Start Click on Delete Button of ");
                _BankBLL.DeleteBank(id);
                new AuditTrailBLL(_unitOfWork).AddAuditTrail("Bank", "Delete", "End Click on Delete Button of ");
                return Json(new { Result = true });
            }
            catch (Exception ex)
            {
                new ErrorLogBLL(_unitOfWork).AddExceptionLog(ex);
                TempData["Message"] = NotificationMessage.ErrorOccurred;
                return RedirectToAction("List");
            }
        }
        private Bank BindBank(int Id)
        {
            Bank model = new Bank();
            if (Id > 0)
            {
                model = _BankBLL.GetBankById(Id);
            }
            else
            {
                model.IsActive = true;
            }
            model.CompanyList = new CompanyBLL(_unitOfWork).DropDownCompanyList(model.CompanyId, true);
            return model;
        }
        public JsonResult GetBankList()
        {
            return Json(_BankBLL.GetAllBank().ToList());
        }
        public IActionResult DropDownBankList(int ComplaintCategoryId)
        {
            return Json(_BankBLL.DropDownBankList(ComplaintCategoryId, 0));
        }
        public JsonResult GetBranchCode(int Id)
        {
            return Json(_BankBLL.GetAllBank().Where(x=>x.Id == Id).Select(x=>x.BranchCode));
        }

    }
}