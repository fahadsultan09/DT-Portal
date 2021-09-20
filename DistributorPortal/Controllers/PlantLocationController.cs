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
    public class PlantLocationController : BaseController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly PlantLocationBLL _PlantLocationBLL;
        public PlantLocationController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _PlantLocationBLL = new PlantLocationBLL(_unitOfWork);
        }
        // GET: PlantLocation
        public IActionResult Index()
        {
            return View(_PlantLocationBLL.GetAllPlantLocation());
        }
        public IActionResult List()
        {
            return PartialView("List", _PlantLocationBLL.GetAllPlantLocation());
        }
        [HttpGet]
        public IActionResult Add(string DPID)
        {
            int id = 0;
            if (!string.IsNullOrEmpty(DPID))
            {
                int.TryParse(EncryptDecrypt.Decrypt(DPID), out id);
            }
            return PartialView("Add", BindPlantLocation(id));
        }
        [HttpPost]
        public IActionResult SaveEdit(PlantLocation model)
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
                    if (_PlantLocationBLL.CheckPlantLocationName(model.Id, model.PlantLocationName))
                    {
                        if (model.Id > 0)
                        {
                            _PlantLocationBLL.UpdatePlantLocation(model);
                            jsonResponse.Status = true;
                            jsonResponse.Message = NotificationMessage.UpdateSuccessfully;
                            jsonResponse.RedirectURL = Url.Action("Index", "PlantLocation");
                        }
                        else
                        {
                            _PlantLocationBLL.AddPlantLocation(model);
                            jsonResponse.Status = true;
                            jsonResponse.Message = NotificationMessage.SaveSuccessfully;
                            jsonResponse.RedirectURL = Url.Action("Index", "PlantLocation");
                        }
                    }
                    else
                    {
                        TempData["Message"] = "PlantLocation name already exist";
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
                _PlantLocationBLL.DeletePlantLocation(id);
                return Json(new { Result = true });
            }
            catch (Exception ex)
            {
                new ErrorLogBLL(_unitOfWork).AddExceptionLog(ex);
                TempData["Message"] = NotificationMessage.ErrorOccurred;
                return RedirectToAction("List");
            }
        }
        private PlantLocation BindPlantLocation(int Id)
        {
            PlantLocation model = new PlantLocation();
            if (Id > 0)
            {
                model = _PlantLocationBLL.GetPlantLocationById(Id);
            }
            else
            {
                model.IsActive = true;
            }
            return model;
        }
        public JsonResult GetPlantLocationList()
        {
            return Json(_PlantLocationBLL.GetAllPlantLocation().ToList());
        }
    }
}