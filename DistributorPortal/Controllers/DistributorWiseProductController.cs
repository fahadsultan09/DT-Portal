using BusinessLogicLayer.Application;
using BusinessLogicLayer.ApplicationSetup;
using BusinessLogicLayer.ErrorLog;
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
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Utility.HelperClasses;

namespace DistributorPortal.Controllers
{
    public class DistributorWiseProductController : BaseController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly DistributorWiseProductDiscountAndPricesBLL _DistributorWiseProductDiscountAndPricesBLL;
        private readonly DistributorBLL _distributorBll;
        private readonly Configuration _configuration;
        private readonly ProductDetailBLL _productDetailBll;
        public DistributorWiseProductController(IUnitOfWork unitOfWork, Configuration configuration)
        {
            _unitOfWork = unitOfWork;
            _DistributorWiseProductDiscountAndPricesBLL = new DistributorWiseProductDiscountAndPricesBLL(_unitOfWork);
            _distributorBll = new DistributorBLL(_unitOfWork);
            _configuration = configuration;
            _productDetailBll = new ProductDetailBLL(_unitOfWork);
        }
        // GET: Product
        public IActionResult Index()
        {
            List<DistributorWiseProductDiscountAndPrices> list = new List<DistributorWiseProductDiscountAndPrices>();

            return View(list);
        }
        public PartialViewResult List(int DistributorId)
        {
            ViewBag.DistributorId = DistributorId;
            return PartialView("List", _DistributorWiseProductDiscountAndPricesBLL.Where(x => x.DistributorId == DistributorId).ToList());
        }
        [HttpGet]
        public IActionResult Sync()
        {
            JsonResponse jsonResponse = new JsonResponse();
            try
            {
                List<DistributorWiseProductViewModel> master = new List<DistributorWiseProductViewModel>();
                List<DistributorWiseProductViewModel> distributorsProduct = new List<DistributorWiseProductViewModel>();
                var request = new RestRequest(Method.GET);
                var distributors = _distributorBll.Where(e => e.IsActive && !e.IsDeleted);
                var productDetail = _productDetailBll.GetAllProductDetail();
                foreach (var item in distributors)
                {
                    using (var client = new HttpClient())
                    {
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        string authInfo = Convert.ToBase64String(Encoding.Default.GetBytes("sami_po:wasay123")); //("Username:Password")  
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authInfo);
                        var result = client.GetAsync(new Uri("http://10.0.3.35:51000/RESTAdapter/PriceDisc?DISTRIBUTOR=" + item.DistributorSAPCode)).Result;
                        if (result.IsSuccessStatusCode)
                        {
                            var JsonContent = result.Content.ReadAsStringAsync().Result;
                            distributorsProduct = JsonConvert.DeserializeObject<List<DistributorWiseProductViewModel>>(JsonContent);
                        }
                    }
                    distributorsProduct.ForEach(e => e.DistributorId = item.Id);
                    distributorsProduct.ForEach(e => e.ProductDetailId = productDetail.FirstOrDefault(c => c.ProductMaster.SAPProductCode == e.SAPProductCode)?.Id);
                    master.AddRange(distributorsProduct);
                }
                _DistributorWiseProductDiscountAndPricesBLL.AddRange(master);
                jsonResponse.Status = true;
                jsonResponse.Message = NotificationMessage.SyncedSuccessfully;
                jsonResponse.RedirectURL = Url.Action("Index", "DistributorWiseProduct");
                return Json(new { data = jsonResponse });
            }
            catch (Exception ex)
            {
                new ErrorLogBLL(_unitOfWork).AddExceptionLog(ex);
                jsonResponse.Status = false;
                jsonResponse.Message = NotificationMessage.ErrorOccurred;
                jsonResponse.RedirectURL = Url.Action("Index", "DistributorWiseProduct");
                return Json(new { data = jsonResponse });
            }
        }
    }
}
