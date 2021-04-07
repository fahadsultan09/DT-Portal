using BusinessLogicLayer.Application;
using BusinessLogicLayer.ApplicationSetup;
using BusinessLogicLayer.ErrorLog;
using BusinessLogicLayer.HelperClasses;
using DataAccessLayer.WorkProcess;
using DistributorPortal.Resource;
using Microsoft.AspNetCore.Mvc;
using Models.Application;
using Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using Utility.HelperClasses;

namespace DistributorPortal.Controllers
{
    public class UserSystemInfoController : BaseController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserSystemInfoBLL _UserSystemInfoBLL;
        private readonly UserSystemInfoDetailBLL _UserSystemInfoDetailBLL;
        public UserSystemInfoController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _UserSystemInfoBLL = new UserSystemInfoBLL(_unitOfWork);
            _UserSystemInfoDetailBLL = new UserSystemInfoDetailBLL(_unitOfWork);
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
                    if (model.Id > 0)
                    {
                        List<UserSystemInfoDetail> UserSystemInfoDetailList = _UserSystemInfoDetailBLL.Where(x => x.UserSystemInfoId == model.Id).ToList();
                        _unitOfWork.GenericRepository<UserSystemInfoDetail>().DeleteRange(UserSystemInfoDetailList);

                        _UserSystemInfoBLL.Update(model);
                    }
                    else
                    {
                        _UserSystemInfoBLL.Add(model);
                    }
                    if (model.UserSystemInfoDetail != null)
                    {
                        foreach (var item in model.UserSystemInfoDetail)
                        {
                            UserSystemInfoDetail userSystemInfoDetail = new UserSystemInfoDetail()
                            {
                                UserSystemInfoId = model.Id,
                                MACAddress = item.MACAddress,
                                CreatedBy = SessionHelper.LoginUser.Id,
                                CreatedDate = DateTime.Now,
                            };
                            _UserSystemInfoDetailBLL.Add(userSystemInfoDetail);
                        }
                    }
                    jsonResponse.Status = true;
                    jsonResponse.Message = NotificationMessage.SaveSuccessfully;
                    jsonResponse.RedirectURL = Url.Action("Index", "UserSystemInfo");
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
                model.UserSystemInfoDetail = _UserSystemInfoDetailBLL.Where(x => x.UserSystemInfoId == model.Id).ToList();
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