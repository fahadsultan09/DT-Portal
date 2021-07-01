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
    public class DisclaimerController : BaseController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly DisclaimerBLL _DisclaimerBLL;
        public DisclaimerController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _DisclaimerBLL = new DisclaimerBLL(_unitOfWork);
        }
        // GET: Disclaimer
        public IActionResult Index()
        {
            return View(_DisclaimerBLL.GetAllDisclaimer());
        }
        public IActionResult List()
        {
            return PartialView("List", _DisclaimerBLL.GetAllDisclaimer());
        }
        [HttpGet]
        public IActionResult Add(string DPID)
        {
            int id = 0;
            if (!string.IsNullOrEmpty(DPID))
            {
                int.TryParse(EncryptDecrypt.Decrypt(DPID), out id);
            }
            return PartialView("Add", BindDisclaimer(id));
        }
        [HttpPost]
        public IActionResult SaveEdit(Disclaimer model)
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
                    if (_DisclaimerBLL.CheckDisclaimerName(model.Id, model.Name))
                    {
                        if (model.Id > 0)
                        {
                            _DisclaimerBLL.UpdateDisclaimer(model);
                            jsonResponse.Status = true;
                            jsonResponse.Message = NotificationMessage.UpdateSuccessfully;
                            jsonResponse.RedirectURL = Url.Action("Index", "Disclaimer");
                        }
                        else
                        {
                            _DisclaimerBLL.AddDisclaimer(model);
                            jsonResponse.Status = true;
                            jsonResponse.Message = NotificationMessage.SaveSuccessfully;
                            jsonResponse.RedirectURL = Url.Action("Index", "Disclaimer");
                        }
                    }
                    else
                    {
                        TempData["Message"] = "Disclaimer name already exist";
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
                _DisclaimerBLL.DeleteDisclaimer(id);
                return Json(new { Result = true });
            }
            catch (Exception ex)
            {
                new ErrorLogBLL(_unitOfWork).AddExceptionLog(ex);
                TempData["Message"] = NotificationMessage.ErrorOccurred;
                return RedirectToAction("List");
            }
        }
        private Disclaimer BindDisclaimer(int Id)
        {
            Disclaimer model = new Disclaimer();
            if (Id > 0)
            {
                model = _DisclaimerBLL.GetDisclaimerById(Id);
            }
            else
            {
                model.IsActive = true;
            }
            return model;
        }
        public JsonResult GetDisclaimerList()
        {
            return Json(_DisclaimerBLL.GetAllDisclaimer().ToList());
        }
    }
}