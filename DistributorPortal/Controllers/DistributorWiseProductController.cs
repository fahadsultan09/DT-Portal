using BusinessLogicLayer.Application;
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
using BusinessLogicLayer.ApplicationSetup;
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
            return View(_DistributorWiseProductDiscountAndPricesBLL.GetAllDistributorWiseProductDiscountAndPrices());
        }
        public IActionResult List()
        {
            return PartialView("List", _DistributorWiseProductDiscountAndPricesBLL.GetAllDistributorWiseProductDiscountAndPrices());
        }
        [HttpGet]
        public IActionResult Sync()
        {
            JsonResponse jsonResponse = new JsonResponse();
            try
            {
                List<DistributorWiseProductViewModel> master = new List<DistributorWiseProductViewModel>();
                var client = new RestClient(_configuration.DistributorWiseProduct);
                var request = new RestRequest(Method.GET);
                var distributors = _distributorBll.Where(e => e.IsActive && !e.IsDeleted);
                var productDetail = _productDetailBll.GetAllProductDetail();
                foreach (var item in distributors)
                {
                    client.AddDefaultParameter("DistributorId", item.DistributorCode);
                    IRestResponse response = client.Execute(request);
                    var distributorsProduct = JsonConvert.DeserializeObject<List<DistributorWiseProductViewModel>>(response.Content);
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
