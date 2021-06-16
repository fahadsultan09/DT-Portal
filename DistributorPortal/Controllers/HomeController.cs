using BusinessLogicLayer.Application;
using BusinessLogicLayer.ApplicationSetup;
using BusinessLogicLayer.ErrorLog;
using BusinessLogicLayer.GeneralSetup;
using BusinessLogicLayer.HelperClasses;
using DataAccessLayer.WorkProcess;
using DistributorPortal.Resource;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.FileProviders;
using Models.Application;
using Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Xml;
using Utility;
using Utility.HelperClasses;

namespace DistributorPortal.Controllers
{
    public class HomeController : BaseController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly OrderBLL _OrderBLL;
        private readonly OrderDetailBLL _OrderDetailBLL;
        private readonly OrderReturnBLL _OrderReturnBLL;
        private readonly PaymentBLL _PaymentBLL;
        private readonly ComplaintBLL _ComplaintBLL;
        private readonly ProductMasterBLL _ProductMasterBLL;
        private readonly DistributorBLL _DistributorBLL;
        private readonly PolicyBLL _PolicyBLL;
        private readonly NotificationBLL _NotificationBLL;
        private List<PaymentMaster> _PaymentMaster;
        private List<OrderMaster> _OrderMaster;
        private List<OrderDetail> _OrderDetail;
        private List<OrderReturnMaster> _OrderReturnMaster;
        private List<Complaint> _Complaint;
        private List<ProductMaster> _ProductMaster;
        private List<Distributor> _Distributor;
        private List<Notification> _Notification;
        private readonly Configuration _configuration;
        public HomeController(IUnitOfWork unitOfWork, Configuration configuration)
        {
            _unitOfWork = unitOfWork;
            _OrderBLL = new OrderBLL(_unitOfWork);
            _OrderDetailBLL = new OrderDetailBLL(_unitOfWork);
            _PaymentBLL = new PaymentBLL(_unitOfWork);
            _ComplaintBLL = new ComplaintBLL(_unitOfWork);
            _ProductMasterBLL = new ProductMasterBLL(_unitOfWork);
            _DistributorBLL = new DistributorBLL(_unitOfWork);
            _OrderReturnBLL = new OrderReturnBLL(_unitOfWork);
            _PolicyBLL = new PolicyBLL(_unitOfWork);
            _NotificationBLL = new NotificationBLL(_unitOfWork);
            _PaymentMaster = _PaymentBLL.GetAllPaymentMaster().ToList();
            _OrderMaster = _OrderBLL.GetAllOrderMaster().ToList();
            _OrderDetail = _OrderDetailBLL.GetAllOrderDetail().ToList();
            _OrderReturnMaster = _OrderReturnBLL.GetAllOrderReturn().ToList();
            _Complaint = _ComplaintBLL.GetAllComplaint().ToList();
            _ProductMaster = _ProductMasterBLL.GetAllProductMaster().ToList();
            _Distributor = _DistributorBLL.GetAllDistributor().ToList();
            _Notification = _NotificationBLL.GetAllNotification().Where(x => x.CreatedDate.Date >= DateTime.Now.Date.AddDays(-10)).OrderByDescending(x => x.CreatedDate).ToList();
            _Notification.ForEach(x => x.RelativeTime = ExtensionUtility.TimeAgo(x.CreatedDate));
            _configuration = configuration;
        }
        public IActionResult Index()
        {
            new AuditLogBLL(_unitOfWork).AddAuditLog("Home", "Index", "Dashboard");

            var dashboard = SessionHelper.NavigationMenu.FirstOrDefault(x => (x.ApplicationPage.ControllerName == "Home" || x.ApplicationPage.ControllerName == "StoreKeeperDashboard") && x.ApplicationPage.PageTitle != "Get File");
            if (dashboard != null)
            {
                return RedirectToAction(dashboard.ApplicationPage.PageURL.Split('/')[2]);
            }
            else
            {
                return View();
            }
        }
        #region Admin
        public IActionResult AdminDashboard()
        {
            try
            {
                AdminDashboardViewModel model = new AdminDashboardViewModel();
                model.Complaint = _Complaint.Where(x => x.Status == ComplaintStatus.Pending && x.Status == ComplaintStatus.Resolved).Count();
                model.PendingApproval = _OrderMaster.Where(x => x.Status == OrderStatus.PendingApproval).Count();
                model.Approved = _OrderMaster.Where(x => x.Status == OrderStatus.Approved).Count();
                model.InProcess = _OrderDetail.Where(x => x.OrderProductStatus == OrderStatus.InProcess).Count();
                model.PartiallyProcessed = _OrderDetail.Where(x => x.OrderProductStatus == OrderStatus.PartiallyProcessed).Count();
                model.CompletelyProcessed = _OrderDetail.Where(x => x.OrderProductStatus == OrderStatus.CompletelyProcessed).Count();
                model.OnHold = _OrderMaster.Where(x => x.Status == OrderStatus.Onhold).Count();
                model.Reject = _OrderMaster.Where(x => x.Status == OrderStatus.Reject).Count();
                model.PendingOrder = model.PendingApproval + model.InProcess + model.PartiallyProcessed;
                model.VerifiedPayment = _PaymentMaster.Where(x => x.Status == PaymentStatus.Verified).Sum(x => x.Amount);
                model.UnverifiedPayment = _PaymentMaster.Where(x => x.Status == PaymentStatus.Unverified).Sum(x => x.Amount);
                model.ReturnOrder = _OrderReturnMaster.Where(x => x.Status == OrderReturnStatus.Submitted && x.Status == OrderReturnStatus.PartiallyReceived).Count();
                return View(model);
            }
            catch (Exception ex)
            {
                new ErrorLogBLL(_unitOfWork).AddExceptionLog(ex);
                return View();
            }
        }
        public IActionResult GetAdminPaymentWiseComparision()
        {
            DateTime CurrentYear = DateTime.Now;
            DateTime LastYear = DateTime.Now.AddYears(-1).AddMonths(1);
            var months = ExtensionUtility.MonthsBetween(LastYear, CurrentYear);
            List<OrderMaster> orderMasters = _OrderMaster.Where(x => x.Status != OrderStatus.Draft && x.Status != OrderStatus.Reject && x.Status != OrderStatus.Cancel).ToList();
            List<PaymentWiseComparision> PaymentWiseComparision = new List<PaymentWiseComparision>();
            foreach (var item in months.Item1)
            {
                PaymentWiseComparision PWSCurrent = new PaymentWiseComparision();
                PWSCurrent.Month = item.MonthName;
                PWSCurrent.CurrentYear = orderMasters.Where(x => x.CreatedDate.Year == item.Year && x.CreatedDate.Month == item.Month).Sum(x => x.TotalValue);
                PWSCurrent.LastYear = orderMasters.Where(x => x.CreatedDate.Year == item.LastYear && x.CreatedDate.Month == item.Month).Sum(x => x.TotalValue);
                PaymentWiseComparision.Add(PWSCurrent);
            }
            return PartialView("AdminPaymentWiseComparision", PaymentWiseComparision);
        }
        public IActionResult GetAdminOrderWiseComparision()
        {
            DateTime CurrentYear = DateTime.Now;
            DateTime LastYear = DateTime.Now.AddYears(-1).AddMonths(1);
            var months = ExtensionUtility.MonthsBetween(LastYear, CurrentYear);
            List<OrderMaster> orderMasters = _OrderMaster.Where(x => x.Status != OrderStatus.Draft && x.Status != OrderStatus.Reject && x.Status != OrderStatus.Cancel).ToList();
            List<OrderWiseComparision> OrderWiseComparision = new List<OrderWiseComparision>();
            foreach (var item in months.Item1)
            {
                OrderWiseComparision OWSCurrent = new OrderWiseComparision();
                OWSCurrent.Month = item.MonthName;
                OWSCurrent.CurrentYear = orderMasters.Where(x => x.CreatedDate.Year == item.Year && x.CreatedDate.Month == item.Month).Count();
                OWSCurrent.LastYear = orderMasters.Where(x => x.CreatedDate.Year == item.LastYear && x.CreatedDate.Month == item.Month).Count();
                OrderWiseComparision.Add(OWSCurrent);
            }
            return PartialView("AdminOrderWiseComparision", OrderWiseComparision);
        }
        public IActionResult GetAdminPaymentWiseStatus()
        {
            DateTime CurrentYear = DateTime.Now;
            DateTime LastYear = DateTime.Now.AddYears(-1).AddMonths(1);
            var months = ExtensionUtility.MonthsBetween(LastYear, CurrentYear);

            List<PaymentWiseStatus> PaymentWiseStatus = new List<PaymentWiseStatus>();
            foreach (var item in months.Item1)
            {
                PaymentWiseStatus PWSCurrent = new PaymentWiseStatus();
                PWSCurrent.Month = item.MonthName;
                PWSCurrent.VerifiedPayment = _PaymentMaster.Where(x => x.CreatedDate.Year == item.Year && x.CreatedDate.Month == item.Month && x.Status == PaymentStatus.Verified).Sum(x => x.Amount);
                PWSCurrent.UnverifiedPayment = _PaymentMaster.Where(x => x.CreatedDate.Year == item.Year && x.CreatedDate.Month == item.Month && x.Status == PaymentStatus.Unverified).Sum(x => x.Amount);
                PaymentWiseStatus.Add(PWSCurrent);
            }
            return PartialView("AdminPaymentWiseStatus", PaymentWiseStatus);
        }
        public IActionResult GetAdminRegionWiseOrder()
        {
            DateTime CurrentYear = DateTime.Now;
            DateTime LastYear = DateTime.Now.AddYears(-1).AddMonths(1);
            var months = ExtensionUtility.MonthsBetween(LastYear, CurrentYear);
            List<RegionWiseOrder> RegionWiseOrder = new List<RegionWiseOrder>();
            List<OrderMaster> orderMasters = _OrderMaster.Where(x => x.Status != OrderStatus.Draft && x.Status != OrderStatus.Reject && x.Status != OrderStatus.Cancel).ToList();

            RegionWiseOrder = orderMasters.GroupBy(x => x.Distributor.Region.RegionName).Select(y => new RegionWiseOrder
            {
                Region = y.Key.ToString(),
                OrderCount = y.Count()
            }).ToList();

            return PartialView("AdminRegionWiseOrder", RegionWiseOrder);
        }
        public IActionResult GetAdminRecentPaymentStatus()
        {
            AdminRecentPaymentStatus model = new AdminRecentPaymentStatus();
            DateTime CurrentYear = DateTime.Now;
            DateTime LastYear = DateTime.Now.AddYears(-1).AddMonths(1);
            var months = ExtensionUtility.MonthsBetween(LastYear, CurrentYear);
            List<OrderMaster> orderMasters = _OrderMaster.Where(x => x.Status != OrderStatus.Draft && x.Status != OrderStatus.Reject && x.Status != OrderStatus.Cancel).ToList();
            model.DistributorViewModelOrder = new List<DistributorViewModel>();
            model.DistributorViewModelOrder = orderMasters.GroupBy(x => x.DistributorId).Select(y => new DistributorViewModel
            {
                DistributorName = _Distributor.FirstOrDefault(x => x.Id == y.Key).DistributorName,
                OrderCount = y.Count()
            }).Take(5).ToList();

            model.DistributorViewModelPayment = new List<DistributorViewModel>();
            model.DistributorViewModelPayment = _PaymentMaster.Where(x => x.Status == PaymentStatus.Verified).GroupBy(x => x.DistributorId).Select(y => new DistributorViewModel
            {
                DistributorName = _Distributor.FirstOrDefault(x => x.Id == y.Key).DistributorName,
                Payment = y.Sum(x => x.Amount)
            }).Take(5).ToList();

            model.ProductViewModelOrder = new List<ProductViewModel>();
            model.ProductViewModelOrder = _OrderDetail.GroupBy(x => x.ProductId).Select(y => new ProductViewModel
            {
                ProductName = _ProductMaster.FirstOrDefault(x => x.Id == y.Key).ProductName,
                CurrentPrice = _ProductMaster.FirstOrDefault(x => x.Id == y.Key).ProductPrice,
                OrderCount = y.Count()
            }).Take(5).ToList();

            model.ProductViewModelQuantity = new List<ProductViewModel>();
            model.ProductViewModelQuantity = _OrderDetail.GroupBy(x => x.ProductId).Select(y => new ProductViewModel
            {
                ProductName = _ProductMaster.FirstOrDefault(x => x.Id == y.Key).ProductName,
                CurrentPrice = _ProductMaster.FirstOrDefault(x => x.Id == y.Key).ProductPrice,
                Quantity = y.Sum(x => x.Quantity)
            }).Take(5).ToList();

            return PartialView("AdminRecentPaymentStatus", model);
        }
        #endregion

