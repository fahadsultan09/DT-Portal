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
    public class PolicyController : BaseController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly PolicyBLL _PolicyBLL;
        public PolicyController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _PolicyBLL = new PolicyBLL(_unitOfWork);
        }
        // GET: Policy
        public IActionResult Index()
        {
            return View(_PolicyBLL.GetAllPolicy());
        }
        public IActionResult List()
        {
            return PartialView("List", _PolicyBLL.GetAllPolicy());
        }
        [HttpGet]
        public IActionResult Add(string DPID)
        {
            int id=0;
            if (!string.IsNullOrEmpty(DPID))
            {
                int.TryParse(EncryptDecrypt.Decrypt(DPID), out id);
            }
            return PartialView("Add", BindPolicy(id));
        }
        [HttpPost]
        public IActionResult SaveEdit(Policy model)
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
                    if (_PolicyBLL.CheckPolicyName(model.Id, model.Title))
                    {
                        if (model.Id > 0)
                        {
                            _PolicyBLL.UpdatePolicy(model);
                            jsonResponse.Status = true;
                            jsonResponse.Message = NotificationMessage.UpdateSuccessfully;
                            jsonResponse.RedirectURL = Url.Action("Index", "Policy");
                        }
                        else
                        {
                            _PolicyBLL.AddPolicy(model);
                            jsonResponse.Status = true;
                            jsonResponse.Message = NotificationMessage.SaveSuccessfully;
                            jsonResponse.RedirectURL = Url.Action("Index", "Policy");
                        }
                    }
                    else
                    {
                        TempData["Message"] = "Policy name already exist";
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
                _PolicyBLL.DeletePolicy(id);
                return Json(new { Result = true });
            }
            catch (Exception ex)
            {
                new ErrorLogBLL(_unitOfWork).AddExceptionLog(ex);
                TempData["Message"] = NotificationMessage.ErrorOccurred;
                return RedirectToAction("List");
            }
        }
        private Policy BindPolicy(int Id)
        {
            Policy model = new Policy();
            if (Id > 0)
            {
                model = _PolicyBLL.GetPolicyById(Id);
            }
            else
            {
                model.IsActive = true;
            }
            return model;
        }
        public JsonResult GetPolicyList()
        {
            return Json(_PolicyBLL.GetAllPolicy().ToList());
        }
    }
}