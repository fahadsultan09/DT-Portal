using BusinessLogicLayer.Application;
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
    public class SubRegionController : BaseController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly SubRegionBLL _SubRegionBLL;
        public SubRegionController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _SubRegionBLL = new SubRegionBLL(_unitOfWork);
        }
        // GET: SubRegion
        public IActionResult Index()
        {
            return View(_SubRegionBLL.GetAllSubRegion());
        }
        public IActionResult List()
        {
            return PartialView("List", _SubRegionBLL.GetAllSubRegion());
        }
        [HttpGet]
        public IActionResult Add(string DPID)
        {
            int id=0;
            if (!string.IsNullOrEmpty(DPID))
            {
                int.TryParse(EncryptDecrypt.Decrypt(DPID), out id);
            }
            return PartialView("Add", BindSubRegion(id));
        }
        [HttpPost]
        public IActionResult SaveEdit(SubRegion model)
        {
            JsonResponse jsonResponse = new JsonResponse();
            try
            {
                ModelState.Remove("Id");
                if (!ModelState.IsValid)
                {
                    jsonResponse.Status = false;
                    jsonResponse.Message = NotificationMessage.RequiredFieldsValidation;
                }
                else
                {
                    if (_SubRegionBLL.CheckSubRegionName(model.Id, model.SubRegionName))
                    {
                        if (model.Id > 0)
                        {
                            _SubRegionBLL.UpdateSubRegion(model);

                            jsonResponse.Status = true;
                            jsonResponse.Message = NotificationMessage.UpdateSuccessfully;
                            jsonResponse.RedirectURL = Url.Action("Index", "SubRegion");
                        }
                        else
                        {
                            _SubRegionBLL.AddSubRegion(model);

                            jsonResponse.Status = true;
                            jsonResponse.Message = NotificationMessage.SaveSuccessfully;
                            jsonResponse.RedirectURL = Url.Action("Index", "SubRegion");
                        }
                    }
                    else
                    {
                        jsonResponse.Status = false;
                        jsonResponse.Message = "Sub Region name already exist";

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
                _SubRegionBLL.DeleteSubRegion(id);
                return Json(new { Result = true });
            }
            catch (Exception ex)
            {
                new ErrorLogBLL(_unitOfWork).AddExceptionLog(ex);
                TempData["Message"] = NotificationMessage.ErrorOccurred;
                return RedirectToAction("List");
            }
        }
        private SubRegion BindSubRegion(int Id)
        {
            SubRegion model = new SubRegion();
            if (Id > 0)
            {
                model = _SubRegionBLL.GetSubRegionById(Id);
            }
            else
            {
                model.IsActive = true;
            }
            model.RegionList = new RegionBLL(_unitOfWork).DropDownRegionList(model.RegionId);
            return model;
        }
        public JsonResult GetSubRegionList()
        {
            return Json(_SubRegionBLL.GetAllSubRegion().ToList());
        }
        public IActionResult DropDownSubRegionList(string DPID)
        {
            int id=0;
            int.TryParse(EncryptDecrypt.Decrypt(DPID), out id);
            return Json(_SubRegionBLL.DropDownSubRegionList(id, 0));
        }
    }
}