        #region Accounts
        public IActionResult AccountDashboard()
        {
            try
            {
                AccountDashboardViewModel model = new AccountDashboardViewModel();

                model.VerifiedPayment = ExtensionUtility.FormatNumberAmount(_PaymentMaster.Where(x => x.Status == PaymentStatus.Verified).Sum(x => x.Amount));
                model.UnverifiedPaymentCount = _PaymentMaster.Where(x => x.Status == PaymentStatus.Unverified).Count();
                model.UnverifiedPayment = ExtensionUtility.FormatNumberAmount(_PaymentMaster.Where(x => x.Status == PaymentStatus.Unverified).Sum(x => x.Amount));
                model.TodayVerifiedPayment = ExtensionUtility.FormatNumberAmount(_PaymentMaster.Where(x => x.Status == PaymentStatus.Verified && x.CreatedDate.Date == DateTime.Now.Date).Sum(x => x.Amount));

                return View(model);
            }
            catch (Exception ex)
            {
                new ErrorLogBLL(_unitOfWork).AddExceptionLog(ex);
                return View();
            }
        }
        public IActionResult GetAccountPaymentWiseComparision()
        {
            DateTime CurrentYear = DateTime.Now;
            DateTime LastYear = DateTime.Now.AddYears(-1).AddMonths(1);
            var months = ExtensionUtility.MonthsBetween(LastYear, CurrentYear);
            List<PaymentMaster> paymentMasters = _PaymentMaster.Where(x => x.Status == PaymentStatus.Verified).ToList();
            List<PaymentWiseComparision> PaymentWiseComparision = new List<PaymentWiseComparision>();
            foreach (var item in months.Item1)
            {
                PaymentWiseComparision PWSCurrent = new PaymentWiseComparision();
                PWSCurrent.Month = item.MonthName;
                PWSCurrent.CurrentYear = paymentMasters.Where(x => x.CreatedDate.Year == item.Year && x.CreatedDate.Month == item.Month).Sum(x => x.Amount);
                PWSCurrent.LastYear = paymentMasters.Where(x => x.CreatedDate.Year == item.LastYear && x.CreatedDate.Month == item.Month).Sum(x => x.Amount);
                PaymentWiseComparision.Add(PWSCurrent);
            }
            return PartialView("AccountPaymentWise", PaymentWiseComparision);
        }
        public IActionResult GetAccountPaymentWiseStatus()
        {
            DateTime CurrentYear = DateTime.Now;
            DateTime LastYear = DateTime.Now.AddYears(-1).AddMonths(1);
            var months = ExtensionUtility.MonthsBetween(LastYear, CurrentYear);
            List<PaymentMaster> paymentMasters = _PaymentMaster.ToList();
            List<PaymentWiseStatus> PaymentWiseStatus = new List<PaymentWiseStatus>();
            foreach (var item in months.Item1)
            {
                PaymentWiseStatus PWSCurrent = new PaymentWiseStatus();
                PWSCurrent.Month = item.MonthName;
                PWSCurrent.VerifiedPayment = paymentMasters.Where(x => x.CreatedDate.Year == item.Year && x.CreatedDate.Month == item.Month && x.Status == PaymentStatus.Verified).Sum(x => x.Amount);
                PWSCurrent.UnverifiedPayment = paymentMasters.Where(x => x.CreatedDate.Year == item.Year && x.CreatedDate.Month == item.Month && x.Status == PaymentStatus.Unverified).Sum(x => x.Amount);
                PaymentWiseStatus.Add(PWSCurrent);
            }

            return PartialView("AccountPaymentWiseStatus", PaymentWiseStatus);
        }
        public IActionResult GetAccountRecentPaymentStatus()
        {
            AccountRecentPaymentStatus model = new AccountRecentPaymentStatus();
            DateTime CurrentYear = DateTime.Now;
            DateTime LastYear = DateTime.Now.AddYears(-1).AddMonths(1);
            var months = ExtensionUtility.MonthsBetween(LastYear, CurrentYear);
            List<PaymentMaster> paymentMasters = _PaymentMaster.Where(x => x.Status == PaymentStatus.Verified).ToList();
            model.PaymentWiseAmount = new List<PaymentWiseAmount>();
            model.PaymentWiseAmount = paymentMasters.Where(x => x.Status == PaymentStatus.Verified).GroupBy(x => x.PaymentMode.PaymentName).Select(y => new PaymentWiseAmount
            {
                PaymentMode = y.Key.ToString() + " " + ExtensionUtility.FormatNumberAmount(y.Sum(x => x.Amount)),
                Amount = y.Sum(x => x.Amount)
            }).ToList();

            model.RecentPayment = new List<RecentPayment>();
            model.RecentPayment = paymentMasters.OrderByDescending(x => x.CreatedDate).Select(y => new RecentPayment
            {

                Id = y.Id,
                PaymentId = y.SNo,
                PaymentMode = y.PaymentMode.PaymentName,
                DistributorName = y.Distributor.DistributorName,
                Status = y.Status,
                Amount = y.Amount
            }).Take(5).ToList();

            return PartialView("AccountRecentPaymentStatus", model);
        }
        #endregion

