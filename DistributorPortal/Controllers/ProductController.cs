using BusinessLogicLayer.Application;
using BusinessLogicLayer.ErrorLog;
using BusinessLogicLayer.HelperClasses;
using DataAccessLayer.WorkProcess;
using DistributorPortal.Controllers;
using DistributorPortal.Resource;
using Fingers10.ExcelExport.ActionResults;
using Microsoft.AspNetCore.Mvc;
using Models.Application;
using Models.ViewModel;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Utility;
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
            List<ProductMaster> SAPProduct = new List<ProductMaster>();
            try
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    string authInfo = Convert.ToBase64String(Encoding.Default.GetBytes("sami_po:wasay123"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authInfo);
                    var result = client.GetAsync(new Uri("http://10.0.3.35:51000/RESTAdapter/getHRMS")).Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var JsonContent = result.Content.ReadAsStringAsync().Result;
                        Root root = (Root)JsonConvert.DeserializeObject(JsonContent);

                    }
                }

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
            List<ProductDetail> productDetails = _ProductDetailBLL.GetAllProductDetail();
            List<ProductMaster> productMasters = _ProductMasterBLL.GetAllProductMaster();
            productMasters.ForEach(x => x.ProductDetail = productDetails.Where(y => y.ProductMasterId == x.Id).FirstOrDefault() ?? new ProductDetail());
            return View("ProductMapping", productMasters);
        }

        [HttpGet]
        public PartialViewResult GetMappedUnmappedProduct(ProductEnum productEnum)
        {
            List<ProductDetail> productDetails = _ProductDetailBLL.GetAllProductDetail();
            List<ProductMaster> productMasters = _ProductMasterBLL.GetAllProductMaster();
            productMasters.ForEach(x => x.ProductDetail = productDetails.Where(y => y.ProductMasterId == x.Id).FirstOrDefault() ?? new ProductDetail());
            ViewBag.productEnum = (int)productEnum;

            if (productEnum == ProductEnum.ProductMapping)
            {
                productMasters = _ProductMasterBLL.Where(x => productDetails.Select(y => y.ProductMasterId).Contains(x.Id)).ToList();
            }
            else if (productEnum == ProductEnum.ProductMaster)
            {
                productMasters = _ProductMasterBLL.Where(x => !productDetails.Select(y => y.ProductMasterId).Contains(x.Id)).ToList();
            }
            else
            {
                productMasters.ForEach(x => x.ProductDetail = productDetails.Where(y => y.ProductMasterId == x.Id).FirstOrDefault() ?? new ProductDetail());    
            }
            return PartialView("PMList", productMasters);
        }
        [HttpPost]
        public IActionResult UpdateProductDetail(ProductDetail model)
        {
            JsonResponse jsonResponse = new JsonResponse();
            try
            {
                if (model.Id > 0)
                {
                    _ProductDetailBLL.UpdateProductDetail(model);
                }
                else
                {
                    _ProductDetailBLL.AddProductDetail(model);
                }

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
        public JsonResult GetProduct(string DPID)
        {
            int.TryParse(EncryptDecrypt.Decrypt(DPID), out int id);
            ProductMaster productMaster = _ProductMasterBLL.GetProductMasterById(id);
            return Json(new { productMaster });
        }

        public IActionResult ProductsExportToExcel(ProductEnum Id)
        {
            if (Id == ProductEnum.ProductMaster)
            {
                var data = _ProductMasterBLL.GetAllProductMaster();
                return new ExcelResult<ProductMaster>(data, "Products", "Product List_" + DateTime.Now.ToString("dd-MM-yyyy"));
            }
            else
            {
                var data = _ProductDetailBLL.GetViewModelForExcel();
                return new ExcelResult<ProductMappingModel>(data, "Product Mapping", "Product List" + DateTime.Now.ToString("dd-MM-yyyy"));
            }
        }
    }
}
