using BusinessLogicLayer.Application;
using BusinessLogicLayer.ErrorLog;
using BusinessLogicLayer.HelperClasses;
using DataAccessLayer.WorkProcess;
using DistributorPortal.BusinessLogicLayer.ApplicationSetup;
using DistributorPortal.Resource;
using Microsoft.AspNetCore.Mvc;
using Models.Application;
using Models.ViewModel;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utility.HelperClasses;

namespace ProductPortal.Controllers
{
    public class ProductController : Controller
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
            new AuditTrailBLL(_unitOfWork).AddAuditTrail("Product", "Index", " Form");
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
                new AuditTrailBLL(_unitOfWork).AddAuditTrail("Product", "Sync", "Start Click on Sync Button of ");
                var Client = new RestClient(_configuration.SyncProductURL);
                var request = new RestRequest(Method.GET);
                IRestResponse response = Client.Execute(request);
                var SAPProduct = JsonConvert.DeserializeObject<List<ProductMaster>>(response.Content);
                var allproduct = _ProductMasterBLL.GetAllProductMaster();
                var addProduct = SAPProduct.Where(e => !allproduct.Any(c => c.SAPProductCode == e.SAPProductCode)).ToList();
                addProduct.ForEach(e =>
                {
                    e.CreatedBy = SessionHelper.LoginUser.Id;
                    e.IsDeleted = false;
                    e.IsActive = true; 
                    e.CreatedDate = DateTime.Now;
                });
                _ProductMasterBLL.AddRange(addProduct);
                new AuditTrailBLL(_unitOfWork).AddAuditTrail("Product", "Sync", "End Click on Sync Button of ");
            }
            catch (Exception ex)
            {
                new ErrorLogBLL(_unitOfWork).AddExceptionLog(ex);
            }
            return PartialView("List", _ProductMasterBLL.GetAllProductMaster());
        }
        [HttpGet]
        public IActionResult ProductMapping()
        {
            new AuditTrailBLL(_unitOfWork).AddAuditTrail("Product", "ProductMapping", "Get Prodct Mapping ");
            List<ProductDetail> productDetails = _ProductDetailBLL.GetAllProductDetail();
            List<ProductMaster> productMasters = _ProductMasterBLL.GetAllProductMaster();
            productMasters.ForEach(x => x.ProductDetail = productDetails.Where(y=>y.ProductMasterId == x.Id).FirstOrDefault() ?? new ProductDetail());
            return View("ProductMapping", productMasters);
        }
        [HttpPost]
        public IActionResult UpdateProductDetail(ProductDetail model)
        {
            JsonResponse jsonResponse = new JsonResponse();
            try
            {
                new AuditTrailBLL(_unitOfWork).AddAuditTrail("Product", "UpdateProductDetail", "Start Click on UpdateProductDetail Button of ");
                if (model.Id > 0)
                {
                    _ProductDetailBLL.UpdateProductDetail(model);
                }
                else
                {
                    _ProductDetailBLL.AddProductDetail(model);
                }

                new AuditTrailBLL(_unitOfWork).AddAuditTrail("Product", "UpdateProductDetail", "End Click on UpdateProductDetail Button of ");
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
            new AuditTrailBLL(_unitOfWork).AddAuditTrail("Product", "GetProduct", "Get Product ");
            ProductMaster productMaster = _ProductMasterBLL.GetProductMasterById(Id);
            return Json(new { productMaster });
        }

    }
}
