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
using Utility.HelperClasses;

namespace DistributorPortal.Controllers
{
    public class ReportsController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly OrderBLL _OrderBLL;
        private readonly OrderDetailBLL _OrderDetailBLL;
        private readonly OrderReturnBLL _OrderReturnBLL;
        private readonly OrderReturnDetailBLL _OrderReturnDetailBLL;
        private readonly PaymentBLL _PaymentBLL;
        private readonly ComplaintBLL _ComplaintBLL;
        public ReportsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _OrderBLL = new OrderBLL(_unitOfWork);
            _OrderDetailBLL = new OrderDetailBLL(_unitOfWork);
            _OrderReturnBLL = new OrderReturnBLL(_unitOfWork);
            _OrderReturnDetailBLL = new OrderReturnDetailBLL(_unitOfWork);
            _PaymentBLL = new PaymentBLL(_unitOfWork);
            _ComplaintBLL = new ComplaintBLL(_unitOfWork);

        }

        #region Order
        public ActionResult Order()
        {
            return View();
        }
        public List<OrderMaster> GetOrderList(OrderSearch model)
        {
            List<OrderMaster> list = _OrderBLL.Search(model).Where(x => SessionHelper.LoginUser.IsDistributor == true ? x.DistributorId == SessionHelper.LoginUser.DistributorId : true).ToList();
            return list;
        }
        [HttpPost]
        public IActionResult OrderSearch(OrderSearch model, string Search)
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
            var Detail = _OrderDetailBLL.GetOrderDetailByIdByMasterId(OrderId);
            return PartialView("OrderDetailList", Detail);
        }
        #endregion

        #region OrderReturn
        public ActionResult OrderReturn()
        {
            return View();
        }
        public List<OrderReturnMaster> GetOrderReturnList(OrderReturnSearch model)
        {
            List<OrderReturnMaster> list = _OrderReturnBLL.SearchReport(model).Where(x => SessionHelper.LoginUser.IsDistributor == true ? x.DistributorId == SessionHelper.LoginUser.DistributorId : true).ToList();
            return list;
        }
        [HttpPost]
        public IActionResult OrderReturnSearch(OrderReturnSearch model, string Search)
        {
            if (Search == "Search")
            {
                return PartialView("OrderReturnList", GetOrderReturnList(model));
            }
            else
            {
                return PartialView("OrderReturnList", new OrderMaster());
            }
        }
        [HttpGet]
        public ActionResult GetOrderReturnDetailList(string DPID)
        {
            int id;
            int.TryParse(EncryptDecrypt.Decrypt(DPID), out id);
            var Detail = _OrderReturnDetailBLL.GetOrderDetailByIdByMasterId(id);
            return PartialView("OrderReturnDetailList", Detail);
        }
        #endregion

        #region Payment
        public ActionResult Payment()
        {
            return View();
        }
        public List<PaymentMaster> GetPaymentList(PaymentSearch model)
        {
            List<PaymentMaster> list = _PaymentBLL.SearchReport(model).Where(x => SessionHelper.LoginUser.IsDistributor == true ? x.DistributorId == SessionHelper.LoginUser.DistributorId : true).ToList();
            return list;
        }
        [HttpPost]
        public IActionResult PaymentSearch(PaymentSearch model, string Search)
        {
            if (Search == "Search")
            {
                return PartialView("PaymentList", GetPaymentList(model));
            }
            else
            {
                return PartialView("PaymentList", new PaymentMaster());
            }
        }
        #endregion

        #region Complain
        public ActionResult Complaint()
        {
            return View();
        }
        public List<Complaint> GetComplaintList(ComplaintSearch model)
        {
            List<Complaint> list = _ComplaintBLL.SearchReport(model).Where(x => SessionHelper.LoginUser.IsDistributor == true ? x.DistributorId == SessionHelper.LoginUser.DistributorId : true).ToList();
            return list;
        }
        [HttpPost]
        public IActionResult ComplaintSearch(ComplaintSearch model, string Search)
        {
            if (Search == "Search")
            {
                return PartialView("ComplaintList", GetComplaintList(model));
            }
            else
            {
                return PartialView("ComplaintList", new Complaint());
            }
        }
        #endregion
    }
}