        #region Distributor
        public IActionResult DistributorDashboard()
        {
            try
            {
                DistributorDashboardViewModel model = new DistributorDashboardViewModel();
                List<OrderMaster> OrderMasterList = _OrderMaster.Where(x => x.DistributorId == SessionHelper.LoginUser.DistributorId && x.Status != OrderStatus.Cancel).ToList();
                List<PaymentMaster> paymentMasters = _PaymentMaster.Where(x => x.DistributorId == SessionHelper.LoginUser.DistributorId).ToList();
                _Complaint = _Complaint.Where(x => x.DistributorId == SessionHelper.LoginUser.DistributorId).ToList();
                _OrderReturnMaster = _OrderReturnMaster.Where(x => x.DistributorId == SessionHelper.LoginUser.DistributorId).ToList();

                model.UnverifiedPaymentAllCount = paymentMasters.Where(x => x.Status == PaymentStatus.Unverified).Count();
                model.UnverifiedPaymentAll = ExtensionUtility.FormatNumberAmount(paymentMasters.Where(x => x.CreatedDate.Year == DateTime.Now.Year && x.Status == PaymentStatus.Unverified).Sum(x => x.Amount));

                model.InProcessOrderCount = OrderMasterList.Where(x => x.Status != OrderStatus.CompletelyProcessed && x.Status != OrderStatus.Draft && x.Status != OrderStatus.Reject).Count();
                model.InProcessOrderValue = ExtensionUtility.FormatNumberAmount(OrderMasterList.Where(x => x.Status != OrderStatus.CompletelyProcessed && x.Status != OrderStatus.Draft && x.Status != OrderStatus.Reject).Sum(x => x.TotalValue));

                model.Complaint = _Complaint.Where(x => x.Status == ComplaintStatus.Pending).Count();
                model.PendingApproval = OrderMasterList.Where(x => x.Status == OrderStatus.PendingApproval).Count();
                model.Approved = OrderMasterList.Where(x => x.Status == OrderStatus.Approved).Count();
                model.InProcess = _OrderDetail.Where(x => x.OrderProductStatus == OrderStatus.InProcess).Count();
                model.PartiallyProcessed = _OrderDetail.Where(x => x.OrderProductStatus == OrderStatus.PartiallyProcessed).Count();
                model.CompletelyProcessed = _OrderDetail.Where(x => x.OrderProductStatus == OrderStatus.CompletelyProcessed).Count();
                model.OnHold = OrderMasterList.Where(x => x.Status == OrderStatus.Onhold).Count();
                model.Reject = OrderMasterList.Where(x => x.Status == OrderStatus.Reject).Count();
                model.Draft = OrderMasterList.Where(x => x.Status == OrderStatus.Draft).Count();
                model.VerifiedPayment = paymentMasters.Where(x => x.Status == PaymentStatus.Verified).Sum(x => x.Amount);
                model.UnverifiedPayment = paymentMasters.Where(x => x.Status == PaymentStatus.Unverified).Sum(x => x.Amount);
                model.ReturnOrder = _OrderReturnMaster.Where(x => x.Status != OrderReturnStatus.Draft).Count();

                model.PolicyList = _PolicyBLL.Where(x => x.IsActive && !x.IsDeleted).ToList();
                SessionHelper.Notification = _Notification;
                if (SessionHelper.LoginUser.Distributor != null)
                {
                    SessionHelper.SAPOrderPendingValue = _OrderBLL.GetPendingOrderValue(SessionHelper.LoginUser.Distributor.DistributorSAPCode, _configuration).ToList();
                    SessionHelper.DistributorBalance = _OrderBLL.GetBalance(SessionHelper.LoginUser.Distributor.DistributorSAPCode, _configuration);
                }
                return View(model);
            }
            catch (Exception ex)
            {
                new ErrorLogBLL(_unitOfWork).AddExceptionLog(ex);
                return View();
            }
        }
        public IActionResult GetDistributorPaymentWiseComparision()
        {
            DateTime CurrentYear = DateTime.Now;
            DateTime LastYear = DateTime.Now.AddYears(-1).AddMonths(1);
            var months = ExtensionUtility.MonthsBetween(LastYear, CurrentYear);
            List<OrderMaster> orderMasters = _OrderMaster.Where(x => x.DistributorId == SessionHelper.LoginUser.DistributorId && x.Status != OrderStatus.Draft && x.Status != OrderStatus.Reject && x.Status != OrderStatus.Cancel).ToList();

            List<PaymentWiseComparision> PaymentWiseComparision = new List<PaymentWiseComparision>();
            foreach (var item in months.Item1)
            {
                PaymentWiseComparision PWSCurrent = new PaymentWiseComparision();
                PWSCurrent.Month = item.MonthName;
                PWSCurrent.CurrentYear = orderMasters.Where(x => x.CreatedDate.Year == item.Year && x.CreatedDate.Month == item.Month).Sum(x => x.TotalValue);
                PWSCurrent.LastYear = orderMasters.Where(x => x.CreatedDate.Year == item.LastYear && x.CreatedDate.Month == item.Month).Sum(x => x.TotalValue);
                PaymentWiseComparision.Add(PWSCurrent);
            }
            return PartialView("DistributorPaymentWise", PaymentWiseComparision);
        }
        public IActionResult GetDistributorOrderWiseComparision()
        {
            DateTime CurrentYear = DateTime.Now;
            DateTime LastYear = DateTime.Now.AddYears(-1).AddMonths(1);
            var months = ExtensionUtility.MonthsBetween(LastYear, CurrentYear);
            List<OrderMaster> orderMasters = _OrderMaster.Where(x => x.DistributorId == SessionHelper.LoginUser.DistributorId && x.Status != OrderStatus.Draft && x.Status != OrderStatus.Reject && x.Status != OrderStatus.Cancel).ToList();

            List<OrderWiseComparision> OrderWiseComparision = new List<OrderWiseComparision>();
            foreach (var item in months.Item1)
            {
                OrderWiseComparision OWSCurrent = new OrderWiseComparision();
                OWSCurrent.Month = item.MonthName;
                OWSCurrent.CurrentYear = orderMasters.Where(x => x.CreatedDate.Year == item.Year && x.CreatedDate.Month == item.Month).Count();
                OWSCurrent.LastYear = orderMasters.Where(x => x.CreatedDate.Year == item.LastYear && x.CreatedDate.Month == item.Month).Count();
                OrderWiseComparision.Add(OWSCurrent);
            }
            return PartialView("DistributorOrderWise", OrderWiseComparision);
        }
        public IActionResult GetDistributorPaymentWiseStatus()
        {
            DateTime CurrentYear = DateTime.Now;
            DateTime LastYear = DateTime.Now.AddYears(-1).AddMonths(1);
            var months = ExtensionUtility.MonthsBetween(LastYear, CurrentYear);
            List<PaymentMaster> PaymentMasterList = _PaymentMaster.Where(x => x.Status == PaymentStatus.Verified && x.DistributorId == SessionHelper.LoginUser.DistributorId).ToList();

            List<PaymentWiseStatus> PaymentWiseStatus = new List<PaymentWiseStatus>();
            foreach (var item in months.Item1)
            {
                PaymentWiseStatus PWSCurrent = new PaymentWiseStatus();
                PWSCurrent.Month = item.MonthName;
                PWSCurrent.VerifiedPayment = PaymentMasterList.Where(x => x.CreatedDate.Year == item.Year && x.CreatedDate.Month == item.Month && x.Status == PaymentStatus.Verified).Sum(x => x.Amount);
                PWSCurrent.UnverifiedPayment = PaymentMasterList.Where(x => x.CreatedDate.Year == item.Year && x.CreatedDate.Month == item.Month && x.Status == PaymentStatus.Unverified).Sum(x => x.Amount);
                PaymentWiseStatus.Add(PWSCurrent);
            }

            return PartialView("DistributorPaymentWiseStatus", PaymentWiseStatus);
        }
        public IActionResult GetDistributorRecentPaymentStatus()
        {
            DistributorRecentPaymentStatus model = new DistributorRecentPaymentStatus();
            DateTime CurrentYear = DateTime.Now;
            DateTime LastYear = DateTime.Now.AddYears(-1).AddMonths(1);
            var months = ExtensionUtility.MonthsBetween(LastYear, CurrentYear);
            List<OrderMaster> orderMasters = _OrderMaster.Where(x => x.DistributorId == SessionHelper.LoginUser.DistributorId && x.Status != OrderStatus.Draft && x.Status != OrderStatus.Reject && x.Status != OrderStatus.Cancel).ToList();
            List<OrderMaster> OrderMasterList = orderMasters.Where(x => x.DistributorId == SessionHelper.LoginUser.DistributorId).ToList();
            List<OrderDetail> OrderDetailList = _OrderDetail.Where(x => x.OrderMaster.DistributorId == SessionHelper.LoginUser.DistributorId).ToList();
            List<PaymentMaster> PaymentMasterList = _PaymentMaster.Where(x => x.DistributorId == SessionHelper.LoginUser.DistributorId).ToList();

            model.RecentOrder = new List<RecentOrder>();
            model.RecentOrder = OrderMasterList.OrderByDescending(x => x.CreatedDate).Select(y => new RecentOrder
            {
                Id = y.Id,
                OrderNo = y.SNo,
                DistributorName = y.Distributor.DistributorName,
                Status = y.Status,
                Amount = y.TotalValue

            }).Take(5).ToList();

            model.RecentPayment = new List<RecentPayment>();
            model.RecentPayment = PaymentMasterList.OrderByDescending(x => x.CreatedDate).Select(y => new RecentPayment
            {
                Id = y.Id,
                PaymentId = y.SNo,
                PaymentMode = y.PaymentMode.PaymentName,
                DistributorName = y.Distributor.DistributorName,
                Status = y.Status,
                Amount = y.Amount
            }).Take(5).ToList();

            model.ProductViewModelOrder = new List<ProductViewModel>();
            model.ProductViewModelOrder = OrderDetailList.GroupBy(x => x.ProductId).Select(y => new ProductViewModel
            {
                ProductName = _ProductMaster.FirstOrDefault(x => x.Id == y.Key).ProductName,
                CurrentPrice = _ProductMaster.FirstOrDefault(x => x.Id == y.Key).ProductPrice,
                OrderCount = y.Count()
            }).Take(5).ToList();

            model.ProductViewModelQuantity = new List<ProductViewModel>();
            model.ProductViewModelQuantity = OrderDetailList.GroupBy(x => x.ProductId).Select(y => new ProductViewModel
            {
                ProductName = _ProductMaster.FirstOrDefault(x => x.Id == y.Key).ProductName,
                CurrentPrice = _ProductMaster.FirstOrDefault(x => x.Id == y.Key).ProductPrice,
                Quantity = y.Sum(x => x.Quantity)
            }).Take(5).ToList();

            return PartialView("DistributorRecentPaymentStatus", model);
        }
        public IActionResult GetDistributorPendingQuantity()
        {
            JsonResponse jsonResponse = new JsonResponse();
            try
            {
                if (SessionHelper.LoginUser.Distributor != null)
                {
                    //SessionHelper.SAPOrderPendingQuantity = _OrderBLL.GetDistributorPendingQuantity(SessionHelper.LoginUser.Distributor.DistributorSAPCode, _configuration).ToList();
                    SessionHelper.SAPOrderPendingQuantity = _OrderBLL.GetDistributorOrderPendingQuantity(SessionHelper.LoginUser.Distributor.DistributorSAPCode, _configuration);
                    var pendingQuantity = SessionHelper.SAPOrderPendingQuantity;
                    pendingQuantity.ForEach(x => x.Id = _ProductMaster.FirstOrDefault(y => y.SAPProductCode == x.ProductCode) != null ? _ProductMaster.FirstOrDefault(y => y.SAPProductCode == x.ProductCode).Id : 0);
                    pendingQuantity.ForEach(x => x.ProductName = _ProductMaster.FirstOrDefault(y => y.SAPProductCode == x.ProductCode) != null ? _ProductMaster.FirstOrDefault(y => y.SAPProductCode == x.ProductCode).ProductName + " " + _ProductMaster.FirstOrDefault(y => y.SAPProductCode == x.ProductCode).ProductDescription : null);
                    SessionHelper.SAPOrderPendingQuantity = pendingQuantity;
                    SessionHelper.SAPOrderPendingQuantity = SessionHelper.SAPOrderPendingQuantity.OrderByDescending(x => Convert.ToDouble(x.PendingQuantity)).ToList();
                }
                else
                {
                    SessionHelper.SAPOrderPendingQuantity = new List<SAPOrderPendingQuantity>();
                }
            }
            catch (Exception ex)
            {
                new ErrorLogBLL(_unitOfWork).AddExceptionLog(ex);
                jsonResponse.Status = false;
                jsonResponse.Message = NotificationMessage.ErrorOccurred;
                return Json(new { data = jsonResponse });
            }
            return PartialView("DistributorPendingQuantity", SessionHelper.SAPOrderPendingQuantity);
        }
        #endregion

