using BusinessLogicLayer.Application;
using BusinessLogicLayer.ErrorLog;
using BusinessLogicLayer.HelperClasses;
using DataAccessLayer.WorkProcess;
using DistributorPortal.Resource;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Models.Application;
using Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utility;
using Utility.Constant;
using Utility.HelperClasses;

namespace DistributorPortal.Controllers
{
    public class DummyOrderFormController : Controller
    {
        private ProductDetailBLL _productDetailBLL;
        private IUnitOfWork _unitOfWork;
        private OrderBLL _OrderBLL;
        private IConfiguration _IConfiguration;
        private Configuration _Configuration;
        public DummyOrderFormController(IUnitOfWork unitOfWork, Configuration _configuration, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _OrderBLL = new OrderBLL(_unitOfWork);
            _productDetailBLL = new ProductDetailBLL(unitOfWork);
            _IConfiguration = configuration;
            _Configuration = _configuration;
        }
        public IActionResult Index()
        {
            var model = _productDetailBLL.GetAllProductDetail();
            model.ForEach(x => x.PendingQuantity = SessionHelper.SAPOrderPendingQuantity.FirstOrDefault(y => y.ProductCode == x.ProductMaster.SAPProductCode) != null ? Math.Floor(Convert.ToDouble(SessionHelper.SAPOrderPendingQuantity.FirstOrDefault(z => z.ProductCode == x.ProductMaster.SAPProductCode).PendingQuantity)).ToString() : "0");

            return View(model);
        }

        public IActionResult Dummy()
        {
            OrderViewModel model = new OrderViewModel();
            model.ProductDetails = _productDetailBLL.GetAllProductDetail();
            if (SessionHelper.SAPOrderPendingQuantity != null)
            {
                model.ProductDetails.ForEach(x => x.PendingQuantity = SessionHelper.SAPOrderPendingQuantity.FirstOrDefault(y => y.ProductCode == x.ProductMaster.SAPProductCode) != null ? Math.Floor(Convert.ToDouble(SessionHelper.SAPOrderPendingQuantity.FirstOrDefault(z => z.ProductCode == x.ProductMaster.SAPProductCode).PendingQuantity)).ToString() : "0");
            }
            SessionHelper.AddProduct = model.ProductDetails;
            return View(model);
        }

        public ActionResult ApprovedOrderValue(int Product, int Quantity)
        {
            var list = SessionHelper.AddProduct;
            list.First(e => e.ProductMasterId == Product).ProductMaster.Quantity = Quantity;
            SessionHelper.AddProduct = list;
            var OrderVal = _OrderBLL.GetOrderValueModel(SessionHelper.AddProduct);
            return PartialView("OrderValue", OrderVal);
        }

        [HttpPost]
        public JsonResult SaveEdit(OrderViewModel model, SubmitStatus btnSubmit)
        {
            JsonResponse jsonResponse = new JsonResponse();
            OrderMaster master = new OrderMaster();
            master.productDetails = model.ProductDetails.Where(e => e.ProductMaster.Quantity != 0).ToList();
            master.ReferenceNo = model.ReferenceNo;
            master.Remarks = model.Remarks;
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
                    if (SessionHelper.AddProduct.Count > 0)
                    {
                        if (btnSubmit == SubmitStatus.Draft)
                        {
                            master.Status = OrderStatus.Draft;
                        }
                        else
                        {
                            master.Status = OrderStatus.PendingApproval;
                        }
                        jsonResponse = _OrderBLL.Save(master, _IConfiguration, Url);
                    }
                    else
                    {
                        jsonResponse.Status = false;
                        jsonResponse.Message = Common.OrderContant.OrderItem;
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
    }
}
