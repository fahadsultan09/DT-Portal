using BusinessLogicLayer.Application;
using BusinessLogicLayer.HelperClasses;
using DataAccessLayer.WorkProcess;
using DistributorPortal.BusinessLogicLayer.ApplicationSetup;
using Microsoft.AspNetCore.Mvc;
using Models.Application;
using Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DistributorPortal.Controllers
{
    public class ReportsController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly OrderBLL _OrderBLL;
        private readonly OrderDetailBLL _orderDetailBLL;
        private readonly ProductDetailBLL _productDetailBLL;
        public ReportsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _OrderBLL = new OrderBLL(_unitOfWork);
            _productDetailBLL = new ProductDetailBLL(_unitOfWork);
            _orderDetailBLL = new OrderDetailBLL(_unitOfWork);

        }
        public ActionResult Order()
        {
            return View();
        }
        public List<OrderMaster> GetOrderList(OrderSearch model)
        {
            List<OrderMaster>  list = _OrderBLL.Search(model).Where(x => SessionHelper.LoginUser.IsDistributor == true ? x.DistributorId == SessionHelper.LoginUser.DistributorId : true).ToList();
            return list;
        }
        [HttpPost]
        public IActionResult Search(OrderSearch model, string Search)
        {
            if (Search == "Search")
            {
                return PartialView("OrderList", GetOrderList(model));
            }
            else
            {
                return PartialView("OrderList", new OrderMaster());
            }
        }
        [HttpGet]
        public ActionResult GetOrderDetailList(int OrderId)
        {
            var Detail = _orderDetailBLL.GetOrderDetailByIdByGatePassMasterId(OrderId);
            return PartialView("OrderDetailList", Detail);
        }
    }
}
