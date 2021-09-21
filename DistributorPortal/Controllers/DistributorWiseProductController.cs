using BusinessLogicLayer.Application;
using BusinessLogicLayer.ApplicationSetup;
using BusinessLogicLayer.ErrorLog;
using DataAccessLayer.WorkProcess;
using DistributorPortal.Resource;
using Microsoft.AspNetCore.Mvc;
using Models.Application;
using Models.ViewModel;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
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
        private readonly ProductMasterBLL _ProductMasterBLL;
        private readonly Configuration _Configuration;
        public DistributorWiseProductController(IUnitOfWork unitOfWork, Configuration configuration)
        {
            _unitOfWork = unitOfWork;
            _DistributorWiseProductDiscountAndPricesBLL = new DistributorWiseProductDiscountAndPricesBLL(_unitOfWork);
            _distributorBll = new DistributorBLL(_unitOfWork);
            _configuration = configuration;
            _productDetailBll = new ProductDetailBLL(_unitOfWork);
            _ProductMasterBLL = new ProductMasterBLL(_unitOfWork);
            _Configuration = configuration;
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
        public IActionResult SyncDistributorWise(int DistributorId)
        {
            JsonResponse jsonResponse = new JsonResponse();
            try
            {
                List<DistributorWiseProductViewModel> master = new List<DistributorWiseProductViewModel>();
                var request = new RestRequest(Method.GET);
                var distributors = _distributorBll.Where(e => e.IsActive && !e.IsDeleted);
                var productDetail = _productDetailBll.GetAllProductDetail();
                var AllDistributorWiseProductDiscountAndPrices = _DistributorWiseProductDiscountAndPricesBLL.GetAllDistributorWiseProductDiscountAndPrices();
                if (DistributorId > 0)
                {
                    List<DistributorWiseProductViewModel> distributorsProduct = _DistributorWiseProductDiscountAndPricesBLL.GetDistributorWiseDiscountAndPrices(distributors.First(x => x.Id == DistributorId).DistributorSAPCode, _Configuration);
                    distributorsProduct.ForEach(e => e.DistributorId = DistributorId);
                    distributorsProduct.ForEach(e => e.ProductDetailId = productDetail.FirstOrDefault(c => c.ProductMaster.SAPProductCode == e.SAPProductCode)?.Id);
                    if (AllDistributorWiseProductDiscountAndPrices != null && AllDistributorWiseProductDiscountAndPrices.Count() > 0)
                    {
                        distributorsProduct.ForEach(e => e.ReturnMRPDicount = AllDistributorWiseProductDiscountAndPrices.FirstOrDefault(c => c.DistributorId == e.DistributorId && c.ProductDetailId == e.ProductDetailId) != null ? AllDistributorWiseProductDiscountAndPrices.FirstOrDefault(c => c.DistributorId == e.DistributorId && c.ProductDetailId == e.ProductDetailId).ReturnMRPDicount : 0);
                    }
                    master.AddRange(distributorsProduct);
                }
                else
                {
                    foreach (var item in distributors)
                    {
                        List<DistributorWiseProductViewModel> distributorsProduct = _DistributorWiseProductDiscountAndPricesBLL.GetDistributorWiseDiscountAndPrices(item.DistributorSAPCode, _Configuration);
                        distributorsProduct.ForEach(e => e.DistributorId = item.Id);
                        distributorsProduct.ForEach(e => e.ProductDetailId = productDetail.FirstOrDefault(c => c.ProductMaster.SAPProductCode == e.SAPProductCode)?.Id);
                        if (AllDistributorWiseProductDiscountAndPrices != null && AllDistributorWiseProductDiscountAndPrices.Count() > 0)
                        {
                            distributorsProduct.ForEach(e => e.ReturnMRPDicount = AllDistributorWiseProductDiscountAndPrices.FirstOrDefault(c => c.DistributorId == e.DistributorId && c.ProductDetailId == e.ProductDetailId) != null ? AllDistributorWiseProductDiscountAndPrices.FirstOrDefault(c => c.DistributorId == e.DistributorId && c.ProductDetailId == e.ProductDetailId).ReturnMRPDicount : 0);
                        }
                        master.AddRange(distributorsProduct);
                    }
                }
                if (master.Count() > 0 && master != null)
                {
                    _DistributorWiseProductDiscountAndPricesBLL.AddRange(master, DistributorId, null);
                }
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
        [HttpGet]
        public IActionResult SyncProductWise(string[] ProductIds)
        {
            JsonResponse jsonResponse = new JsonResponse();
            try
            {
                if (ProductIds.Count() > 0)
                {
                    List<DistributorWiseProductViewModel> master = new List<DistributorWiseProductViewModel>();
                    var request = new RestRequest(Method.GET);
                    var distributors = _distributorBll.Where(e => e.IsActive && !e.IsDeleted);
                    var productDetail = _productDetailBll.GetAllProductDetail();
                    var AllDistributorWiseProductDiscountAndPrices = _DistributorWiseProductDiscountAndPricesBLL.GetAllDistributorWiseProductDiscountAndPrices();
                    List<DistributorWiseProductViewModel> distributorsProduct = _DistributorWiseProductDiscountAndPricesBLL.GetProductWiseDiscountAndPrices(ProductIds, _Configuration);
                    distributorsProduct = distributorsProduct.Where(x => distributors.Select(y => y.DistributorSAPCode).Contains(x.SAPDistributorCode)).ToList();
                    distributorsProduct.ForEach(x => x.DistributorId = distributors.FirstOrDefault(c => c.DistributorSAPCode == x.SAPDistributorCode).Id);
                    distributorsProduct.ForEach(x => x.ProductDetailId = productDetail.FirstOrDefault(c => c.ProductMaster.SAPProductCode == x.SAPProductCode)?.Id);
                    if (AllDistributorWiseProductDiscountAndPrices != null && AllDistributorWiseProductDiscountAndPrices.Count() > 0)
                    {
                        distributorsProduct.ForEach(e => e.ReturnMRPDicount = AllDistributorWiseProductDiscountAndPrices.FirstOrDefault(c => c.DistributorId == e.DistributorId && c.ProductDetailId == e.ProductDetailId) != null ? AllDistributorWiseProductDiscountAndPrices.FirstOrDefault(c => c.DistributorId == e.DistributorId && c.ProductDetailId == e.ProductDetailId).ReturnMRPDicount : 0);
                    }
                    master.AddRange(distributorsProduct);

                    if (master.Count() > 0 && master != null)
                    {
                        _DistributorWiseProductDiscountAndPricesBLL.AddRange(master, 0, ProductIds);
                    }
                    jsonResponse.Status = true;
                    jsonResponse.Message = NotificationMessage.SyncedSuccessfully;
                    jsonResponse.RedirectURL = Url.Action("Index", "DistributorWiseProduct");
                    return Json(new { data = jsonResponse });
                }
                else
                {
                    jsonResponse.Status = false;
                    jsonResponse.Message = "Select atleast one product";
                    return Json(new { data = jsonResponse });
                }
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
