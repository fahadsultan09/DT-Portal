using BusinessLogicLayer.Application;
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
    public class CityController : BaseController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly CityBLL _CityBLL;
        public CityController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _CityBLL = new CityBLL(_unitOfWork);
        }
        // GET: City
        public IActionResult Index()
        {
            return View(_CityBLL.GetAllCity());
        }
        public IActionResult List()
        {
            return PartialView("List", _CityBLL.GetAllCity());
        }
        [HttpGet]
        public IActionResult Add(string DPID)
        {
            int id;
            int.TryParse(EncryptDecrypt.Decrypt(DPID), out id);
            return PartialView("Add", BindCity(id));
        }
        [HttpPost]
        public IActionResult SaveEdit(City model)
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
                    if (_CityBLL.CheckCityName(model.Id, model.CityName))
                    {
                        if (model.Id > 0)
                        {
                            _CityBLL.UpdateCity(model);
                            jsonResponse.Status = true;
                            jsonResponse.Message = NotificationMessage.UpdateSuccessfully;
                            jsonResponse.RedirectURL = Url.Action("Index", "City");
                        }
                        else
                        {
                            _CityBLL.AddCity(model);
                            jsonResponse.Status = true;
                            jsonResponse.Message = NotificationMessage.SaveSuccessfully;
                            jsonResponse.RedirectURL = Url.Action("Index", "City");
                        }
                    }
                    else
                    {
                        jsonResponse.Status = false;
                        jsonResponse.Message = "City name already exist";
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
                int id;
                int.TryParse(EncryptDecrypt.Decrypt(DPID), out id);
                _CityBLL.DeleteCity(id);
                return Json(new { Result = true });
            }
            catch (Exception ex)
            {
                new ErrorLogBLL(_unitOfWork).AddExceptionLog(ex);
                TempData["Message"] = NotificationMessage.ErrorOccurred;
                return RedirectToAction("List");
            }
        }
        private City BindCity(int Id)
        {
            City model = new City();
            if (Id > 0)
            {
                model = _CityBLL.GetCityById(Id);
            }
            else
            {
                model.IsActive = true;
            }
            model.SubRegionList = new SubRegionBLL(_unitOfWork).DropDownSubRegionList(model.SubRegionId);
            return model;
        }
        public JsonResult GetCityList()
        {
            return Json(_CityBLL.GetAllCity().ToList());
        }
        public IActionResult DropDownCityList(string DPID)
        {
            int id;
            int.TryParse(EncryptDecrypt.Decrypt(DPID), out id);
            return Json(_CityBLL.DropDownCityList(id, 0));
        }
    }
}