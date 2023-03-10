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
            return View(_BankBLL.GetAllBank());
        }
        public IActionResult List()
        {
            return PartialView("List", _BankBLL.GetAllBank());
        }
        [HttpGet]
        public IActionResult Add(string DPID)
        {
            int id=0;
            if (!string.IsNullOrEmpty(DPID))
            {
                int.TryParse(EncryptDecrypt.Decrypt(DPID), out id);
            }
            return PartialView("Add", BindBank(id));
        }
        [HttpPost]
        public IActionResult SaveEdit(Bank model)
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
                        TempData["Message"] = "Bank name already exist";
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
                int id=0;
                int.TryParse(EncryptDecrypt.Decrypt(DPID), out id);
                _BankBLL.DeleteBank(id);
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
        public IActionResult DropDownBankList(int id)
        {            
            return Json(_BankBLL.DropDownBankList(id, 0));
        }
        public JsonResult GetBranchCode(int id)
        {            
            return Json(_BankBLL.GetAllBank().Where(x=>x.Id == id).Select(x=>x.BranchCode));
        }

    }
}