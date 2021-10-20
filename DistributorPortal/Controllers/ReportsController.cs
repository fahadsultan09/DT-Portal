using BusinessLogicLayer.Application;
using BusinessLogicLayer.ApplicationSetup;
using BusinessLogicLayer.ErrorLog;
using BusinessLogicLayer.GeneralSetup;
using BusinessLogicLayer.HelperClasses;
using DataAccessLayer.Repository;
using DataAccessLayer.WorkProcess;
using DistributorPortal.Resource;
using Microsoft.AspNetCore.Mvc;
using Models.Application;
using Models.ViewModel;
using Rotativa.AspNetCore;
using System;
using System.Collections.Generic;
using System.Linq;
using Utility;
using Utility.HelperClasses;

namespace DistributorPortal.Controllers
{
    public class ReportsController : BaseController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly OrderBLL _OrderBLL;
        private readonly OrderDetailBLL _OrderDetailBLL;
        private readonly OrderReturnBLL _OrderReturnBLL;
        private readonly OrderReturnDetailBLL _OrderReturnDetailBLL;
        private readonly PaymentBLL _PaymentBLL;
        private readonly ComplaintBLL _ComplaintBLL;
        private readonly OrderValueBLL _OrderValueBLL;
        private readonly DistributorWiseProductDiscountAndPricesBLL _DistributorWiseProductDiscountAndPricesBLL;
        private readonly ProductMasterBLL _ProductMasterBLL;
        private readonly ProductDetailBLL _ProductDetailBLL;
        private readonly ReportsBLL _ReportsBLL;
        private readonly Configuration _Configuration;
        private readonly IDapper _dapper;
        public ReportsController(IUnitOfWork unitOfWork,IDapper dapper, Configuration configuration)
        {
            _dapper = dapper; 
            _unitOfWork = unitOfWork;
            _OrderBLL = new OrderBLL(_unitOfWork);
            _OrderDetailBLL = new OrderDetailBLL(_unitOfWork);
            _OrderReturnBLL = new OrderReturnBLL(_unitOfWork);
            _OrderReturnDetailBLL = new OrderReturnDetailBLL(_unitOfWork);
            _PaymentBLL = new PaymentBLL(_unitOfWork);
            _ComplaintBLL = new ComplaintBLL(_unitOfWork);
            _OrderValueBLL = new OrderValueBLL(_unitOfWork);
            _DistributorWiseProductDiscountAndPricesBLL = new DistributorWiseProductDiscountAndPricesBLL(_unitOfWork);
            _ProductDetailBLL = new ProductDetailBLL(_unitOfWork);
            _ReportsBLL = new ReportsBLL();
            _Configuration = configuration;
            _ProductMasterBLL = new ProductMasterBLL(_unitOfWork);
        }

