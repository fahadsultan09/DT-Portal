using BusinessLogicLayer.Application;
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
using System.Globalization;
using System.IO;
using System.Linq;
using Utility;

namespace DistributorPortal.Controllers
{
    public class HomeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly OrderBLL _OrderBLL;
        private readonly OrderDetailBLL _OrderDetailBLL;
        private readonly PaymentBLL _PaymentBLL;
        public HomeController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _OrderBLL = new OrderBLL(_unitOfWork);
            _OrderDetailBLL = new OrderDetailBLL(_unitOfWork);
            _PaymentBLL = new PaymentBLL(_unitOfWork);
        }
        public IActionResult Index()
        {
            if (SessionHelper.LoginUser.Role.Id == 4)
            {
                return RedirectToAction("AdminDashboard");

            }
            else if (SessionHelper.LoginUser.Role.Id == 3)
            {
                return RedirectToAction("DistributorDashboard");
            }
            else if (SessionHelper.LoginUser.Role.Id == 2)
            {
                return RedirectToAction("AccountDashboard");
            }
            else
            {
                return RedirectToAction("AdminDashboard");
            }
        }
        public IActionResult AdminDashboard()
        {
            try
            {
                AdminDashboardViewModel model = new AdminDashboardViewModel();
                DateTime CurrentYear = DateTime.Now;
                DateTime LastYear = DateTime.Now.AddYears(-1).AddMonths(1);
                var months = ExtensionUtility.MonthsBetween(LastYear, CurrentYear);
                List<OrderMaster> _OrderMaster = _OrderBLL.GetAllOrderMaster().ToList();
                List<OrderDetail> _OrderDetail = _OrderDetailBLL.GetAllOrderDetail().ToList();
                List<PaymentMaster> _PaymentMaster = _PaymentBLL.GetAllPaymentMaster().ToList();

                model.PendingApproval = _OrderMaster.Where(x => x.Status == OrderStatus.PendingApproval).Count();
                model.InProcess = _OrderMaster.Where(x => x.Status == OrderStatus.InProcess).Count();
                model.PartiallyProcessed = _OrderMaster.Where(x => x.Status == OrderStatus.PartiallyProcessed).Count();
                model.CompletelyProcessed = _OrderMaster.Where(x => x.Status == OrderStatus.CompletelyProcessed).Count();
                model.OnHold = _OrderMaster.Where(x => x.Status == OrderStatus.Onhold).Count();
                model.Reject = _OrderMaster.Where(x => x.Status == OrderStatus.Reject).Count();
                model.PendingOrder = model.PendingApproval + model.InProcess + model.PartiallyProcessed;
                model.VerifiedPayment = _PaymentMaster.Where(x => x.Status == PaymentStatus.Verified).Sum(x => x.Amount);
                model.UnverifiedPayment = _PaymentMaster.Where(x => x.Status == PaymentStatus.Unverified).Sum(x => x.Amount);

                model.OrderWiseComparision = new List<OrderWiseComparision>();
                foreach (var item in months.Item1)
                {
                    OrderWiseComparision OWSCurrent = new OrderWiseComparision();
                    OWSCurrent.Month = item.MonthName;
                    OWSCurrent.CurrentYear = _OrderMaster.Where(x => x.CreatedDate.Year == item.Year && x.CreatedDate.Month == item.Month).Count();
                    OWSCurrent.LastYear = _OrderMaster.Where(x => x.CreatedDate.Year == item.LastYear && x.CreatedDate.Month == item.Month).Count();
                    model.OrderWiseComparision.Add(OWSCurrent);
                }

                model.PaymentWiseComparision = new List<PaymentWiseComparision>();
                foreach (var item in months.Item1)
                {
                    PaymentWiseComparision PWSCurrent = new PaymentWiseComparision();
                    PWSCurrent.Month = item.MonthName;
                    PWSCurrent.CurrentYear = _PaymentMaster.Where(x => x.CreatedDate.Year == item.Year && x.CreatedDate.Month == item.Month).Sum(x => x.Amount);
                    PWSCurrent.LastYear = _PaymentMaster.Where(x => x.CreatedDate.Year == item.LastYear && x.CreatedDate.Month == item.Month).Sum(x => x.Amount);
                    model.PaymentWiseComparision.Add(PWSCurrent);
                }

                model.PaymentWiseStatus = new List<PaymentWiseStatus>();
                foreach (var item in months.Item1)
                {
                    PaymentWiseStatus PWSCurrent = new PaymentWiseStatus();
                    PWSCurrent.Month = item.MonthName;
                    PWSCurrent.VerifiedPayment = _PaymentMaster.Where(x => x.CreatedDate.Year == item.Year && x.CreatedDate.Month == item.Month && x.Status == PaymentStatus.Verified).Sum(x => x.Amount);
                    PWSCurrent.UnverifiedPayment = _PaymentMaster.Where(x => x.CreatedDate.Year == item.Year && x.CreatedDate.Month == item.Month && x.Status == PaymentStatus.Unverified).Sum(x => x.Amount);
                    model.PaymentWiseStatus.Add(PWSCurrent);
                }
                model.RegionWiseOrder = new List<RegionWiseOrder>();
                model.RegionWiseOrder = _OrderMaster.GroupBy(x => x.Distributor.Region.RegionName).Select(y => new RegionWiseOrder
                {
                    Region = y.Key.ToString(),
                    OrderCount = y.Count()
                }).ToList();

                model.DistributorViewModelOrder = new List<DistributorViewModel>();
                model.DistributorViewModelOrder = _OrderMaster.GroupBy(x => new { x.Distributor.DistributorSAPCode, x.Distributor.DistributorName }).Select(y => new DistributorViewModel
                {
                    DistributorName = y.Key.DistributorName.ToString(),
                    OrderCount = y.Count()
                }).Take(5).ToList();

                model.DistributorViewModelPayment = new List<DistributorViewModel>();
                model.DistributorViewModelPayment = _PaymentMaster.GroupBy(x => new { x.Id, x.Distributor.DistributorSAPCode, x.Distributor.DistributorName }).Select(y => new DistributorViewModel
                {
                    DistributorName = y.Key.DistributorName.ToString(),
                    Payment = y.Sum(x => x.Amount)
                }).Take(5).ToList();

                model.ProductViewModelOrder = new List<ProductViewModel>();
                model.ProductViewModelOrder = _OrderDetail.GroupBy(x => new { x.ProductId, x.ProductMaster }).Select(y => new ProductViewModel
                {
                    ProductName = y.Key.ProductMaster.ProductName,
                    CurrentPrice = y.Key.ProductMaster.ProductPrice.ToString(),
                    OrderCount = y.Count()
                }).Take(5).ToList();

                model.ProductViewModelQuantity = new List<ProductViewModel>();
                model.ProductViewModelQuantity = _OrderDetail.GroupBy(x => new { x.ProductId, x.ProductMaster }).Select(y => new ProductViewModel
                {
                    ProductName = y.Key.ProductMaster.ProductName,
                    CurrentPrice = y.Key.ProductMaster.ProductPrice.ToString(),
                    Quantity = y.Sum(x => x.Quantity)
                }).Take(5).ToList();

                return View(model);
            }
            catch (Exception ex)
            {
                new ErrorLogBLL(_unitOfWork).AddExceptionLog(ex);
                return View();
            }
        }
        public IActionResult AccountDashboard()
        {
            AccountDashboardViewModel model = new AccountDashboardViewModel();
            List<OrderMaster> _OrderMaster = _OrderBLL.GetAllOrderMaster().ToList();
            List<OrderDetail> _OrderDetail = _OrderDetailBLL.GetAllOrderDetail().ToList();
            List<PaymentMaster> _PaymentMaster = _PaymentBLL.GetAllPaymentMaster().ToList();

            model.PaymentWiseComparision = new List<PaymentWiseComparision>();
            model.VerifiedPayment = ExtensionUtility.FormatNumberAmount(_PaymentMaster.Where(x => x.Status == PaymentStatus.Verified).Sum(x => x.Amount));
            model.UnverifiedPayment = ExtensionUtility.FormatNumberAmount(_PaymentMaster.Where(x => x.Status == PaymentStatus.Unverified).Sum(x => x.Amount));
            model.TodayVerifiedPayment = ExtensionUtility.FormatNumberAmount(_PaymentMaster.Where(x => x.Status == PaymentStatus.Verified && x.CreatedDate.Date == DateTime.Now.Date).Sum(x => x.Amount));
            DateTime CurrentYear = DateTime.Now;
            DateTime LastYear = DateTime.Now.AddYears(-1).AddMonths(1);
            var months = ExtensionUtility.MonthsBetween(LastYear, CurrentYear);

            model.PaymentWiseComparision = new List<PaymentWiseComparision>();
            foreach (var item in months.Item1)
            {
                PaymentWiseComparision PWSCurrent = new PaymentWiseComparision();
                PWSCurrent.Month = item.MonthName;
                PWSCurrent.CurrentYear = _PaymentMaster.Where(x => x.CreatedDate.Year == item.Year && x.CreatedDate.Month == item.Month).Sum(x => x.Amount);
                PWSCurrent.LastYear = _PaymentMaster.Where(x => x.CreatedDate.Year == item.LastYear && x.CreatedDate.Month == item.Month).Sum(x => x.Amount);
                model.PaymentWiseComparision.Add(PWSCurrent);
            }

            model.PaymentWiseStatus = new List<PaymentWiseStatus>();
            foreach (var item in months.Item1)
            {
                PaymentWiseStatus PWSCurrent = new PaymentWiseStatus();
                PWSCurrent.Month = item.MonthName;
                PWSCurrent.VerifiedPayment = _PaymentMaster.Where(x => x.CreatedDate.Year == item.Year && x.CreatedDate.Month == item.Month && x.Status == PaymentStatus.Verified).Sum(x => x.Amount);
                PWSCurrent.UnverifiedPayment = _PaymentMaster.Where(x => x.CreatedDate.Year == item.Year && x.CreatedDate.Month == item.Month && x.Status == PaymentStatus.Unverified).Sum(x => x.Amount);
                model.PaymentWiseStatus.Add(PWSCurrent);
            }
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

            return View(model);
        }
        public IActionResult DistributorDashboard()
        {
            DistributorDashboardViewModel model = new DistributorDashboardViewModel();
            List<OrderMaster> _OrderMaster = _OrderBLL.GetAllOrderMaster().Where(x => x.DistributorId == SessionHelper.LoginUser.DistributorId).ToList();
            List<OrderDetail> _OrderDetail = _OrderDetailBLL.GetAllOrderDetail().Where(x => x.OrderMaster.DistributorId == SessionHelper.LoginUser.DistributorId).ToList();
            List<PaymentMaster> _PaymentMaster = _PaymentBLL.GetAllPaymentMaster().Where(x => x.DistributorId == SessionHelper.LoginUser.DistributorId).ToList();
            model.InProcessOrder = _OrderMaster.Where(x => x.Status != OrderStatus.Draft && x.Status != OrderStatus.CompletelyProcessed && x.Status != OrderStatus.Reject).Count();
            model.UnverifiedPaymentAll = ExtensionUtility.FormatNumberAmount(_PaymentMaster.Where(x => x.CreatedDate.Year == DateTime.Now.Year && x.Status == PaymentStatus.Unverified).Sum(x => x.Amount));

            DateTime CurrentYear = DateTime.Now;
            DateTime LastYear = DateTime.Now.AddYears(-1).AddMonths(1);
            var months = ExtensionUtility.MonthsBetween(LastYear, CurrentYear);

            model.PendingApproval = _OrderMaster.Where(x => x.Status == OrderStatus.PendingApproval).Count();
            model.InProcess = _OrderMaster.Where(x => x.Status == OrderStatus.InProcess).Count();
            model.PartiallyProcessed = _OrderMaster.Where(x => x.Status == OrderStatus.PartiallyProcessed).Count();
            model.CompletelyProcessed = _OrderMaster.Where(x => x.Status == OrderStatus.CompletelyProcessed).Count();
            model.OnHold = _OrderMaster.Where(x => x.Status == OrderStatus.Onhold).Count();
            model.Reject = _OrderMaster.Where(x => x.Status == OrderStatus.Reject).Count();
            model.Draft = _OrderMaster.Where(x => x.Status == OrderStatus.Draft).Count();
            model.VerifiedPayment = _PaymentMaster.Where(x => x.Status == PaymentStatus.Verified).Sum(x => x.Amount);
            model.UnverifiedPayment = _PaymentMaster.Where(x => x.Status == PaymentStatus.Unverified).Sum(x => x.Amount);

            model.OrderWiseComparision = new List<OrderWiseComparision>();
            foreach (var item in months.Item1)
            {
                OrderWiseComparision OWSCurrent = new OrderWiseComparision();
                OWSCurrent.Month = item.MonthName;
                OWSCurrent.CurrentYear = _OrderMaster.Where(x => x.CreatedDate.Year == item.Year && x.CreatedDate.Month == item.Month).Count();
                OWSCurrent.LastYear = _OrderMaster.Where(x => x.CreatedDate.Year == item.LastYear && x.CreatedDate.Month == item.Month).Count();
                model.OrderWiseComparision.Add(OWSCurrent);
            }

            model.PaymentWiseComparision = new List<PaymentWiseComparision>();
            foreach (var item in months.Item1)
            {
                PaymentWiseComparision PWSCurrent = new PaymentWiseComparision();
                PWSCurrent.Month = item.MonthName;
                PWSCurrent.CurrentYear = _PaymentMaster.Where(x => x.CreatedDate.Year == item.Year && x.CreatedDate.Month == item.Month).Sum(x => x.Amount);
                PWSCurrent.LastYear = _PaymentMaster.Where(x => x.CreatedDate.Year == item.LastYear && x.CreatedDate.Month == item.Month).Sum(x => x.Amount);
                model.PaymentWiseComparision.Add(PWSCurrent);
            }

            model.PaymentWiseStatus = new List<PaymentWiseStatus>();
            foreach (var item in months.Item1)
            {
                PaymentWiseStatus PWSCurrent = new PaymentWiseStatus();
                PWSCurrent.Month = item.MonthName;
                PWSCurrent.VerifiedPayment = _PaymentMaster.Where(x => x.CreatedDate.Year == item.Year && x.CreatedDate.Month == item.Month && x.Status == PaymentStatus.Verified).Sum(x => x.Amount);
                PWSCurrent.UnverifiedPayment = _PaymentMaster.Where(x => x.CreatedDate.Year == item.Year && x.CreatedDate.Month == item.Month && x.Status == PaymentStatus.Unverified).Sum(x => x.Amount);
                model.PaymentWiseStatus.Add(PWSCurrent);
            }

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
            model.ProductViewModelOrder = _OrderDetail.GroupBy(x => new { x.ProductId, x.ProductMaster }).Select(y => new ProductViewModel
            {
                ProductName = y.Key.ProductMaster.ProductName,
                CurrentPrice = y.Key.ProductMaster.ProductPrice.ToString(),
                OrderCount = y.Count()
            }).Take(5).ToList();

            model.ProductViewModelQuantity = new List<ProductViewModel>();
            model.ProductViewModelQuantity = _OrderDetail.GroupBy(x => new { x.ProductId, x.ProductMaster }).Select(y => new ProductViewModel
            {
                ProductName = y.Key.ProductMaster.ProductName,
                CurrentPrice = y.Key.ProductMaster.ProductPrice.ToString(),
                Quantity = y.Sum(x => x.Quantity)
            }).Take(5).ToList();

            return View(model);
        }
        public IActionResult GetFile(string filepath)
        {
            try
            {
                string contenttype;
                string filename = Path.GetFileName(filepath);
                using (var provider = new PhysicalFileProvider(Path.GetDirectoryName(filepath)))
                {
                    var stream = provider.GetFileInfo(filename).CreateReadStream();
                    new FileExtensionContentTypeProvider().TryGetContentType(filename, out contenttype);
                    if (contenttype == "application/vnd.openxmlformats-officedocument.wordprocessingml.document" || contenttype == "image/jpeg" || contenttype == "image/png")
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
    }
}
