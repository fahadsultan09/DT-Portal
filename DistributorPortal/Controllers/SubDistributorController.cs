using BusinessLogicLayer.Application;
using BusinessLogicLayer.ApplicationSetup;
using BusinessLogicLayer.ErrorLog;
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
    public class SubDistributorController : BaseController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly SubDistributorBLL _SubDistributorBLL;
        public SubDistributorController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _SubDistributorBLL = new SubDistributorBLL(_unitOfWork);
        }
        public IActionResult Index()
        {
            List<SubDistributor> list = _SubDistributorBLL.GetAllSubDistributor().Where(x => !x.IsDeleted).ToList();
            return View(list);
        }
        public IActionResult List()
        {
            List<SubDistributor> list = _SubDistributorBLL.GetAllSubDistributor().Where(x => !x.IsDeleted).ToList();
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
            return PartialView("Add", BindSubDistributor(id));
        }
        [HttpPost]
        public IActionResult SaveEdit(SubDistributor model)
        {
            JsonResponse jsonResponse = new JsonResponse();
            try
            {
                TempData["Message"] = string.Empty;
                ModelState.Remove("Id");
                if (!ModelState.IsValid)
                {
                    TempData["Message"] = NotificationMessage.RequiredFieldsValidation;
                    model.DistributorList = new DistributorBLL(_unitOfWork).DropDownDistributorList(Convert.ToInt32(model.DistributorId));
                    model.SubDistributorList = new DistributorBLL(_unitOfWork).DropDownDistributorList(Convert.ToInt32(model.DistributorId));
                    return PartialView("Add", model);
                }
                else
                {
                    if (_SubDistributorBLL.CheckDistributor(model.Id, model.DistributorId))
                    {
                        if (model.SubDistributorIds.Where(x => x != model.DistributorId).ToArray().Count() == 0)
                        {
                            TempData["Message"] = "Aleast add one Sub distributor.";
                            model.DistributorList = new DistributorBLL(_unitOfWork).DropDownDistributorList(Convert.ToInt32(model.DistributorId));
                            model.SubDistributorList = new DistributorBLL(_unitOfWork).DropDownDistributorList(Convert.ToInt32(model.DistributorId));
                            return PartialView("Add", model);
                        }
                        if (model.Id > 0)
                        {
                            _SubDistributorBLL.Update(model);
                            jsonResponse.Status = true;
                            jsonResponse.Message = NotificationMessage.UpdateSuccessfully;
                            jsonResponse.RedirectURL = Url.Action("Index", "SubDistributor");
                        }
                        else
                        {
                            _SubDistributorBLL.Add(model);
                            jsonResponse.Status = true;
                            jsonResponse.Message = NotificationMessage.SaveSuccessfully;
                            jsonResponse.RedirectURL = Url.Action("Index", "SubDistributor");
                        }
                    }
                    else
                    {
                        TempData["Message"] = "Already exist as parent distributor.";
                        model.DistributorList = new DistributorBLL(_unitOfWork).DropDownDistributorList(Convert.ToInt32(model.DistributorId));
                        model.SubDistributorList = new DistributorBLL(_unitOfWork).DropDownDistributorList(Convert.ToInt32(model.DistributorId));
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

                SubDistributor SubDistributor = _SubDistributorBLL.Where(x => x.Id == id).FirstOrDefault();
                List<SubDistributor> SubDistributorList = _SubDistributorBLL.Where(x => !x.IsDeleted && x.DistributorId == SubDistributor.DistributorId).ToList();
                foreach (var item in SubDistributorList)
                {
                    _SubDistributorBLL.Delete(item.Id);
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
        private SubDistributor BindSubDistributor(int Id)
        {
            SubDistributor model = new SubDistributor();
            if (Id > 0)
            {
                model = _SubDistributorBLL.GetById(Id);
            }
            else
            {
                model.IsActive = true;
            }
            model.SubDistributorIds = _SubDistributorBLL.Where(x => x.DistributorId == model.DistributorId && !x.IsDeleted).Select(x => x.SubDistributorId).ToArray();
            model.DistributorList = new DistributorBLL(_unitOfWork).DropDownDistributorList(Convert.ToInt32(model.DistributorId));
            model.SubDistributorList = new DistributorBLL(_unitOfWork).DropDownDistributorList(Convert.ToInt32(model.DistributorId));
            return model;
        }
    }
}