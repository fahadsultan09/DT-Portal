using BusinessLogicLayer.Application;
using BusinessLogicLayer.ApplicationSetup;
using BusinessLogicLayer.ErrorLog;
using BusinessLogicLayer.GeneralSetup;
using BusinessLogicLayer.HelperClasses;
using DataAccessLayer.WorkProcess;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.FileProviders;
using Models.Application;
using Models.ViewModel;
using MoreLinq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
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
        private readonly DisclaimerBLL _DisclaimerBLL;
        private List<PaymentMaster> _PaymentMaster;
        private List<OrderMaster> _OrderMaster;
        private List<OrderDetail> _OrderDetail;
        private List<OrderReturnMaster> _OrderReturnMaster;
        private List<Complaint> _Complaint;
        private List<ProductMaster> _ProductMaster;
        private List<Distributor> _Distributor;
        private readonly Configuration _configuration;
        public HomeController(IUnitOfWork unitOfWork, Configuration configuration)
        {
            _unitOfWork = unitOfWork;
            _configuration = configuration;
            _OrderBLL = new OrderBLL(_unitOfWork);
            _OrderDetailBLL = new OrderDetailBLL(_unitOfWork);
            _PaymentBLL = new PaymentBLL(_unitOfWork);
            _ComplaintBLL = new ComplaintBLL(_unitOfWork);
            _ProductMasterBLL = new ProductMasterBLL(_unitOfWork);
            _DistributorBLL = new DistributorBLL(_unitOfWork);
            _OrderReturnBLL = new OrderReturnBLL(_unitOfWork);
            _PolicyBLL = new PolicyBLL(_unitOfWork);
            _NotificationBLL = new NotificationBLL(_unitOfWork);
            _DisclaimerBLL = new DisclaimerBLL(_unitOfWork);

            if (_DisclaimerBLL.GetAllDisclaimer().FirstOrDefault() != null)
            {
                SessionHelper.Disclaimer = _DisclaimerBLL.GetAllDisclaimer().FirstOrDefault(x => x.IsActive)?.Description;
            }
            else
            {
                SessionHelper.Disclaimer = string.Empty;
            }
            if (SessionHelper.LoginUser != null)
            {
                SessionHelper.Notification = _NotificationBLL.GetAllNotification().Where(x => (SessionHelper.LoginUser.IsDistributor
                ? x.DistributorId == SessionHelper.LoginUser.DistributorId && x.CreatedDate.Date >= DateTime.Now.Date.AddDays(-10)
                && Enum.GetValues(typeof(DistributorTransactionStatus)).OfType<DistributorTransactionStatus>().Select(s => s.ToString()).Contains(x.Status)
                : x.CompanyId == SessionHelper.LoginUser.CompanyId && Enum.GetValues(typeof(TransactionStatus)).OfType<TransactionStatus>().Select(s => s.ToString()).Contains(x.Status)
                && SessionHelper.NavigationMenu.Where(y => y.ApplicationActionId == (int)ApplicationActions.View).Select(y => y.ApplicationPageId).Contains(x.ApplicationPageId))
                && x.CreatedDate.Date >= DateTime.Now.Date.AddDays(-10)).OrderByDescending(x => x.CreatedDate).DistinctBy(x => x.RequestId).ToList();

                var list = SessionHelper.Notification;
                list.ForEach(x => x.RelativeTime = ExtensionUtility.TimeAgo(x.CreatedDate));
                SessionHelper.Notification = list;
                if (SessionHelper.LoginUser.IsDistributor)
                {
                    SessionHelper.NotificationCount = SessionHelper.Notification.Where(x => !x.IsView).Count();
                }
                else
                {
                    SessionHelper.NotificationCount = list.Where(x => x.ApplicationPageId == (int)ApplicationPages.Order ? !x.IsOrderView : x.ApplicationPageId == (int)ApplicationPages.OrderReturn ? !x.IsOrderReturnView : x.ApplicationPageId == (int)ApplicationPages.Payment ? !x.IsPaymentView : true).Count();
                }
            }
            else
            {
                SessionHelper.Notification = new List<Notification>();
            }
        }
        public IActionResult Index()
        {
            new AuditLogBLL(_unitOfWork).AddAuditLog("Home", "Index", "Dashboard");

            var dashboard = SessionHelper.NavigationMenu.FirstOrDefault(x => (x.ApplicationPage.ControllerName == "Home" || x.ApplicationPage.ControllerName == "StoreKeeperDashboard") && x.ApplicationPage.PageTitle != "Allow users to view files");
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
                _PaymentMaster = _PaymentBLL.GetAllPaymentMaster().ToList();
                _OrderDetail = _OrderDetailBLL.Where(x => x.OrderMaster.IsActive && !x.OrderMaster.IsDeleted).ToList();
                _OrderMaster = _OrderBLL.Where(x => x.IsActive && !x.IsDeleted).ToList();
                _OrderReturnMaster = _OrderReturnBLL.GetAllOrderReturn().ToList();
                AdminDashboardViewModel model = new AdminDashboardViewModel();
                model.Complaint = _ComplaintBLL.GetPendingComplaint().Count();
                model.PartiallyApproved = _OrderMaster.Where(x => x.Status == OrderStatus.PartiallyApproved).Count();
                model.PendingApproval = _OrderMaster.Where(x => x.Status == OrderStatus.PendingApproval).Count();
                model.Approved = _OrderMaster.Where(x => x.Status == OrderStatus.Approved).Count();
                model.OnHold = _OrderMaster.Where(x => x.Status == OrderStatus.Onhold).Count();
                model.Reject = _OrderMaster.Where(x => x.Status == OrderStatus.Rejected).Count();
                model.InProcess = _OrderDetail.Where(x => x.OrderProductStatus == OrderStatus.InProcess).Select(x => x.SAPOrderNumber).Distinct().Count();
                model.PartiallyProcessed = _OrderDetail.Where(x => x.OrderProductStatus == OrderStatus.PartiallyProcessed).Select(x => x.SAPOrderNumber).Distinct().Count();
                model.CompletelyProcessed = _OrderDetail.Where(x => x.OrderProductStatus == OrderStatus.CompletelyProcessed).Select(x => x.SAPOrderNumber).Distinct().Count();
                model.PendingOrder = model.PendingApproval + model.OnHold + model.PartiallyApproved;
                model.VerifiedPayment = _PaymentMaster.Where(x => x.Status == PaymentStatus.Verified).Sum(x => x.Amount);
                model.UnverifiedPayment = _PaymentMaster.Where(x => x.Status == PaymentStatus.Unverified).Sum(x => x.Amount);
                model.ReturnOrder = _OrderReturnMaster.Where(x => x.Status == OrderReturnStatus.Submitted || x.Status == OrderReturnStatus.PartiallyReceived).Count();
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
            _OrderMaster = _OrderBLL.Where(x => x.IsActive && !x.IsDeleted).ToList();
            DateTime CurrentYear = DateTime.Now;
            DateTime LastYear = DateTime.Now.AddYears(-1).AddMonths(1);
            var months = ExtensionUtility.MonthsBetween(LastYear, CurrentYear);
            List<OrderMaster> orderMasters = _OrderMaster.Where(x => x.Status != OrderStatus.Draft && x.Status != OrderStatus.Rejected && x.Status != OrderStatus.Canceled).ToList();
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
            _OrderMaster = _OrderBLL.Where(x => x.IsActive && !x.IsDeleted).ToList();
            DateTime CurrentYear = DateTime.Now;
            DateTime LastYear = DateTime.Now.AddYears(-1).AddMonths(1);
            var months = ExtensionUtility.MonthsBetween(LastYear, CurrentYear);
            List<OrderMaster> orderMasters = _OrderMaster.Where(x => x.Status != OrderStatus.Draft && x.Status != OrderStatus.Rejected && x.Status != OrderStatus.Canceled).ToList();
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
            _PaymentMaster = _PaymentBLL.GetAllPaymentMaster().ToList();
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
            _OrderMaster = _OrderBLL.Where(x => x.IsActive && !x.IsDeleted).ToList();
            DateTime CurrentYear = DateTime.Now;
            DateTime LastYear = DateTime.Now.AddYears(-1).AddMonths(1);
            var months = ExtensionUtility.MonthsBetween(LastYear, CurrentYear);
            List<RegionWiseOrder> RegionWiseOrder = new List<RegionWiseOrder>();
            List<OrderMaster> orderMasters = _OrderMaster.Where(x => x.Status != OrderStatus.Draft && x.Status != OrderStatus.Rejected && x.Status != OrderStatus.Canceled).ToList();

            RegionWiseOrder = orderMasters.GroupBy(x => x.Distributor.Region.RegionName).Select(y => new RegionWiseOrder
            {
                Region = y.Key.ToString(),
                OrderCount = y.Count()
            }).ToList();

            return PartialView("AdminRegionWiseOrder", RegionWiseOrder);
        }
        public IActionResult GetAdminRecentPaymentStatus()
        {
            _Distributor = _DistributorBLL.GetAllDistributor().ToList();
            _OrderMaster = _OrderBLL.Where(x => x.IsActive && !x.IsDeleted).ToList();
            _OrderDetail = _OrderDetailBLL.Where(x => x.OrderMaster.IsActive && !x.OrderMaster.IsDeleted).ToList();
            _ProductMaster = _ProductMasterBLL.GetAllProductMaster().ToList();
            AdminRecentPaymentStatus model = new AdminRecentPaymentStatus();
            DateTime CurrentYear = DateTime.Now;
            DateTime LastYear = DateTime.Now.AddYears(-1).AddMonths(1);
            var months = ExtensionUtility.MonthsBetween(LastYear, CurrentYear);
            List<OrderMaster> orderMasters = _OrderMaster.Where(x => x.Status != OrderStatus.Draft && x.Status != OrderStatus.Rejected && x.Status != OrderStatus.Canceled).ToList();
            model.DistributorViewModelOrder = new List<DistributorViewModel>();
            model.DistributorViewModelOrder = orderMasters.GroupBy(x => x.DistributorId).Select(y => new DistributorViewModel
            {
                DistributorName = _Distributor.FirstOrDefault(x => x.Id == y.Key).DistributorName,
                OrderCount = y.Count()
            }).Take(5).ToList();

            model.DistributorViewModelPayment = new List<DistributorViewModel>();
            model.DistributorViewModelPayment = _PaymentBLL.Where(x => x.Status == PaymentStatus.Verified).GroupBy(x => x.DistributorId).Select(y => new DistributorViewModel
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
                _PaymentMaster = _PaymentBLL.GetAllPaymentMaster().ToList();
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
            _PaymentMaster = _PaymentBLL.GetAllPaymentMaster().ToList();
            List<PaymentWiseComparision> PaymentWiseComparision = new List<PaymentWiseComparision>();
            foreach (var item in months.Item1)
            {
                PaymentWiseComparision PWSCurrent = new PaymentWiseComparision();
                PWSCurrent.Month = item.MonthName;
                PWSCurrent.CurrentYear = _PaymentMaster.Where(x => x.CreatedDate.Year == item.Year && x.CreatedDate.Month == item.Month).Sum(x => x.Amount);
                PWSCurrent.LastYear = _PaymentMaster.Where(x => x.CreatedDate.Year == item.LastYear && x.CreatedDate.Month == item.Month).Sum(x => x.Amount);
                PaymentWiseComparision.Add(PWSCurrent);
            }
            return PartialView("AccountPaymentWise", PaymentWiseComparision);
        }
        public IActionResult GetAccountPaymentWiseStatus()
        {
            _PaymentMaster = _PaymentBLL.GetAllPaymentMaster().ToList();
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

            return PartialView("AccountPaymentWiseStatus", PaymentWiseStatus);
        }
        public IActionResult GetAccountRecentPaymentStatus()
        {
            AccountRecentPaymentStatus model = new AccountRecentPaymentStatus();
            DateTime CurrentYear = DateTime.Now;
            DateTime LastYear = DateTime.Now.AddYears(-1).AddMonths(1);
            var months = ExtensionUtility.MonthsBetween(LastYear, CurrentYear);
            _PaymentMaster = _PaymentBLL.Where(x => x.Status == PaymentStatus.Verified).ToList();
            model.PaymentWiseAmount = new List<PaymentWiseAmount>();
            model.PaymentWiseAmount = _PaymentMaster.GroupBy(x => x.PaymentMode.PaymentName).Select(y => new PaymentWiseAmount
            {
                PaymentMode = y.Key.ToString() + " " + ExtensionUtility.FormatNumberAmount(y.Sum(x => x.Amount)),
                Amount = y.Sum(x => x.Amount)
            }).ToList();

            model.RecentPayment = new List<RecentPayment>();
            model.RecentPayment = _PaymentMaster.OrderByDescending(x => x.CreatedDate).Select(y => new RecentPayment
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
        public async Task<IActionResult> DistributorDashboard()
        {
            try
            {
                DistributorDashboardViewModel model = new DistributorDashboardViewModel();
                _OrderDetail = _OrderDetailBLL.Where(x => x.OrderMaster.IsActive && !x.OrderMaster.IsDeleted && x.OrderMaster.DistributorId == SessionHelper.LoginUser.DistributorId).ToList();
                _OrderReturnMaster = _OrderReturnBLL.Where(x => x.DistributorId == SessionHelper.LoginUser.DistributorId).ToList();
                _OrderMaster = _OrderBLL.Where(x => x.IsActive && !x.IsDeleted && x.DistributorId == SessionHelper.LoginUser.DistributorId).ToList();
                _PaymentMaster = _PaymentBLL.Where(x => x.DistributorId == SessionHelper.LoginUser.DistributorId).ToList();
                _OrderReturnMaster = _OrderReturnMaster.Where(x => x.DistributorId == SessionHelper.LoginUser.DistributorId).ToList();

                model.UnverifiedPaymentAllCount = _PaymentMaster.Where(x => x.Status == PaymentStatus.Unverified).Count();
                model.UnverifiedPaymentAll = ExtensionUtility.FormatNumberAmount(_PaymentMaster.Where(x => x.CreatedDate.Year == DateTime.Now.Year && x.Status == PaymentStatus.Unverified).Sum(x => x.Amount));

                model.InProcessOrderCount = _OrderMaster.Where(x => x.Status == OrderStatus.PendingApproval || x.Status == OrderStatus.Onhold || x.Status == OrderStatus.PartiallyApproved).Count();
                model.InProcessOrderValue = ExtensionUtility.FormatNumberAmount(_OrderMaster.Where(x => x.Status == OrderStatus.PendingApproval || x.Status == OrderStatus.Onhold || x.Status == OrderStatus.PartiallyApproved).Sum(x => x.TotalValue));

                model.Complaint = _ComplaintBLL.GetPendingComplaint().Where(x => x.DistributorId == SessionHelper.LoginUser.DistributorId).Count();
                model.PendingApproval = _OrderMaster.Where(x => x.Status == OrderStatus.PendingApproval).Count();
                model.Approved = _OrderMaster.Where(x => x.Status == OrderStatus.Approved).Count();
                model.InProcess = _OrderDetail.Where(x => x.OrderMaster.DistributorId == SessionHelper.LoginUser.DistributorId && x.OrderProductStatus == OrderStatus.InProcess).Select(x => x.SAPOrderNumber).Distinct().Count();
                model.PartiallyProcessed = _OrderDetail.Where(x => x.OrderMaster.DistributorId == SessionHelper.LoginUser.DistributorId && x.OrderProductStatus == OrderStatus.PartiallyProcessed).Select(x => x.SAPOrderNumber).Distinct().Count();
                model.CompletelyProcessed = _OrderDetail.Where(x => x.OrderMaster.DistributorId == SessionHelper.LoginUser.DistributorId && x.OrderProductStatus == OrderStatus.CompletelyProcessed).Select(x => x.SAPOrderNumber).Distinct().Count();
                model.OnHold = _OrderMaster.Where(x => x.Status == OrderStatus.Onhold).Count();
                model.Reject = _OrderMaster.Where(x => x.Status == OrderStatus.Rejected).Count();
                model.Draft = _OrderMaster.Where(x => x.Status == OrderStatus.Draft).Count();
                model.VerifiedPayment = _PaymentMaster.Where(x => x.Status == PaymentStatus.Verified).Sum(x => x.Amount);
                model.UnverifiedPayment = _PaymentMaster.Where(x => x.Status == PaymentStatus.Unverified).Sum(x => x.Amount);
                model.ReturnOrder = _OrderReturnMaster.Where(x => x.Status == OrderReturnStatus.Submitted).Count();

                model.PolicyList = _PolicyBLL.Where(x => x.IsActive && !x.IsDeleted).ToList();
                await Get();
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
            _OrderMaster = _OrderBLL.Where(x => x.IsActive && !x.IsDeleted && x.DistributorId == SessionHelper.LoginUser.DistributorId && x.Status != OrderStatus.Draft && x.Status != OrderStatus.Rejected && x.Status != OrderStatus.Canceled).ToList();
            DateTime CurrentYear = DateTime.Now;
            DateTime LastYear = DateTime.Now.AddYears(-1).AddMonths(1);
            var months = ExtensionUtility.MonthsBetween(LastYear, CurrentYear);
            List<OrderMaster> orderMasters = _OrderMaster.Where(x => x.DistributorId == SessionHelper.LoginUser.DistributorId && x.Status != OrderStatus.Draft && x.Status != OrderStatus.Rejected && x.Status != OrderStatus.Canceled).ToList();

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
            _OrderMaster = _OrderBLL.Where(x => x.IsActive && !x.IsDeleted && x.DistributorId == SessionHelper.LoginUser.DistributorId && x.Status != OrderStatus.Draft && x.Status != OrderStatus.Rejected && x.Status != OrderStatus.Canceled).ToList();
            DateTime CurrentYear = DateTime.Now;
            DateTime LastYear = DateTime.Now.AddYears(-1).AddMonths(1);
            var months = ExtensionUtility.MonthsBetween(LastYear, CurrentYear);
            List<OrderMaster> orderMasters = _OrderMaster.Where(x => x.DistributorId == SessionHelper.LoginUser.DistributorId && x.Status != OrderStatus.Draft && x.Status != OrderStatus.Rejected && x.Status != OrderStatus.Canceled).ToList();

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
            _PaymentMaster = _PaymentBLL.Where(x => x.DistributorId == SessionHelper.LoginUser.DistributorId).ToList();
            DateTime CurrentYear = DateTime.Now;
            DateTime LastYear = DateTime.Now.AddYears(-1).AddMonths(1);
            var months = ExtensionUtility.MonthsBetween(LastYear, CurrentYear);

            List<PaymentWiseStatus> PaymentWiseStatus = new List<PaymentWiseStatus>();
            foreach (var item in months.Item1.ToList())
            {
                PaymentWiseStatus PWSCurrent = new PaymentWiseStatus();
                PWSCurrent.Month = item.MonthName;
                PWSCurrent.VerifiedPayment = _PaymentMaster.Where(x => x.CreatedDate.Year == item.Year && x.CreatedDate.Month == item.Month && x.Status == PaymentStatus.Verified).Sum(x => x.Amount);
                PWSCurrent.UnverifiedPayment = _PaymentMaster.Where(x => x.CreatedDate.Year == item.Year && x.CreatedDate.Month == item.Month && x.Status == PaymentStatus.Unverified).Sum(x => x.Amount);
                PaymentWiseStatus.Add(PWSCurrent);
            }

            return PartialView("DistributorPaymentWiseStatus", PaymentWiseStatus);
        }
        public IActionResult GetDistributorRecentPaymentStatus()
        {
            _OrderDetail = _OrderDetailBLL.Where(x => x.OrderMaster.IsActive && !x.OrderMaster.IsDeleted && x.OrderMaster.DistributorId == SessionHelper.LoginUser.DistributorId).ToList();
            _OrderMaster = _OrderBLL.Where(x => x.IsActive && !x.IsDeleted && x.DistributorId == SessionHelper.LoginUser.DistributorId && x.Status != OrderStatus.Draft && x.Status != OrderStatus.Rejected && x.Status != OrderStatus.Canceled).ToList();
            _ProductMaster = _ProductMasterBLL.GetAllProductMaster().ToList();
            DistributorRecentPaymentStatus model = new DistributorRecentPaymentStatus();
            DateTime CurrentYear = DateTime.Now;
            DateTime LastYear = DateTime.Now.AddYears(-1).AddMonths(1);
            var months = ExtensionUtility.MonthsBetween(LastYear, CurrentYear);
            List<OrderMaster> orderMasters = _OrderMaster.Where(x => x.DistributorId == SessionHelper.LoginUser.DistributorId && x.Status != OrderStatus.Draft && x.Status != OrderStatus.Rejected && x.Status != OrderStatus.Canceled).ToList();
            _PaymentMaster = _PaymentBLL.Where(x => x.DistributorId == SessionHelper.LoginUser.DistributorId).ToList();

            model.RecentOrder = new List<RecentOrder>();
            model.RecentOrder = orderMasters.OrderByDescending(x => x.CreatedDate).Select(y => new RecentOrder
            {
                Id = y.Id,
                OrderNo = y.SNo,
                DistributorName = y.Distributor.DistributorName,
                Status = y.Status,
                Amount = y.TotalValue

            }).Take(5).ToList();

            model.RecentPayment = new List<RecentPayment>();
            model.RecentPayment = _PaymentMaster.OrderByDescending(x => x.CreatedDate).Select(y => new RecentPayment
            {
                Id = y.Id,
                PaymentId = y.SNo,
                PaymentMode = y.PaymentMode.PaymentName,
                DistributorName = y.Distributor.DistributorName,
                Status = y.Status,
                Amount = y.Amount
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

            return PartialView("DistributorRecentPaymentStatus", model);
        }
        public JsonResult GetDistributorBalance()
        {
            List<KeyValuePair<string, double>> list = new List<KeyValuePair<string, double>>();
            try
            {
                if (SessionHelper.LoginUser.IsDistributor && SessionHelper.DistributorBalance == null)
                {
                    SessionHelper.DistributorBalance = _PaymentBLL.GetDistributorBalance(SessionHelper.LoginUser.Distributor.DistributorSAPCode, _configuration);
                    if (SessionHelper.DistributorBalance != null)
                    {
                        list.Add(new KeyValuePair<string, double>("SAMI", SessionHelper.DistributorBalance.SAMI));
                        list.Add(new KeyValuePair<string, double>("HealthTek", SessionHelper.DistributorBalance.HealthTek));
                        list.Add(new KeyValuePair<string, double>("Min", SessionHelper.DistributorBalance.SAMI < SessionHelper.DistributorBalance.HealthTek ? Math.Abs(SessionHelper.DistributorBalance.SAMI) : Math.Abs(SessionHelper.DistributorBalance.HealthTek)));
                        list.Add(new KeyValuePair<string, double>("Max", SessionHelper.DistributorBalance.SAMI > SessionHelper.DistributorBalance.HealthTek ? Math.Abs(SessionHelper.DistributorBalance.SAMI) : Math.Abs(SessionHelper.DistributorBalance.HealthTek)));
                    }
                }
                else
                {
                    list.Add(new KeyValuePair<string, double>("SAMI", SessionHelper.DistributorBalance.SAMI));
                    list.Add(new KeyValuePair<string, double>("HealthTek", SessionHelper.DistributorBalance.HealthTek));
                    list.Add(new KeyValuePair<string, double>("Min", SessionHelper.DistributorBalance.SAMI < SessionHelper.DistributorBalance.HealthTek ? Math.Abs(SessionHelper.DistributorBalance.SAMI) : Math.Abs(SessionHelper.DistributorBalance.HealthTek)));
                    list.Add(new KeyValuePair<string, double>("Max", SessionHelper.DistributorBalance.SAMI > SessionHelper.DistributorBalance.HealthTek ? Math.Abs(SessionHelper.DistributorBalance.SAMI) : Math.Abs(SessionHelper.DistributorBalance.HealthTek)));
                }
            }
            catch (Exception ex)
            {
                new ErrorLogBLL(_unitOfWork).AddExceptionLog(ex);
            }
            return Json(new { data = list });
        }
        public IActionResult GetDistributorPendingQuantity()
        {
            JsonResponse jsonResponse = new JsonResponse();
            try
            {
                if (SessionHelper.LoginUser.IsDistributor && (SessionHelper.DistributorPendingQuantity == null || SessionHelper.DistributorPendingQuantity.Count() == 0))
                {
                    SessionHelper.DistributorPendingQuantity = _OrderBLL.DistributorPendingQuantity((int)SessionHelper.LoginUser.DistributorId);
                    SessionHelper.DistributorPendingValue = _OrderBLL.DistributorPendingValue(SessionHelper.DistributorPendingQuantity);
                }
            }
            catch (Exception ex)
            {
                new ErrorLogBLL(_unitOfWork).AddExceptionLog(ex);
                return Json(new { data = jsonResponse });
            }
            return PartialView("DistributorPendingQuantity", SessionHelper.DistributorPendingQuantity);
        }
        [HttpPost]
        public async Task Get()
        {
            try
            {
                if (SessionHelper.LoginUser.IsDistributor && (SessionHelper.DistributorPendingValue == null || SessionHelper.DistributorPendingValue.Count() == 0))
                {

                    if (SessionHelper.DistributorPendingQuantity == null || SessionHelper.DistributorPendingQuantity.Count() == 0)
                    {
                        SessionHelper.DistributorPendingQuantity = _OrderBLL.DistributorPendingQuantity((int)SessionHelper.LoginUser.DistributorId);
                    }
                    SessionHelper.DistributorPendingValue = _OrderBLL.DistributorPendingValue(SessionHelper.DistributorPendingQuantity);
                }
            }
            catch (Exception ex)
            {
                new ErrorLogBLL(_unitOfWork).AddExceptionLog(ex);
            }
        }
        #endregion

        #region Store Keeper
        public IActionResult StoreKeeperDashboard()
        {
            try
            {
                _OrderReturnMaster = _OrderReturnBLL.GetAllOrderReturn().ToList();
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