        #region Store Keeper
        public IActionResult StoreKeeperDashboard()
        {
            try
            {
                StoreKeeperDashboard model = new StoreKeeperDashboard();
                model.Submitted = _OrderReturnMaster.Where(x => x.Status == OrderReturnStatus.Submitted).Count();
                model.Received = _OrderReturnMaster.Where(x => x.Status == OrderReturnStatus.Received).Count();
                model.Completed = _OrderReturnMaster.Where(x => x.Status == OrderReturnStatus.CompletelyProcessed).Count();
                return View(model);
            }
            catch (Exception ex)
            {
                new ErrorLogBLL(_unitOfWork).AddExceptionLog(ex);
                return View();
            }
        }
        #endregion
        public IActionResult GetFile(string filepath)
        {
            try
            {
                string filename = Path.GetFileName(filepath);
                using (var provider = new PhysicalFileProvider(Path.GetDirectoryName(filepath)))
                {
                    var stream = provider.GetFileInfo(filename).CreateReadStream();
                    new FileExtensionContentTypeProvider().TryGetContentType(filename, out string contenttype);
                    if (contenttype == "application/vnd.openxmlformats-officedocument.wordprocessingml.document")
                    {
                        return File(stream, contenttype, filename.Split('_')[1]);
                    }
                    return File(stream, contenttype);
                }
            }
            catch (Exception ex)
            {
                new ErrorLogBLL(_unitOfWork).AddExceptionLog(ex);
                return Json(false);
            }

        }

        #region Handle error
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("~/Views/Shared/Error.cshtml", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult InternalServerError()
        {
            return View("~/Views/Shared/InternalServerError.cshtml", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult AccessDenied()
        {
            return View("~/Views/Shared/AccessDenied.cshtml", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        [Route("/Home/HandleError/{code:int}")]
        public IActionResult HandleError(int code)
        {
            ViewData["ErrorMessage"] = $"Error occurred. The ErrorCode is: {code}";
            return View("~/Views/Shared/Error.cshtml");
        }
        #endregion    
    }
}
