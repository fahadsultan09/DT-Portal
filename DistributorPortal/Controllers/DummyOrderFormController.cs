using BusinessLogicLayer.Application;
using BusinessLogicLayer.HelperClasses;
using DataAccessLayer.WorkProcess;
using Microsoft.AspNetCore.Mvc;
using Models.Application;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DistributorPortal.Controllers
{
    public class DummyOrderFormController : Controller
    {
        private readonly ProductDetailBLL _productDetailBLL;
        private readonly IUnitOfWork _unitOfWork;
        private readonly OrderBLL _OrderBLL;
        public DummyOrderFormController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _OrderBLL = new OrderBLL(_unitOfWork);
            _productDetailBLL = new ProductDetailBLL(unitOfWork);
        }
        public IActionResult Index()
        {
            var model = _productDetailBLL.GetAllProductDetail();
            model.ForEach(x => x.PendingQuantity = SessionHelper.SAPOrderPendingQuantity.FirstOrDefault(y => y.ProductCode == x.ProductMaster.SAPProductCode) != null ? Math.Floor(Convert.ToDouble(SessionHelper.SAPOrderPendingQuantity.FirstOrDefault(z => z.ProductCode == x.ProductMaster.SAPProductCode).PendingQuantity)).ToString() : "0");

            return View(model);
        }

        public IActionResult Dummy()
        {
            var model = _productDetailBLL.GetAllProductDetail();
            model.ForEach(x => x.PendingQuantity = SessionHelper.SAPOrderPendingQuantity.FirstOrDefault(y => y.ProductCode == x.ProductMaster.SAPProductCode) != null ? Math.Floor(Convert.ToDouble(SessionHelper.SAPOrderPendingQuantity.FirstOrDefault(z => z.ProductCode == x.ProductMaster.SAPProductCode).PendingQuantity)).ToString() : "0");
            SessionHelper.AddProduct = model;
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
        public IActionResult SaveEdit(List<ProductDetail> model, OrderMaster master)
        {
            return View();
        }
    }
}
