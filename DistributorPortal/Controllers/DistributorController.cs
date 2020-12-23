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
    public class DistributorController : BaseController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly DistributorBLL _DistributorBLL;
        public DistributorController(IUnitOfWork unitOfWork, Configuration configuration)
        {
            _unitOfWork = unitOfWork;
            _DistributorBLL = new DistributorBLL(_unitOfWork);
        }

        // GET: Distributor
        public ActionResult Index()
        {
            return View(_DistributorBLL.GetAllDistributor());
        }

        public IActionResult List()
        {
            return PartialView("List", _DistributorBLL.GetAllDistributor());
        }
        [HttpGet]
        public IActionResult Sync()
        {
            List<string> distributorCodes = _DistributorBLL.GetAllDistributor().Select(x => x.DistributorCode).ToList();
            List<Distributor> distributorList = new List<Distributor>();
            foreach (var item in distributorList)
            {
                if (distributorCodes.Contains(item.DistributorCode))
                {
                    _DistributorBLL.UpdateDistributor(item);
                }
                else
                {
                    _DistributorBLL.AddDistributor(item);
                }
            }
            return PartialView("List", _DistributorBLL.GetAllDistributor());
        }

        [HttpGet]
        public IActionResult Add(int id)
        {
            return PartialView("Add", BindDistributor(id));
        }

        [HttpPost]
        public JsonResult SaveEdit(Distributor model)
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
                    if (_DistributorBLL.CheckDistributorName(model.Id, model.DistributorCode))
                    {
                        if (model.Id > 0)
                        {
                            _DistributorBLL.UpdateDistributor(model);
                            jsonResponse.Status = true;
                            jsonResponse.Message = NotificationMessage.UpdateSuccessfully;
                            jsonResponse.RedirectURL = Url.Action("Index", "Distributor");
                        }
                        else
                        {
                            _DistributorBLL.AddDistributor(model);
                            jsonResponse.Status = true;
                            jsonResponse.Message = NotificationMessage.SaveSuccessfully;
                            jsonResponse.RedirectURL = Url.Action("Index", "Distributor");
                        }
                    }
                    else
                    {
                        jsonResponse.Status = false;
                        jsonResponse.Message = "Distributor name already exist";
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
        public IActionResult Delete(int id)
        {
            try
            {
                _DistributorBLL.DeleteDistributor(id);
                return Json(new { Result = true });
            }
            catch (Exception ex)
            {
                new ErrorLogBLL(_unitOfWork).AddExceptionLog(ex);
                ViewBag.Records = NotificationMessage.ErrorOccurred;
                return RedirectToAction("List");
            }
        }

        private Distributor BindDistributor(int Id)
        {
            Distributor model = new Distributor();
            if (Id > 0)
            {
                model = _DistributorBLL.GetDistributorById(Id);
            }
            else
            {
                model.IsActive = true;
            }
            model.RegionList = new RegionBLL(_unitOfWork).DropDownRegionList(model.RegionId);
            model.SubRegionList = new SubRegionBLL(_unitOfWork).DropDownSubRegionList(model.RegionId, model.SubRegionId);
            model.CityList = new CityBLL(_unitOfWork).DropDownCityList(model.SubRegionId, model.CityId);
            return model;
        }

        public JsonResult GetDistributorList()
        {
            return Json(_DistributorBLL.GetAllDistributor().ToList());
        }
    }
}