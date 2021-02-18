using BusinessLogicLayer.Application;
using BusinessLogicLayer.ErrorLog;
using DataAccessLayer.WorkProcess;
using DistributorPortal.Resource;
using Microsoft.AspNetCore.Mvc;
using Models.Application;
using System;
using System.Linq;
using Utility.HelperClasses;

namespace DistributorPortal.Controllers
{
    public class RegionController : BaseController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly RegionBLL _RegionBLL;
        public RegionController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _RegionBLL = new RegionBLL(_unitOfWork);
        }
        // GET: Region
        public IActionResult Index()
        {
            return View(_RegionBLL.GetAllRegion());
        }
        public IActionResult List()
        {
            return PartialView("List", _RegionBLL.GetAllRegion());
        }
        [HttpGet]
        public IActionResult Add(string DPID)
        {
            int id=0;
            if (!string.IsNullOrEmpty(DPID))
            {
                int.TryParse(EncryptDecrypt.Decrypt(DPID), out id);
            }
            return PartialView("Add", BindRegion(id));
        }
        [HttpPost]
        public IActionResult SaveEdit(Region model)
        {
            try
            {
                ModelState.Remove("Id");
                if (!ModelState.IsValid)
                {
                    TempData["Message"] = NotificationMessage.RequiredFieldsValidation;
                    return PartialView("Add", model);
                }
                else
                {
                    if (_RegionBLL.CheckRegionName(model.Id, model.RegionName))
                    {
                        if (model.Id > 0)
                        {
                            _RegionBLL.UpdateRegion(model);
                            TempData["Message"] = NotificationMessage.UpdateSuccessfully;
                        }
                        else
                        {
                            _RegionBLL.AddRegion(model);
                            TempData["Message"] = NotificationMessage.SaveSuccessfully;
                        }
                    }
                    else
                    {
                        TempData["Message"] = "Region name already exist";
                        return PartialView("Add", model);
                    }
                }
                return RedirectToAction("List");
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
                int id=0;
                int.TryParse(EncryptDecrypt.Decrypt(DPID), out id);
                _RegionBLL.DeleteRegion(id);
                return Json(new { Result = true });
            }
            catch (Exception ex)
            {
                new ErrorLogBLL(_unitOfWork).AddExceptionLog(ex);
                TempData["Message"] = NotificationMessage.ErrorOccurred;
                return RedirectToAction("List");
            }
        }
        private Region BindRegion(int Id)
        {
            Region model = new Region();
            if (Id > 0)
            {
                model = _RegionBLL.GetRegionById(Id);
            }
            else
            {
                model.IsActive = true;
            }
            return model;
        }
        public JsonResult GetRegionList()
        {
            return Json(_RegionBLL.GetAllRegion().ToList());
        }
    }
}