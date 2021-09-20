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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Utility.HelperClasses;

namespace DistributorPortal.Controllers
{
    public class DistributorController : BaseController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly DistributorBLL _DistributorBLL;
        private readonly Configuration _configuration;
        private readonly RegionBLL regionBLL;
        public DistributorController(IUnitOfWork unitOfWork, Configuration configuration)
        {
            _unitOfWork = unitOfWork;
            _DistributorBLL = new DistributorBLL(_unitOfWork);
            _configuration = configuration;
            regionBLL = new RegionBLL(_unitOfWork);
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
            List<Distributor> SAPDistributor = new List<Distributor>();
            Root root = new Root();
            try
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    string authInfo = Convert.ToBase64String(Encoding.Default.GetBytes(_configuration.POUserName + ":" + _configuration.POPassword));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authInfo);
                    var result = client.GetAsync(new Uri(_configuration.SyncDistributorURL)).Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var JsonContent = result.Content.ReadAsStringAsync().Result;
                        root = JsonConvert.DeserializeObject<Root>(JsonContent.ToString());
                        for (int i = 0; i < root.ZWASITHRMSBAPIResponse.DISTRIBUTOR.item.Count(); i++)
                        {
                            SAPDistributor.Add(new Distributor()
                            {
                                DistributorSAPCode = root.ZWASITHRMSBAPIResponse.DISTRIBUTOR.item[i].KUNNR,
                                DistributorName = root.ZWASITHRMSBAPIResponse.DISTRIBUTOR.item[i].NAME1,
                                City = root.ZWASITHRMSBAPIResponse.DISTRIBUTOR.item[i].ORT01,
                                RegionCode = root.ZWASITHRMSBAPIResponse.DISTRIBUTOR.item[i].REGIO,
                                CustomerGroup = root.ZWASITHRMSBAPIResponse.DISTRIBUTOR.item[i].KDGRPT,
                                DistributorAddress = root.ZWASITHRMSBAPIResponse.DISTRIBUTOR.item[i].STRAS,
                                NTN = root.ZWASITHRMSBAPIResponse.DISTRIBUTOR.item[i].STCD2,
                                CNIC = root.ZWASITHRMSBAPIResponse.DISTRIBUTOR.item[i].STCD1,
                                EmailAddress = root.ZWASITHRMSBAPIResponse.DISTRIBUTOR.item[i].EMAIL,
                                MobileNumber = root.ZWASITHRMSBAPIResponse.DISTRIBUTOR.item[i].TELF1
                            });
                        }
                    }
                }
                var alldist = _DistributorBLL.GetAllDistributors();
                var addDistributor = SAPDistributor.Where(e => !alldist.Any(c => c.DistributorSAPCode == e.DistributorSAPCode) && e.CustomerGroup.Contains("Local")).ToList();
                addDistributor.ForEach(e =>
                {
                    e.MobileNumber = e.MobileNumber.Replace("-", "");
                    e.CNIC = e.CNIC.Replace("-", "");
                    e.IsActive = true;
                    e.IsDeleted = false;
                    e.CreatedBy = SessionHelper.LoginUser.Id;
                    e.CreatedDate = DateTime.Now;
                });
                _DistributorBLL.AddRange(addDistributor);
                var UpdateDistributor = SAPDistributor.Where(e => alldist.Select(x => x.DistributorSAPCode).Contains(e.DistributorSAPCode)).ToList();
                foreach (var item in UpdateDistributor)
                {
                    var distributor = _DistributorBLL.GetDistributorBySAPId(item.DistributorSAPCode);
                    if (distributor != null)
                    {
                        var region = regionBLL.GetAllRegion();
                        item.Region = region.First(c => c.SAPId == distributor.Region.SAPId);
                        _DistributorBLL.UpdateDistributor(item);
                    }
                }

                jsonResponse.Message = "Distributor sync successfully";
                jsonResponse.Status = true;
                jsonResponse.RedirectURL = Url.Action("Index", "Distributor");
                return Json(new { data = jsonResponse });
            }
            catch (Exception ex)
            {
                new ErrorLogBLL(_unitOfWork).AddExceptionLog(ex);
                jsonResponse.Status = false;
                jsonResponse.Message = NotificationMessage.ErrorOccurred;
                jsonResponse.RedirectURL = Url.Action("Index", "Distributor");
                return Json(new { data = jsonResponse });
            }
        }
        [HttpGet]
        public IActionResult Add(string DPID)
        {
            int id = 0;
            if (!string.IsNullOrEmpty(DPID))
            {
                int.TryParse(EncryptDecrypt.Decrypt(DPID), out id);
            }
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
                int.TryParse(EncryptDecrypt.Decrypt(DPID), out int id);
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
        public JsonResult IncomeTaxApplicable(string DPID)
        {
            JsonResponse jsonResponse = new JsonResponse();
            int.TryParse(EncryptDecrypt.Decrypt(DPID), out int id);
            var distributor = _DistributorBLL.FirstOrDefault(e => e.Id == id);

            if (distributor != null)
            {
                if (distributor.IsIncomeTaxApplicable)
                {
                    distributor.IsIncomeTaxApplicable = false;
                    _DistributorBLL.UpdateDistributor(distributor);
                }
                else
                {
                    distributor.IsIncomeTaxApplicable = true;
                    _DistributorBLL.UpdateDistributor(distributor);
                }
                jsonResponse.Status = true;
                jsonResponse.Message = NotificationMessage.UpdateSuccessfully;
            }
            else
            {
                jsonResponse.Status = false;
                jsonResponse.Message = NotificationMessage.ErrorOccurred;
            }
            return Json(new { data = jsonResponse });
        }

        public JsonResult SalesTaxApplicable(string DPID)
        {
            JsonResponse jsonResponse = new JsonResponse();
            int.TryParse(EncryptDecrypt.Decrypt(DPID), out int id);
            var distributor = _DistributorBLL.FirstOrDefault(e => e.Id == id);

            if (distributor != null)
            {
                if (distributor.IsSalesTaxApplicable)
                {
                    distributor.IsSalesTaxApplicable = false;
                    _DistributorBLL.UpdateDistributor(distributor);
                }
                else
                {
                    distributor.IsSalesTaxApplicable = true;
                    _DistributorBLL.UpdateDistributor(distributor);
                }
                jsonResponse.Status = true;
                jsonResponse.Message = NotificationMessage.UpdateSuccessfully;
            }
            else
            {
                jsonResponse.Status = false;
                jsonResponse.Message = NotificationMessage.ErrorOccurred;
            }
            return Json(new { data = jsonResponse });
        }
    }
}