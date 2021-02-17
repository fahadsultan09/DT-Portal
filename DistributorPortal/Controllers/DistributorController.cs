using BusinessLogicLayer.Application;
using BusinessLogicLayer.ApplicationSetup;
using BusinessLogicLayer.ErrorLog;
using BusinessLogicLayer.HelperClasses;
using DataAccessLayer.WorkProcess;
using DistributorPortal.Resource;
using Microsoft.AspNetCore.Mvc;
using Models.Application;
using Models.ViewModel;
using Newtonsoft.Json;
using RestSharp;
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
        private readonly Configuration _configuration;
        public DistributorController(IUnitOfWork unitOfWork, Configuration configuration)
        {
            _unitOfWork = unitOfWork;
            _DistributorBLL = new DistributorBLL(_unitOfWork);
            _configuration = configuration;
        }
        // GET: Distributor
        public IActionResult Index()
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
            JsonResponse jsonResponse = new JsonResponse();
            try
            {                
                var Client = new RestClient(_configuration.SyncDistributorURL);
                var request = new RestRequest(Method.GET);
                IRestResponse response = Client.Execute(request);
                var SAPDistributor = JsonConvert.DeserializeObject<List<Distributor>>(response.Content);
                var alldist = _DistributorBLL.GetAllDistributor();
                var addDistributor = SAPDistributor.Where(e => !alldist.Any(c => c.DistributorSAPCode == e.DistributorSAPCode) && e.CustomerGroup.Contains("Local")).ToList();
                addDistributor.ForEach(e =>
                {
                    e.CreatedBy = SessionHelper.LoginUser.Id; e.MobileNumber = e.MobileNumber.Replace("-", ""); e.CNIC = e.CNIC.Replace("-", ""); e.IsDeleted = false; e.IsActive = true; e.CreatedDate = DateTime.Now;
                });
                _DistributorBLL.AddRange(addDistributor);
                var UpdateDistributor = alldist.Where(e => SAPDistributor.Any(c => c.DistributorSAPCode == e.DistributorSAPCode && (c.City != e.City || c.DistributorCode != e.DistributorCode || c.DistributorName != e.DistributorName || c.DistributorAddress != e.DistributorAddress || c.NTN != c.NTN || e.CNIC != e.CNIC || c.EmailAddress != e.EmailAddress || c.MobileNumber != e.MobileNumber || c.CustomerGroup != e.CustomerGroup)));
                foreach (var item in UpdateDistributor)
                {
                    var distributor = _DistributorBLL.GetDistributorBySAPId(item.DistributorSAPCode);
                    if (distributor != null)
                    {
                        _DistributorBLL.UpdateDistributor(distributor);
                    }
                }

                jsonResponse.Message = "Distributor Sync Successfully";
                jsonResponse.Status = true;
                jsonResponse.RedirectURL = Url.Action("Index", "Distributor");
                return Json(new { data = jsonResponse });
            }
            catch (Exception ex)
            {
                jsonResponse.Message = ex.Message;
                jsonResponse.Status = false;
                jsonResponse.RedirectURL = Url.Action("Index", "Distributor");
                return Json(new { data = jsonResponse });
            }
        }
        [HttpGet]
        public IActionResult Add(string DPID)
        {
            int id;
            int.TryParse(EncryptDecrypt.Decrypt(DPID), out id);
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
        public IActionResult Delete(string DPID)
        {
            try
            {
                int id;
                int.TryParse(EncryptDecrypt.Decrypt(DPID), out id);
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
            return model;
        }
        public JsonResult GetDistributorList()
        {
            return Json(_DistributorBLL.GetAllDistributor().ToList());
        }
    }
}