        #region Order
        public IActionResult Order()
        {
            return View();
        }
        public List<OrderMaster> GetOrderList(OrderSearch model)
        {
            List<OrderMaster> list = _OrderBLL.Search(model).Where(x => SessionHelper.LoginUser.IsDistributor == true ? x.DistributorId == SessionHelper.LoginUser.DistributorId : x.Status != OrderStatus.Canceled && x.Status != OrderStatus.Draft).ToList();
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
                return PartialView("OrderList", GetOrderList(model));
            }
        }
        [HttpGet]
        public IActionResult GetOrderDetailList(string DPID)
        {
            int id = 0;
            if (!string.IsNullOrEmpty(DPID))
            {
                int.TryParse(EncryptDecrypt.Decrypt(DPID), out id);
            }
            var Detail = _OrderDetailBLL.GetOrderDetailByIdByMasterId(id);
            return PartialView("OrderDetailList", Detail);
        }
        #endregion
        #region Order
        public IActionResult OrderPlantWise()
        {
            return View();
        }
        public List<OrderMaster> GetOrderPlantWiseList(OrderSearch model)
        {
            List<OrderMaster> list = _OrderBLL.Search(model).Where(x => SessionHelper.LoginUser.IsDistributor == true ? x.DistributorId == SessionHelper.LoginUser.DistributorId : x.Status != OrderStatus.Canceled && x.Status != OrderStatus.Draft).ToList();
            if (SessionHelper.LoginUser.IsDistributor)
            {
                list = _OrderBLL.Search(model).Where(x => SessionHelper.LoginUser.IsDistributor == true ? x.DistributorId == SessionHelper.LoginUser.DistributorId : true).ToList();
            }
            else
            {
                List<int> ProductMasterIds = _ProductDetailBLL.Where(x => x.PlantLocationId == SessionHelper.LoginUser.PlantLocationId).Select(x => x.ProductMasterId).Distinct().ToList();
                List<int> OrderIds = _OrderDetailBLL.Where(x => ProductMasterIds.Contains(x.ProductMaster.Id)).Select(x => x.OrderId).ToList();
                list = _OrderBLL.Search(model).Where(x => x.IsDeleted == false && OrderIds.Contains(x.Id) && x.Status != OrderStatus.Canceled && x.Status != OrderStatus.Draft).ToList();
            }
            return list;
        }
        [HttpPost]
        public IActionResult OrderPlantWiseSearch(OrderSearch model, string Search)
        {
            if (Search == "Search")
            {
                return PartialView("OrderPlantWiseList", GetOrderPlantWiseList(model));
            }
            else
            {
                return PartialView("OrderPlantWiseList", GetOrderPlantWiseList(model));
            }
        }
        [HttpGet]
        public IActionResult GetOrderPlantWiseDetailList(string DPID)
        {
            int id = 0;
            if (!string.IsNullOrEmpty(DPID))
            {
                int.TryParse(EncryptDecrypt.Decrypt(DPID), out id);
            }
            List<int> ProductMasterIds = _ProductDetailBLL.Where(x => x.PlantLocationId == SessionHelper.LoginUser.PlantLocationId).Select(x => x.ProductMasterId).Distinct().ToList();
            var Detail = _OrderDetailBLL.GetOrderDetailByIdByMasterId(id).Where(x => ProductMasterIds.Contains(x.ProductId)).ToList();
            return PartialView("OrderPlantWiseDetailList", Detail);
        }
        #endregion

        #region OrderReturn
        public IActionResult OrderReturn()
        {
            return View();
        }
        public List<OrderReturnMaster> GetOrderReturnList(OrderReturnSearch model)
        {
            List<OrderReturnMaster> list = _OrderReturnBLL.SearchReport(model).Where(x => SessionHelper.LoginUser.IsDistributor == true ? x.DistributorId == SessionHelper.LoginUser.DistributorId : true).ToList();
            if (SessionHelper.LoginUser.IsDistributor)
            {
                list = _OrderReturnBLL.SearchReport(model).Where(x => SessionHelper.LoginUser.IsDistributor == true ? x.DistributorId == SessionHelper.LoginUser.DistributorId : true).ToList();
            }
            else
            {
                List<int> ProductMasterIds = _ProductDetailBLL.Where(x => x.PlantLocationId == SessionHelper.LoginUser.PlantLocationId).Select(x => x.ProductMasterId).Distinct().ToList();
                List<int> OrderReturnIds = _OrderReturnDetailBLL.Where(x => ProductMasterIds.Contains(x.ProductMaster.Id)).Select(x => x.OrderReturnId).ToList();
                list = _OrderReturnBLL.SearchReport(model).Where(x => x.IsDeleted == false && OrderReturnIds.Contains(x.Id) && x.Status != OrderReturnStatus.Draft).ToList();
            }
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
                return PartialView("OrderReturnList", GetOrderReturnList(model));
            }
        }
        [HttpGet]
        public IActionResult GetOrderReturnDetailList(string DPID)
        {
            int id = 0;
            if (!string.IsNullOrEmpty(DPID))
            {
                int.TryParse(EncryptDecrypt.Decrypt(DPID), out id);
            }
            var Detail = _OrderReturnDetailBLL.GetOrderDetailByIdByMasterId(id);
            return PartialView("OrderReturnDetailList", Detail);
        }
        #endregion

        #region Payment
        public IActionResult Payment()
        {
            PaymentSearch paymentSearch = new PaymentSearch();
            paymentSearch.PaymentMasters = GetPaymentList(new PaymentSearch());
            return View(paymentSearch);
        }
        public List<PaymentMaster> GetPaymentList(PaymentSearch model)
        {
            List<PaymentMaster> list = _PaymentBLL.SearchReport(model).Where(x => SessionHelper.LoginUser.IsDistributor ? x.DistributorId == SessionHelper.LoginUser.DistributorId : SessionHelper.LoginUser.CompanyId != null ? x.CompanyId == SessionHelper.LoginUser.CompanyId : true).ToList();
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
                return PartialView("PaymentList", GetPaymentList(model));
            }
        }
        #endregion

        #region Complain
        public IActionResult Complaint()
        {
            return View();
        }
        public List<Complaint> GetComplaintList(ComplaintSearch model)
        {
            //List<Complaint> list = _ComplaintBLL.SearchReport(model).Where(x => SessionHelper.LoginUser.IsDistributor == true ? x.DistributorId == SessionHelper.LoginUser.DistributorId : true).ToList();
            //return list;
            List<Complaint> ComplaintList = new List<Complaint>();

            if (SessionHelper.LoginUser.IsDistributor)
            {
                ComplaintList = _ComplaintBLL.SearchReport(model).Where(x => x.DistributorId == SessionHelper.LoginUser.DistributorId).ToList();
                return ComplaintList;
            }
            if (SessionHelper.NavigationMenu.Where(x => x.ApplicationPage.ControllerName == "ComplaintSubCategory").Select(x => x.ApplicationAction.Id).Contains((int)ApplicationActions.IsAdmin))
            {
                ComplaintList = _ComplaintBLL.SearchReport(model);
            }
            else
            {
                int[] ComplaintSubCategoryIds = new ComplaintSubCategoryBLL(_unitOfWork).Where(x => x.UserEmailTo == SessionHelper.LoginUser.Id).Select(x => x.Id).ToArray();
                ComplaintList = _ComplaintBLL.SearchReport(model).Where(x => ComplaintSubCategoryIds.Contains(x.ComplaintSubCategoryId)).ToList();
            }
            return ComplaintList;
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
                return PartialView("ComplaintList", GetComplaintList(model));
            }
        }
        #endregion

        #region Common
        public IActionResult Print(ApplicationPages ApplicationPage, string DPID, string SiteTRNo, string KorangiTRNo, string SITEPhytek, string KorangiPhytek)
        {
            JsonResponse jsonResponse = new JsonResponse();
            try
            {
                List<OrderReturnDetail> List = new List<OrderReturnDetail>();
                int id = 0;
                if (!string.IsNullOrEmpty(DPID))
                {
                    int.TryParse(EncryptDecrypt.Decrypt(DPID), out id);
                }
                switch (ApplicationPage)
                {
                    case ApplicationPages.Order:
                        return new ViewAsPdf("PrintOrder", BindOrderMaster(id))
                        {
                            PageOrientation = Rotativa.AspNetCore.Options.Orientation.Landscape,
                            CustomSwitches = "--footer-left \" Use Controlled Copy Only \" --footer-center \" [page]/[toPage] \" --footer-right \" For Internal Use Only \"" +
                            " --footer-line --footer-font-size \"12\" --footer-spacing 1 --footer-font-name \"Segoe UI\""
                        };
                    case ApplicationPages.OrderReturn:
                        if (!string.IsNullOrEmpty(SiteTRNo))
                        {
                            List<OrderReturnDetail> OrderReturnDetailList = _OrderReturnDetailBLL.Where(x => x.OrderReturnId == id && x.PlantLocationId == 1).ToList();
                            OrderReturnDetailList.ForEach(x => x.TRNo = SiteTRNo);
                            _OrderReturnDetailBLL.UpdateRange(OrderReturnDetailList);
                            List.AddRange(_OrderReturnDetailBLL.GetOrderDetailByIdByMasterId(id).Where(x => x.OrderReturnId == id && x.PlantLocationId == 1).ToList());
                        }
                        if (!string.IsNullOrEmpty(KorangiTRNo))
                        {
                            List<OrderReturnDetail> OrderReturnDetailList = _OrderReturnDetailBLL.Where(x => x.OrderReturnId == id && x.PlantLocationId == 2).ToList();
                            OrderReturnDetailList.ForEach(x => x.TRNo = KorangiTRNo);
                            _OrderReturnDetailBLL.UpdateRange(OrderReturnDetailList);
                            List.AddRange(_OrderReturnDetailBLL.GetOrderDetailByIdByMasterId(id).Where(x => x.OrderReturnId == id && x.PlantLocationId == 2).ToList());
                        }
                        if (!string.IsNullOrEmpty(SITEPhytek))
                        {
                            List<OrderReturnDetail> OrderReturnDetailList = _OrderReturnDetailBLL.Where(x => x.OrderReturnId == id && x.PlantLocationId == 3).ToList();
                            OrderReturnDetailList.ForEach(x => x.TRNo = SITEPhytek);
                            _OrderReturnDetailBLL.UpdateRange(OrderReturnDetailList);
                            List.AddRange(_OrderReturnDetailBLL.GetOrderDetailByIdByMasterId(id).Where(x => x.OrderReturnId == id && x.PlantLocationId == 3).ToList());
                        }
                        if (!string.IsNullOrEmpty(KorangiPhytek))
                        {
                            List<OrderReturnDetail> OrderReturnDetailList = _OrderReturnDetailBLL.Where(x => x.OrderReturnId == id && x.PlantLocationId == 4).ToList();
                            OrderReturnDetailList.ForEach(x => x.TRNo = KorangiPhytek);
                            _OrderReturnDetailBLL.UpdateRange(OrderReturnDetailList);
                            List.AddRange(_OrderReturnDetailBLL.GetOrderDetailByIdByMasterId(id).Where(x => x.OrderReturnId == id && x.PlantLocationId == 4).ToList());
                        }
                        if (string.IsNullOrEmpty(KorangiTRNo) && string.IsNullOrEmpty(SiteTRNo) && string.IsNullOrEmpty(SITEPhytek) && string.IsNullOrEmpty(KorangiPhytek))
                        {
                            List = _OrderReturnDetailBLL.GetOrderDetailByIdByMasterId(id).Where(x => x.OrderReturnId == id).ToList();
                        }
                        return new ViewAsPdf("PrintOrderReturn", List)
                        {
                            PageOrientation = Rotativa.AspNetCore.Options.Orientation.Landscape,
                            CustomSwitches = "--footer-left \" Use Controlled Copy Only \" --footer-center \" [page]/[toPage] \" --footer-right \" For Internal Use Only \"" +
                            " --footer-line --footer-font-size \"12\" --footer-spacing 1 --footer-font-name \"Segoe UI\""
                            //CustomSwitches = "--footer-center \" Created Date: " + DateTime.Now.Date.ToString("dd/MM/yyyy") + "  Page: [page]/[toPage]\"" + " --footer-line --footer-font-size \"12\" --footer-spacing 1 --footer-font-name \"Segoe UI\""
                        };

                }
                jsonResponse.Status = false;
                jsonResponse.Message = NotificationMessage.ErrorOccurred;
                return Json(new { data = jsonResponse });

            }
            catch (Exception ex)
            {
                jsonResponse.Status = false;
                jsonResponse.Message = NotificationMessage.ErrorOccurred;
                new ErrorLogBLL(_unitOfWork).AddExceptionLog(ex);
                return Json(new { data = jsonResponse });
            }
        }

        private OrderMaster BindOrderMaster(int Id)
        {
            OrderMaster model = _OrderBLL.GetOrderMasterById(Id);
            List<DistributorWiseProductDiscountAndPrices> DistributorWiseProductDiscountAndPricesList = _DistributorWiseProductDiscountAndPricesBLL.Where(x => x.DistributorId == model.DistributorId).ToList();
            model.OrderDetail = _OrderDetailBLL.GetOrderDetailByIdByMasterId(Id);
            model.OrderDetail.ForEach(x => x.ProductDetail = _ProductDetailBLL.FirstOrDefault(y => y.ProductMasterId == x.ProductMaster.Id) != null ? _ProductDetailBLL.FirstOrDefault(y => y.ProductMasterId == x.ProductMaster.Id) : null);
            model.OrderValueViewModel = _OrderBLL.GetOrderValueModel(_OrderValueBLL.GetOrderValueByOrderId(Id));
            return model;
        }
        #endregion

        #region Customer Ledger
        public IActionResult CustomerLedger()
        {
            CustomerLedgerSeach customerLedgerSeach = new CustomerLedgerSeach();
            customerLedgerSeach.DistributorSAPCodeList = new DistributorBLL(_unitOfWork).DropDownDistributorList();
            customerLedgerSeach.CompanyList = new CompanyBLL(_unitOfWork).DropDownCompanyList();
            return View(customerLedgerSeach);
        }
        [HttpPost]
        public IActionResult CustomerLedgerViewPDF(CustomerLedgerSeach customerLedgerSeach)
        {
            JsonResponse jsonResponse = new JsonResponse();
            try
            {
                ViewBag.CustomerLedgerViewPDF = _ReportsBLL.CustomerLedger(customerLedgerSeach, _Configuration);
                return PartialView();
            }
            catch (Exception ex)
            {
                jsonResponse.Status = false;
                jsonResponse.Message = NotificationMessage.ErrorOccurred;
                new ErrorLogBLL(_unitOfWork).AddExceptionLog(ex);
                return Json(new { data = jsonResponse });
            }
        }
        #endregion
        #region Invoice Ledger
        public IActionResult Invoice()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Invoice(InvoiceSearch invoiceSearch)
        {
            JsonResponse jsonResponse = new JsonResponse();
            try
            {
                ViewBag.InvoiceViewPDF = _ReportsBLL.Invoice(invoiceSearch, _Configuration);
                return PartialView("InvoiceViewPDF");
            }
            catch (Exception ex)
            {
                jsonResponse.Status = false;
                jsonResponse.Message = NotificationMessage.ErrorOccurred;
                new ErrorLogBLL(_unitOfWork).AddExceptionLog(ex);
                return Json(new { data = jsonResponse });
            }
        }
        #endregion
        #region Sale Return Credit Note
        public IActionResult CreditNote()
        {
            return View();
        }
        public IActionResult CreditNoteViewPDF(SaleReturnCreditNoteSearch saleReturnCreditNoteSearch)
        {
            JsonResponse jsonResponse = new JsonResponse();
            try
            {
                ViewBag.InvoiceViewPDF = _ReportsBLL.SaleReturnCreditNote(saleReturnCreditNoteSearch, _Configuration);
                return PartialView();
            }
            catch (Exception ex)
            {
                jsonResponse.Status = false;
                jsonResponse.Message = NotificationMessage.ErrorOccurred;
                new ErrorLogBLL(_unitOfWork).AddExceptionLog(ex);
                return Json(new { data = jsonResponse });
            }
        }
        #endregion
        #region Customer Balance report
        public IActionResult CustomerBalance()
        {
            return View();
        }
        public IActionResult CustomerBalanceView(CustomerBalanceSearch customerBalanceSearch)
        {
            JsonResponse jsonResponse = new JsonResponse();
            try
            {
                List<CustomerBalanceViewModel> customerBalanceViewModels = _ReportsBLL.CustomerBalance(customerBalanceSearch, _Configuration);
                return PartialView(customerBalanceViewModels);
            }
            catch (Exception ex)
            {
                jsonResponse.Status = false;
                jsonResponse.Message = NotificationMessage.ErrorOccurred;
                new ErrorLogBLL(_unitOfWork).AddExceptionLog(ex);
                return Json(new { data = jsonResponse });
            }
        }
        #endregion
        #region ProductPending
        public IActionResult ProductPending()
        {
            return View();
        }
        public List<ProductPending> GetProductList(ProductPendingSearch model)
        {
            List<ProductPending> pendings = new List<ProductPending>();
            double pendingValue = 0;
            List<ProductPending> list = _DistributorWiseProductDiscountAndPricesBLL.GetProductPendings(model, _dapper);
            
            foreach(var item in list)
            {
                pendingValue = _OrderBLL.CalculatePendingValue(item.PendingQuantity, item.Rate, item.Discount, item.SalesTax + item.AdditionalSalesTax, item.IncomeTax);
                pendings.Add(new ProductPending()
                {
                    SAPProductCode = item.SAPProductCode,
                    ProductName = item.ProductName,
                    PackSize = item.PackSize,
                    CompanyName = item.CompanyName,
                    PendingQuantity = item.PendingQuantity,
                    Rate = item.Rate,
                    IncomeTax = item.IncomeTax,
                    SalesTax = item.SalesTax,
                    AdditionalSalesTax = item.AdditionalSalesTax,
                    PendingValue = pendingValue,
                    Status = item.Status,
                    CompanyId = item.CompanyId,
                    productId = item.productId,
                    DistributorId = item.DistributorId,
                    Discount = item.Discount


                });
            }
            return pendings;
        }
        [HttpPost]
        public IActionResult ProductPendingSearch(ProductPendingSearch model, string Search)
        {
            if (Search == "Search")
            {
                return PartialView("ProductPendingList", GetProductList(model));
            }
            else
            {
                return PartialView("ProductPendingList", GetProductList(model));
            }
        }
        #endregion
    }
}
