using BusinessLogicLayer.Application;
using BusinessLogicLayer.ApplicationSetup;
using BusinessLogicLayer.ErrorLog;
using BusinessLogicLayer.HelperClasses;
using DataAccessLayer.WorkProcess;
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
        private readonly OrderReturnDetailBLL _OrderReturnDetailBLL;
        private readonly PaymentBLL _PaymentBLL;
        private readonly ComplaintBLL _ComplaintBLL;
        private readonly ProductMasterBLL _ProductMasterBLL;
        private readonly DistributorBLL _DistributorBLL;
        private List<PaymentMaster> _PaymentMaster;
        private List<OrderMaster> _OrderMaster;
        private List<OrderDetail> _OrderDetail;
        private List<OrderReturnMaster> _OrderReturnMaster;
        private List<OrderReturnDetail> _OrderReturnDetail;
        private List<Complaint> _Complaint;
        private List<ProductMaster> _ProductMaster;
        private List<Distributor> _Distributor;
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
            _OrderReturnDetailBLL = new OrderReturnDetailBLL(_unitOfWork);
            _PaymentMaster = _PaymentBLL.GetAllPaymentMaster().ToList();
            _OrderMaster = _OrderBLL.GetAllOrderMaster().ToList();
            _OrderDetail = _OrderDetailBLL.GetAllOrderDetail().ToList();
            _OrderReturnMaster = _OrderReturnBLL.GetAllOrderReturn().ToList();
            _OrderReturnDetail = _OrderReturnDetailBLL.GetAllOrderReturnDetail().ToList();
            _Complaint = _ComplaintBLL.GetAllComplaint().ToList();
            _ProductMaster = _ProductMasterBLL.GetAllProductMaster().ToList();
            _Distributor = _DistributorBLL.GetAllDistributor().ToList();
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

                model.Complaint = _Complaint.Where(x => x.Status == ComplaintStatus.Pending).Count();
                model.PendingApproval = _OrderMaster.Where(x => x.Status == OrderStatus.PendingApproval).Count();
                model.InProcess = _OrderMaster.Where(x => x.Status == OrderStatus.InProcess).Count();
                model.PartiallyProcessed = _OrderMaster.Where(x => x.Status == OrderStatus.PartiallyProcessed).Count();
                model.CompletelyProcessed = _OrderMaster.Where(x => x.Status == OrderStatus.CompletelyProcessed).Count();
                model.OnHold = _OrderMaster.Where(x => x.Status == OrderStatus.Onhold).Count();
                model.Reject = _OrderMaster.Where(x => x.Status == OrderStatus.Reject).Count();
                model.PendingOrder = model.PendingApproval + model.InProcess + model.PartiallyProcessed;
                model.VerifiedPayment = _PaymentMaster.Where(x => x.Status == PaymentStatus.Verified).Sum(x => x.Amount);
                model.UnverifiedPayment = _PaymentMaster.Where(x => x.Status == PaymentStatus.Unverified).Sum(x => x.Amount);
                model.ReturnOrder = _OrderReturnBLL.GetAllOrderReturn().Count();
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

            List<PaymentWiseComparision> PaymentWiseComparision = new List<PaymentWiseComparision>();
            foreach (var item in months.Item1)
            {
                PaymentWiseComparision PWSCurrent = new PaymentWiseComparision();
                PWSCurrent.Month = item.MonthName;
                PWSCurrent.CurrentYear = _OrderMaster.Where(x => x.CreatedDate.Year == item.Year && x.CreatedDate.Month == item.Month).Sum(x => x.TotalValue);
                PWSCurrent.LastYear = _OrderMaster.Where(x => x.CreatedDate.Year == item.LastYear && x.CreatedDate.Month == item.Month).Sum(x => x.TotalValue);
                PaymentWiseComparision.Add(PWSCurrent);
            }
            return PartialView("AdminPaymentWiseComparision", PaymentWiseComparision);
        }
        public IActionResult GetAdminOrderWiseComparision()
        {
            DateTime CurrentYear = DateTime.Now;
            DateTime LastYear = DateTime.Now.AddYears(-1).AddMonths(1);
            var months = ExtensionUtility.MonthsBetween(LastYear, CurrentYear);

            List<OrderWiseComparision> OrderWiseComparision = new List<OrderWiseComparision>();

            foreach (var item in months.Item1)
            {
                OrderWiseComparision OWSCurrent = new OrderWiseComparision();
                OWSCurrent.Month = item.MonthName;
                OWSCurrent.CurrentYear = _OrderMaster.Where(x => x.CreatedDate.Year == item.Year && x.CreatedDate.Month == item.Month).Count();
                OWSCurrent.LastYear = _OrderMaster.Where(x => x.CreatedDate.Year == item.LastYear && x.CreatedDate.Month == item.Month).Count();
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
            RegionWiseOrder = _OrderMaster.GroupBy(x => x.Distributor.Region.RegionName).Select(y => new RegionWiseOrder
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

            model.DistributorViewModelOrder = new List<DistributorViewModel>();
            model.DistributorViewModelOrder = _OrderMaster.GroupBy(x => x.DistributorId).Select(y => new DistributorViewModel
            {
                DistributorName = _Distributor.FirstOrDefault(x => x.Id == y.Key).DistributorName,
                OrderCount = y.Count()
            }).Take(5).ToList();

            model.DistributorViewModelPayment = new List<DistributorViewModel>();
            model.DistributorViewModelPayment = _PaymentMaster.GroupBy(x => x.DistributorId).Select(y => new DistributorViewModel
            {
                DistributorName = _Distributor.FirstOrDefault(x => x.Id == y.Key).DistributorName,
                Payment = y.Sum(x => x.Amount)
            }).Take(5).ToList();

            model.ProductViewModelOrder = new List<ProductViewModel>();
            model.ProductViewModelOrder = _OrderDetail.GroupBy(x => x.ProductId).Select(y => new ProductViewModel
            {
                ProductName = _ProductMaster.FirstOrDefault(x => x.Id == y.Key).ProductName,
                CurrentPrice = _ProductMaster.FirstOrDefault(x => x.Id == y.Key).ProductPrice.ToString(),
                OrderCount = y.Count()
            }).Take(5).ToList();

            model.ProductViewModelQuantity = new List<ProductViewModel>();
            model.ProductViewModelQuantity = _OrderDetail.GroupBy(x => x.ProductId).Select(y => new ProductViewModel
            {
                ProductName = _ProductMaster.FirstOrDefault(x => x.Id == y.Key).ProductName,
                CurrentPrice = _ProductMaster.FirstOrDefault(x => x.Id == y.Key).ProductPrice.ToString(),
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

            model.PaymentWiseAmount = new List<PaymentWiseAmount>();
            model.PaymentWiseAmount = _PaymentMaster.GroupBy(x => x.PaymentMode.PaymentName).Select(y => new PaymentWiseAmount
            {
                PaymentMode = y.Key.ToString() + " " + ExtensionUtility.FormatNumberAmount(y.Sum(x => x.Amount)),
                Amount = y.Sum(x => x.Amount)
            }).ToList();

            model.RecentPayment = new List<RecentPayment>();
            model.RecentPayment = _PaymentMaster.OrderByDescending(x => x.CreatedDate).Select(y => new RecentPayment
            {

                PaymentId = y.Id,
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
                _OrderMaster = _OrderMaster.Where(x => x.DistributorId == SessionHelper.LoginUser.DistributorId).ToList();
                _OrderDetail = _OrderDetail.Where(x => x.OrderMaster.DistributorId == SessionHelper.LoginUser.DistributorId).ToList();
                _PaymentMaster = _PaymentMaster.Where(x => x.DistributorId == SessionHelper.LoginUser.DistributorId).ToList();
                _Complaint = _Complaint.ToList();

                model.UnverifiedPaymentAllCount = _PaymentMaster.Where(x => x.Status == PaymentStatus.Unverified).Count();
                model.UnverifiedPaymentAll = ExtensionUtility.FormatNumberAmount(_PaymentMaster.Where(x => x.CreatedDate.Year == DateTime.Now.Year && x.Status == PaymentStatus.Unverified).Sum(x => x.Amount));

                model.InProcessOrderCount = _OrderMaster.Where(x => x.Status != OrderStatus.Draft && x.Status != OrderStatus.CompletelyProcessed && x.Status != OrderStatus.Reject).Count();
                model.InProcessOrderValue = ExtensionUtility.FormatNumberAmount(_OrderMaster.Where(x => x.Status != OrderStatus.Draft && x.Status != OrderStatus.CompletelyProcessed && x.Status != OrderStatus.Reject).Sum(x => x.TotalValue));

                model.Complaint = _Complaint.Where(x => x.Status == ComplaintStatus.Pending).Count();
                model.PendingApproval = _OrderMaster.Where(x => x.Status == OrderStatus.PendingApproval).Count();
                model.InProcess = _OrderMaster.Where(x => x.Status == OrderStatus.InProcess).Count();
                model.PartiallyProcessed = _OrderMaster.Where(x => x.Status == OrderStatus.PartiallyProcessed).Count();
                model.CompletelyProcessed = _OrderMaster.Where(x => x.Status == OrderStatus.CompletelyProcessed).Count();
                model.OnHold = _OrderMaster.Where(x => x.Status == OrderStatus.Onhold).Count();
                model.Reject = _OrderMaster.Where(x => x.Status == OrderStatus.Reject).Count();
                model.Draft = _OrderMaster.Where(x => x.Status == OrderStatus.Draft).Count();
                model.VerifiedPayment = _PaymentMaster.Where(x => x.Status == PaymentStatus.Verified).Sum(x => x.Amount);
                model.UnverifiedPayment = _PaymentMaster.Where(x => x.Status == PaymentStatus.Unverified).Sum(x => x.Amount);

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

            List<PaymentWiseComparision> PaymentWiseComparision = new List<PaymentWiseComparision>();
            foreach (var item in months.Item1)
            {
                PaymentWiseComparision PWSCurrent = new PaymentWiseComparision();
                PWSCurrent.Month = item.MonthName;
                PWSCurrent.CurrentYear = _OrderMaster.Where(x => x.CreatedDate.Year == item.Year && x.CreatedDate.Month == item.Month).Sum(x => x.TotalValue);
                PWSCurrent.LastYear = _OrderMaster.Where(x => x.CreatedDate.Year == item.LastYear && x.CreatedDate.Month == item.Month).Sum(x => x.TotalValue);
                PaymentWiseComparision.Add(PWSCurrent);
            }
            return PartialView("DistributorPaymentWise", PaymentWiseComparision);
        }
        public IActionResult GetDistributorOrderWiseComparision()
        {
            DateTime CurrentYear = DateTime.Now;
            DateTime LastYear = DateTime.Now.AddYears(-1).AddMonths(1);
            var months = ExtensionUtility.MonthsBetween(LastYear, CurrentYear);

            List<OrderWiseComparision> OrderWiseComparision = new List<OrderWiseComparision>();
            foreach (var item in months.Item1)
            {
                OrderWiseComparision OWSCurrent = new OrderWiseComparision();
                OWSCurrent.Month = item.MonthName;
                OWSCurrent.CurrentYear = _OrderMaster.Where(x => x.CreatedDate.Year == item.Year && x.CreatedDate.Month == item.Month).Count();
                OWSCurrent.LastYear = _OrderMaster.Where(x => x.CreatedDate.Year == item.LastYear && x.CreatedDate.Month == item.Month).Count();
                OrderWiseComparision.Add(OWSCurrent);
            }
            return PartialView("DistributorOrderWise", OrderWiseComparision);
        }
        public IActionResult GetDistributorPaymentWiseStatus()
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

            return PartialView("DistributorPaymentWiseStatus", PaymentWiseStatus);
        }
        public IActionResult GetDistributorRecentPaymentStatus()
        {
            DistributorRecentPaymentStatus model = new DistributorRecentPaymentStatus();
            DateTime CurrentYear = DateTime.Now;
            DateTime LastYear = DateTime.Now.AddYears(-1).AddMonths(1);
            var months = ExtensionUtility.MonthsBetween(LastYear, CurrentYear);

            model.RecentOrder = new List<RecentOrder>();
            model.RecentOrder = _OrderMaster.OrderByDescending(x => x.CreatedDate).Select(y => new RecentOrder
            {
                OrderNo = y.Id,
                DistributorName = y.Distributor.DistributorName,
                Status = y.Status,
                Amount = _OrderDetail != null && _OrderDetail.Count() > 0 ? _OrderDetail.Where(x => x.OrderId == y.Id).Sum(x => x.Amount) : 0

            }).Take(5).ToList();

            model.RecentPayment = new List<RecentPayment>();
            model.RecentPayment = _PaymentMaster.OrderByDescending(x => x.CreatedDate).Select(y => new RecentPayment
            {
                PaymentId = y.Id,
                PaymentMode = y.PaymentMode.PaymentName,
                DistributorName = y.Distributor.DistributorName,
                Status = y.Status,
                Amount = y.Amount
            }).Take(5).ToList();

            model.ProductViewModelOrder = new List<ProductViewModel>();
            model.ProductViewModelOrder = _OrderDetail.GroupBy(x => x.ProductId).Select(y => new ProductViewModel
            {
                ProductName = _ProductMaster.FirstOrDefault(x => x.Id == y.Key).ProductName,
                CurrentPrice = _ProductMaster.FirstOrDefault(x => x.Id == y.Key).ProductPrice.ToString(),
                OrderCount = y.Count()
            }).Take(5).ToList();

            model.ProductViewModelQuantity = new List<ProductViewModel>();
            model.ProductViewModelQuantity = _OrderDetail.GroupBy(x => x.ProductId).Select(y => new ProductViewModel
            {
                ProductName = _ProductMaster.FirstOrDefault(x => x.Id == y.Key).ProductName,
                CurrentPrice = _ProductMaster.FirstOrDefault(x => x.Id == y.Key).ProductPrice.ToString(),
                Quantity = y.Sum(x => x.Quantity)
            }).Take(5).ToList();

            return PartialView("DistributorRecentPaymentStatus", model);
        }
        public IActionResult GetDistributorPendingQuantity()
        {
            SessionHelper.SAPOrderPendingQuantity = _OrderBLL.GetDistributorPendingQuantity(SessionHelper.LoginUser.Distributor.DistributorSAPCode, _configuration);
            var pendingQuantity = SessionHelper.SAPOrderPendingQuantity;
            pendingQuantity.ForEach(x => x.Id = _ProductMaster.FirstOrDefault(y => y.SAPProductCode == x.ProductCode).Id);
            pendingQuantity.ForEach(x => x.ProductName = _ProductMaster.FirstOrDefault(y => y.SAPProductCode == x.ProductCode).ProductName + " " + _ProductMaster.FirstOrDefault(y => y.SAPProductCode == x.ProductCode).ProductDescription);
            SessionHelper.SAPOrderPendingQuantity = pendingQuantity;
            SessionHelper.SAPOrderPendingQuantity = SessionHelper.SAPOrderPendingQuantity.OrderByDescending(x => Convert.ToDouble(x.PendingQuantity)).ToList();
            return PartialView("DistributorPendingQuantity", SessionHelper.SAPOrderPendingQuantity);
        }
        #endregion

        #region Store Keeper
        public IActionResult StoreKeeperDashboard()
        {
            try
            {
                StoreKeeperDashboard model = new StoreKeeperDashboard();

                model.Submitted = _OrderReturnBLL.Where(x => x.Status == OrderReturnStatus.Submitted).Count();
                model.Received = _OrderReturnBLL.Where(x => x.Status == OrderReturnStatus.Received).Count();
                model.Completed = _OrderReturnBLL.Where(x => x.Status == OrderReturnStatus.CompletelyProcessed).Count();

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
                new AuditLogBLL(_unitOfWork).AddAuditLog("Home", "GetFile", " Start");
                string contenttype;
                string filename = Path.GetFileName(filepath);
                using (var provider = new PhysicalFileProvider(Path.GetDirectoryName(filepath)))
                {
                    var stream = provider.GetFileInfo(filename).CreateReadStream();
                    new FileExtensionContentTypeProvider().TryGetContentType(filename, out contenttype);
                    if (contenttype == "application/vnd.openxmlformats-officedocument.wordprocessingml.document")
                    {
                        new AuditLogBLL(_unitOfWork).AddAuditLog("Home", "GetFile", " End");
                        return File(stream, contenttype, filename.Split('_')[1]);
                    }
                    new AuditLogBLL(_unitOfWork).AddAuditLog("Home", "GetFile", " End");
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
