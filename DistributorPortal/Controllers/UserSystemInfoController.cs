using BusinessLogicLayer.Application;
using BusinessLogicLayer.ApplicationSetup;
using BusinessLogicLayer.ErrorLog;
using DataAccessLayer.WorkProcess;
using DistributorPortal.Resource;
using Microsoft.AspNetCore.Mvc;
using Models.Application;
using Models.ViewModel;
using MoreLinq;
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
        public UserSystemInfoController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _UserSystemInfoBLL = new UserSystemInfoBLL(_unitOfWork);
        }
        public IActionResult Index()
        {
            List<UserSystemInfo> list = _UserSystemInfoBLL.GetAllUserSystemInfo().Where(x => !x.IsDeleted).Select(x => new UserSystemInfo { Id = x.Id, Distributor = x.Distributor, ProcessorId = x.ProcessorId, HostName = x.HostName, IsActive = x.IsActive }).DistinctBy(y => y.HostName).ToList();
            return View(list);
        }
        public IActionResult List()
        {
            List<UserSystemInfo> list = _UserSystemInfoBLL.GetAllUserSystemInfo().Where(x => !x.IsDeleted).Select(x => new UserSystemInfo { Id = x.Id, Distributor = x.Distributor, ProcessorId = x.ProcessorId, HostName = x.HostName, IsActive = x.IsActive }).DistinctBy(y => y.HostName).ToList();
            return PartialView("List", list);
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
                    if (model.UserSystemInfoDetail != null)
                    {
                        if (model.UserSystemInfoDetail.Where(x => x.IsRowDeleted == false).Count() > 0)
                        {
                            foreach (var item in model.UserSystemInfoDetail)
                            {
                                UserSystemInfo userSystemInfo = _UserSystemInfoBLL.Where(x => x.Id == item.Id).FirstOrDefault();
                                if (userSystemInfo != null)
                                {
                                    _UserSystemInfoBLL.Delete(userSystemInfo.Id);
                                    _unitOfWork.Save();
                                }
                            }
                            foreach (var item in model.UserSystemInfoDetail.Where(x => x.IsRowDeleted == false))
                            {
                                if (_UserSystemInfoBLL.CheckMACAddress(model.Id, item.MACAddress))
                                {
                                    model.MACAddress = item.IsRowDeleted ? string.Empty : item.MACAddress;
                                    _UserSystemInfoBLL.Add(model);
                                }
                                else
                                {
                                    model.DistributorList = new DistributorBLL(_unitOfWork).DropDownDistributorList(Convert.ToInt32(model.DistributorId));
                                    TempData["Message"] = "MAC address already exists.";
                                    return PartialView("Add", model);
                                }
                            }
                            jsonResponse.Status = true;
                            jsonResponse.Message = NotificationMessage.SaveSuccessfully;
                            jsonResponse.RedirectURL = Url.Action("Index", "UserSystemInfo");
                            return Json(new { data = jsonResponse });

                        }
                        model.DistributorList = new DistributorBLL(_unitOfWork).DropDownDistributorList(Convert.ToInt32(model.DistributorId));
                        TempData["Message"] = "Add at least one MAC address";
                        return PartialView("Add", model);
                    }
                    else
                    {
                        model.DistributorList = new DistributorBLL(_unitOfWork).DropDownDistributorList(Convert.ToInt32(model.DistributorId));
                        TempData["Message"] = "Add at least one MAC address";
                        return PartialView("Add", model);
                    }
                }
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
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

                UserSystemInfo userSystemInfo = _UserSystemInfoBLL.Where(x => x.Id == id).FirstOrDefault();
                List<UserSystemInfo> UserSystemInfoList = _UserSystemInfoBLL.Where(x => !x.IsDeleted && x.DistributorId == userSystemInfo.DistributorId && x.ProcessorId == userSystemInfo.ProcessorId && x.HostName == userSystemInfo.HostName).ToList();
                foreach (var item in UserSystemInfoList)
                {
                    _UserSystemInfoBLL.Delete(item.Id);
                    _unitOfWork.Save();
                }
                _unitOfWork.Save();
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
                model.UserSystemInfoDetail = _UserSystemInfoBLL.Where(x => x.DistributorId == model.DistributorId && x.HostName == model.HostName).Select(x => new UserSystemInfoViewModel { Id = x.Id, MACAddress = x.MACAddress, IsRowDeleted = x.IsDeleted }).ToList();
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