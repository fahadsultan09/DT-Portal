﻿using BusinessLogicLayer.Application;
using BusinessLogicLayer.ErrorLog;
using BusinessLogicLayer.HelperClasses;
using DataAccessLayer.WorkProcess;
using DistributorPortal.Controllers;
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

namespace ProductPortal.Controllers
{
    public class ProductController : BaseController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ProductMasterBLL _ProductMasterBLL;
        private readonly ProductDetailBLL _ProductDetailBLL;
        private readonly Configuration _configuration;
        public ProductController(IUnitOfWork unitOfWork, Configuration configuration)
        {
            _unitOfWork = unitOfWork;
            _ProductMasterBLL = new ProductMasterBLL(_unitOfWork);
            _ProductDetailBLL = new ProductDetailBLL(_unitOfWork);
            _configuration = configuration;
        }
        // GET: Product
        public IActionResult Index()
        {
            new AuditLogBLL(_unitOfWork).AddAuditLog("Product", "Index", " Form");
            return View(_ProductMasterBLL.GetAllProductMaster());
        }
        public IActionResult List()
        {
            return PartialView("List", _ProductMasterBLL.GetAllProductMaster());
        }
        [HttpGet]
        public IActionResult Sync()
        {
            JsonResponse jsonResponse = new JsonResponse();
            try
            {
                new AuditLogBLL(_unitOfWork).AddAuditLog("Product", "Sync", "Start Click on Sync Button of ");
                var Client = new RestClient(_configuration.SyncProductURL);
                var request = new RestRequest(Method.GET);
                IRestResponse response = Client.Execute(request);
                var SAPProduct = JsonConvert.DeserializeObject<List<ProductMaster>>(response.Content);
                var allproduct = _ProductMasterBLL.GetAllProductMaster();
                var addProduct = SAPProduct.Where(e => !allproduct.Any(c => c.SAPProductCode == e.SAPProductCode)).ToList();
                var updateProduct = SAPProduct.Where(e => allproduct.Any(c => c.SAPProductCode == e.SAPProductCode)).ToList();
                if (addProduct != null && addProduct.Count() > 0)
                {
                    addProduct.ForEach(e =>
                    {
                        e.CreatedBy = SessionHelper.LoginUser.Id;
                        e.IsDeleted = false;
                        e.IsActive = true;
                        e.CreatedDate = DateTime.Now;
                    });
                    _ProductMasterBLL.AddRange(addProduct);
                }
                else
                {
                    List<ProductMaster> list = new List<ProductMaster>();
                    updateProduct.ForEach(e =>
                    {
                        var item = _unitOfWork.GenericRepository<ProductMaster>().FirstOrDefault(x => x.SAPProductCode == e.SAPProductCode);
                        item.ProductName = e.ProductName;
                        item.PackCode = e.PackCode;
                        item.SAPProductCode = e.SAPProductCode;
                        item.Discount = e.Discount;
                        item.Rate = e.Rate;
                        item.TradePrice = e.TradePrice;
                        item.SFSize = e.SFSize;
                        item.CartonSize = e.CartonSize;
                        item.Strength = e.Strength;
                        item.PackSize = e.PackSize;
                        item.ProductOrigin = e.ProductOrigin;
                        item.ProductPrice = e.ProductPrice;
                        item.ProductDescription = e.ProductDescription;
                        item.LicenseType = e.LicenseType;
                        item.IsActive = e.IsActive;
                        item.UpdatedBy = SessionHelper.LoginUser.Id;
                        item.UpdatedDate = DateTime.Now;
                        list.Add(item);
                    });
                    _ProductMasterBLL.UpdateRange(list);
                }
                new AuditLogBLL(_unitOfWork).AddAuditLog("Product", "Sync", "End Click on Sync Button of ");
                jsonResponse.Status = true;
                jsonResponse.Message = NotificationMessage.SyncedSuccessfully;
                jsonResponse.RedirectURL = Url.Action("Index", "Product");
                return Json(new { data = jsonResponse });
            }
            catch (Exception ex)
            {
                new ErrorLogBLL(_unitOfWork).AddExceptionLog(ex);
                jsonResponse.Status = false;
                jsonResponse.Message = NotificationMessage.ErrorOccurred;
                jsonResponse.RedirectURL = Url.Action("Index", "Product");
                return Json(new { data = jsonResponse });
            }
        }
        [HttpGet]
        public IActionResult ProductMapping()
        {
            new AuditLogBLL(_unitOfWork).AddAuditLog("Product", "ProductMapping", "Get Prodct Mapping ");
            List<ProductDetail> productDetails = _ProductDetailBLL.GetAllProductDetail();
            List<ProductMaster> productMasters = _ProductMasterBLL.GetAllProductMaster();
            productMasters.ForEach(x => x.ProductDetail = productDetails.Where(y => y.ProductMasterId == x.Id).FirstOrDefault() ?? new ProductDetail());
            return View("ProductMapping", productMasters);
        }
        [HttpPost]
        public IActionResult UpdateProductDetail(ProductDetail model)
        {
            JsonResponse jsonResponse = new JsonResponse();
            try
            {
                new AuditLogBLL(_unitOfWork).AddAuditLog("Product", "UpdateProductDetail", "Start Click on UpdateProductDetail Button of ");
                if (model.Id > 0)
                {
                    _ProductDetailBLL.UpdateProductDetail(model);
                }
                else
                {
                    _ProductDetailBLL.AddProductDetail(model);
                }

                new AuditLogBLL(_unitOfWork).AddAuditLog("Product", "UpdateProductDetail", "End Click on UpdateProductDetail Button of ");
                jsonResponse.Status = true;
                jsonResponse.Message = NotificationMessage.SaveSuccessfully;
                jsonResponse.RedirectURL = Url.Action("ProductMapping", "Product");
                return Json(new { data = jsonResponse });
            }
            catch (Exception ex)
            {
                new ErrorLogBLL(_unitOfWork).AddExceptionLog(ex);
                jsonResponse.Status = true;
                jsonResponse.Message = NotificationMessage.ErrorOccurred;
                jsonResponse.RedirectURL = Url.Action("ProductMapping", "Product");
                return Json(new { data = jsonResponse });
            }
        }
        [HttpGet]
        public JsonResult GetProduct(int Id)
        {
            new AuditLogBLL(_unitOfWork).AddAuditLog("Product", "GetProduct", "Get Product ");
            ProductMaster productMaster = _ProductMasterBLL.GetProductMasterById(Id);
            return Json(new { productMaster });
        }

    }
}
