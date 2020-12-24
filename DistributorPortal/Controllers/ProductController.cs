using BusinessLogicLayer.Application;
using BusinessLogicLayer.ErrorLog;
using DataAccessLayer.WorkProcess;
using DistributorPortal.Resource;
using Microsoft.AspNetCore.Mvc;
using Models.Application;
using Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductPortal.Controllers
{
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ProductMasterBLL _ProductMasterBLL;
        private readonly ProductDetailBLL _ProductDetailBLL;
        public ProductController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _ProductMasterBLL = new ProductMasterBLL(_unitOfWork);
            _ProductDetailBLL = new ProductDetailBLL(_unitOfWork);
        }
        // GET: Product
        public ActionResult Index()
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
            List<int> ProductCodes = _ProductMasterBLL.GetAllProductMaster().Select(x => x.SAPProductCode).ToList();
            List<ProductMaster> ProductList = new List<ProductMaster>();
            foreach (var item in ProductList)
            {
                if (ProductCodes.Contains(item.SAPProductCode))
                {
                    _ProductMasterBLL.Update(item);
                }
                else
                {
                    _ProductMasterBLL.Add(item);
                }
            }
            return PartialView("List", _ProductMasterBLL.GetAllProductMaster());
        }
        [HttpGet]
        public ActionResult ProductMapping()
        {
            List<ProductDetail> productDetails = _ProductDetailBLL.GetAllProductDetail();
            List<ProductMaster> productMasters = _ProductMasterBLL.GetAllProductMaster();
            productMasters.ForEach(x => x.ProductDetail = productDetails.Where(y=>y.ProductMasterId == x.Id).FirstOrDefault() ?? new ProductDetail());
            return View("ProductMapping", productMasters);
        }
        [HttpPost]
        public ActionResult UpdateProductDetail(ProductDetail model)
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
        public JsonResult GetProduct(int Id) 
        {
             ProductMaster productMaster = _ProductMasterBLL.GetProductMasterById(Id);
            return Json(new { productMaster });
        }

    }
}
