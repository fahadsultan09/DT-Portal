using BusinessLogicLayer.Application;
using BusinessLogicLayer.ApplicationSetup;
using BusinessLogicLayer.ErrorLog;
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
    public class UserSystemInfoController : BaseController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserSystemInfoBLL _UserSystemInfoBLL;
        public UserSystemInfoController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _UserSystemInfoBLL = new UserSystemInfoBLL(_unitOfWork);
        }
        public IActionResult Index()
        {
            return View(_UserSystemInfoBLL.GetAllUserSystemInfo().ToList());
        }
        public IActionResult List()
        {
            return PartialView("List", _UserSystemInfoBLL.GetAllUserSystemInfo().ToList());
        }
        [HttpGet]
        public IActionResult Add(string DPID)
        {
            int id = 0;
            if (!string.IsNullOrEmpty(DPID))
            {
                int.TryParse(EncryptDecrypt.Decrypt(DPID), out id);
            }
            return PartialView("Add", BindUserSystemInfo(id));
        }
        [HttpPost]
        public IActionResult SaveEdit(UserSystemInfo model)
        {
            try
            {
                JsonResponse jsonResponse = new JsonResponse();
                ModelState.Remove("Id");
                if (!ModelState.IsValid)
                {
                    TempData["Message"] = NotificationMessage.RequiredFieldsValidation;
                    return PartialView("Add", model);
                }
                else
                {
                    if (_UserSystemInfoBLL.Check(model.Id, model.MACAddress))
                    {
                        if (model.Id > 0)
                        {
                            _UserSystemInfoBLL.Update(model);
                        }
                        else
                        {
                            _UserSystemInfoBLL.Add(model);
                        }
                        jsonResponse.Status = true;
                        jsonResponse.Message = NotificationMessage.SaveSuccessfully;
                        jsonResponse.RedirectURL = Url.Action("Index", "UserSystemInfo");
                    }
                    else
                    {
                        jsonResponse.Status = false;
                        jsonResponse.Message = "System Info name already exist";
                    }
                }
                return Json(new { data = jsonResponse });
            }
            catch (Exception ex)
            {
                new ErrorLogBLL(_unitOfWork).AddExceptionLog(ex);
                TempData["Message"] = NotificationMessage.ErrorOccurred;
                return RedirectToAction("List");
            }
        }
        [HttpPost]
        public IActionResult Delete(string DPID)
        {
            try
            {
                int id = 0;
                int.TryParse(EncryptDecrypt.Decrypt(DPID), out id);
                _UserSystemInfoBLL.Delete(id);
                return Json(new { Result = true });
            }
            catch (Exception ex)
            {
                new ErrorLogBLL(_unitOfWork).AddExceptionLog(ex);
                TempData["Message"] = NotificationMessage.ErrorOccurred;
                return RedirectToAction("List");
            }
        }
        private UserSystemInfo BindUserSystemInfo(int Id)
        {
            UserSystemInfo model = new UserSystemInfo();
            if (Id > 0)
            {
                model = _UserSystemInfoBLL.GetById(Id);
            }
            else
            {
                model.IsActive = true;
            }
            model.DistributorList = new DistributorBLL(_unitOfWork).DropDownDistributorList(Convert.ToInt32(model.DistributorId));
            return model;
        }
    }
}