﻿using BusinessLogicLayer.Application;
using BusinessLogicLayer.ApplicationSetup;
using BusinessLogicLayer.ErrorLog;
using BusinessLogicLayer.GeneralSetup;
using BusinessLogicLayer.HelperClasses;
using DataAccessLayer.WorkProcess;
using DistributorPortal.Resource;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Configuration;
using Models.Application;
using Models.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Utility;
using Utility.HelperClasses;
using static Utility.Constant.Common;

namespace DistributorPortal.Controllers
{
    public class OrderFormController : BaseController
    {
        private readonly ProductDetailBLL _productDetailBLL;
        private readonly IUnitOfWork _unitOfWork;
        private readonly OrderBLL _OrderBLL;
        private readonly DistributorWiseProductDiscountAndPricesBLL discountAndPricesBll;
        private readonly ProductMasterBLL _ProductMasterBLL;
        private readonly OrderDetailBLL _orderDetailBll;
        private readonly OrderValueBLL _orderValueBll;
        private readonly DistributorLicenseBLL _DistributorLicenseBLL;
        private readonly Configuration _Configuration;
        private readonly IConfiguration _IConfiguration;
        private readonly NotificationBLL _NotificationBLL;
        private readonly UserBLL _UserBLL;
        private readonly IWebHostEnvironment _env;
        private readonly EmailLogBLL _EmailLogBLL;
        private readonly DistributorBLL _distributorBll;
        private readonly DistributorPendingQuanityBLL _DistributorPendingQuanityBLL;
        private readonly PaymentBLL _PaymentBLL;
        private readonly ICompositeViewEngine _viewEngine;
        public OrderFormController(IUnitOfWork unitOfWork, ICompositeViewEngine viewEngine, Configuration configuration, IConfiguration iConfiguration, IWebHostEnvironment env)
        {
            _viewEngine = viewEngine;
            _unitOfWork = unitOfWork;
            _env = env;
            _OrderBLL = new OrderBLL(_unitOfWork);
            _ProductMasterBLL = new ProductMasterBLL(_unitOfWork);
            _productDetailBLL = new ProductDetailBLL(unitOfWork);
            _Configuration = configuration;
            _IConfiguration = iConfiguration;
            _orderDetailBll = new OrderDetailBLL(_unitOfWork);
            discountAndPricesBll = new DistributorWiseProductDiscountAndPricesBLL(_unitOfWork);
            _orderValueBll = new OrderValueBLL(_unitOfWork);
            _DistributorLicenseBLL = new DistributorLicenseBLL(_unitOfWork);
            _NotificationBLL = new NotificationBLL(_unitOfWork);
            _UserBLL = new UserBLL(_unitOfWork);
            _EmailLogBLL = new EmailLogBLL(_unitOfWork, _Configuration);
            _distributorBll = new DistributorBLL(_unitOfWork);
            _DistributorPendingQuanityBLL = new DistributorPendingQuanityBLL(_unitOfWork);
            _PaymentBLL = new PaymentBLL(_unitOfWork);
        }
        public IActionResult Index()
        {
            var model = _productDetailBLL.GetAllProductDetail();
            return View(model);
        }
        public IActionResult Create(string DPID)
        {
            JsonResponse jsonResponse = new JsonResponse();
            OrderViewModel model = new OrderViewModel();
            SessionHelper.AddDistributorWiseProduct = new List<DistributorWiseProductDiscountAndPrices>();
            try
            {
                var distributorProduct = discountAndPricesBll.Where(e => e.DistributorId == SessionHelper.LoginUser.DistributorId && e.ProductDetailId != null && (e.ProductDetail.ProductVisibilityId == ProductVisibility.Visible || e.ProductDetail.ProductVisibilityId == ProductVisibility.OrderDispatch)).OrderBy(x => x.ProductDescription).ToList();
                List<int> LicenseId = new List<int>();
                List<DistributorLicense> distributorLicenses = _DistributorLicenseBLL.Where(e => e.IsActive && e.Status == LicenseStatus.Verified && e.Expiry > DateTime.Now && e.DistributorId == SessionHelper.LoginUser.DistributorId).ToList();
                foreach (var item in distributorLicenses.Select(x => x.LicenseId))
                {
                    LicenseId.Add(item);
                }

                ViewBag.LicenseId = LicenseId;
                if (!string.IsNullOrEmpty(DPID))
                {
                    int.TryParse(EncryptDecrypt.Decrypt(DPID), out int id);
                    var order = _OrderBLL.FirstOrDefault(e => e.Id == id);
                    var orderDetails = _orderDetailBll.Where(e => e.OrderId == id);
                    if (SessionHelper.DistributorPendingQuantity != null && SessionHelper.DistributorPendingQuantity.Count() > 0)
                    {
                        distributorProduct.ForEach(x => x.PendingQuantity = SessionHelper.DistributorPendingQuantity.FirstOrDefault(e => e.ProductCode == x.SAPProductCode) == null ? 0 : SessionHelper.DistributorPendingQuantity.First(y => y.ProductCode == x.SAPProductCode).PendingQuantity);
                    }
                    else
                    {
                        SessionHelper.DistributorPendingQuantity = _OrderBLL.DistributorPendingQuantity((int)SessionHelper.LoginUser.DistributorId);
                        if (SessionHelper.DistributorPendingQuantity != null && SessionHelper.DistributorPendingQuantity.Count() > 0)
                        {
                            distributorProduct.ForEach(x => x.PendingQuantity = SessionHelper.DistributorPendingQuantity.FirstOrDefault(e => e.ProductCode == x.SAPProductCode) == null ? 0 : SessionHelper.DistributorPendingQuantity.First(y => y.ProductCode == x.SAPProductCode).PendingQuantity);
                        }
                    }
                    if (SessionHelper.LoginUser.IsDistributor && SessionHelper.DistributorBalance == null)
                    {
                        SessionHelper.DistributorBalance = _PaymentBLL.GetDistributorBalance(SessionHelper.LoginUser.Distributor.DistributorSAPCode, _Configuration);
                    }
                    distributorProduct.ForEach(x => x.ProductDetail.TotalPrice = orderDetails.FirstOrDefault(e => e.ProductId == x.ProductDetail.ProductMaster.Id) != null ? orderDetails.FirstOrDefault(e => e.ProductId == x.ProductDetail.ProductMaster.Id).Amount : 0);
                    distributorProduct.ForEach(x => x.ProductDetail.QuanityCarton = orderDetails.FirstOrDefault(e => e.ProductId == x.ProductDetail.ProductMaster.Id) != null ? orderDetails.FirstOrDefault(e => e.ProductId == x.ProductDetail.ProductMaster.Id).QuanityCarton : 0);
                    distributorProduct.ForEach(x => x.ProductDetail.QuanityLoose = orderDetails.FirstOrDefault(e => e.ProductId == x.ProductDetail.ProductMaster.Id) != null ? orderDetails.FirstOrDefault(e => e.ProductId == x.ProductDetail.ProductMaster.Id).QuanityLoose : 0);
                    distributorProduct.ForEach(x => x.ProductDetail.QuanitySF = orderDetails.FirstOrDefault(e => e.ProductId == x.ProductDetail.ProductMaster.Id) != null ? orderDetails.FirstOrDefault(e => e.ProductId == x.ProductDetail.ProductMaster.Id).QuanitySF : 0);
                    foreach (var item in orderDetails)
                    {
                        distributorProduct.First(e => e.ProductDetail.ProductMasterId == item.ProductId).ProductDetail.ProductMaster.Quantity = item.Quantity;
                    }
                    model.ReferenceNo = order.ReferenceNo;
                    model.Attachment = order.Attachment;
                    model.Remarks = order.Remarks;
                    model.OrderValues = _OrderBLL.GetOrderValueModel(_orderValueBll.GetOrderValueByOrderId(id));
                    SessionHelper.TotalOrderValue = orderDetails.First().OrderMaster.TotalValue.ToString("#,##0.00");
                    model.Id = id;
                    distributorProduct.ForEach(x => x.SalesTax = SessionHelper.LoginUser.Distributor.IsSalesTaxApplicable ? x.ProductDetail.SalesTax : x.ProductDetail.SalesTax + x.ProductDetail.AdditionalSalesTax);
                    distributorProduct.ForEach(x => x.IncomeTax = SessionHelper.LoginUser.Distributor.IsIncomeTaxApplicable ? x.ProductDetail.IncomeTax : x.ProductDetail.IncomeTax * 2);
                    distributorProduct.ForEach(x => x.AdditionalSalesTax = x.ProductDetail.AdditionalSalesTax);
                    distributorProduct.ForEach(x => x.ViewSalesTax = x.ProductDetail.SalesTax);
                    SessionHelper.AddDistributorWiseProduct = model.ProductDetails = distributorProduct.ToList();
                    return View("AddOrder", model);
                }
                else
                {
                    List<DistributorPendingQuantity> pendingQuantity = new List<DistributorPendingQuantity>();
                    if (SessionHelper.LoginUser.IsDistributor && SessionHelper.DistributorBalance == null)
                    {
                        SessionHelper.DistributorBalance = _PaymentBLL.GetDistributorBalance(SessionHelper.LoginUser.Distributor.DistributorSAPCode, _Configuration);
                    }
                    if (SessionHelper.LoginUser.Distributor != null)
                    {
                        SessionHelper.DistributorPendingQuantity = _DistributorPendingQuanityBLL.Where(x => x.DistributorId == SessionHelper.LoginUser.DistributorId).ToList();
                        List<ProductMaster> _ProductMaster = _ProductMasterBLL.GetAllProductMaster().ToList();
                        pendingQuantity = SessionHelper.DistributorPendingQuantity = _DistributorPendingQuanityBLL.Where(x => x.DistributorId == SessionHelper.LoginUser.DistributorId).ToList();
                        pendingQuantity.ForEach(x => x.Id = _ProductMaster.FirstOrDefault(y => y.SAPProductCode == x.ProductCode) != null ? _ProductMaster.FirstOrDefault(y => y.SAPProductCode == x.ProductCode).Id : 0);
                        pendingQuantity.ForEach(x => x.ProductName = _ProductMaster.FirstOrDefault(y => y.SAPProductCode == x.ProductCode) != null ? _ProductMaster.FirstOrDefault(y => y.SAPProductCode == x.ProductCode).ProductDescription : null);
                        SessionHelper.DistributorPendingQuantity = pendingQuantity;
                    }
                    distributorProduct.ForEach(x => x.PendingQuantity = pendingQuantity.FirstOrDefault(e => e.ProductCode == x.SAPProductCode) == null ? 0 : pendingQuantity.FirstOrDefault(e => e.ProductCode == x.SAPProductCode).PendingQuantity);
                    distributorProduct.ForEach(x => x.SalesTax = SessionHelper.LoginUser.Distributor.IsSalesTaxApplicable ? x.ProductDetail.SalesTax : x.ProductDetail.SalesTax + x.ProductDetail.AdditionalSalesTax);
                    distributorProduct.ForEach(x => x.IncomeTax = SessionHelper.LoginUser.Distributor.IsIncomeTaxApplicable ? x.ProductDetail.IncomeTax : x.ProductDetail.IncomeTax * 2);
                    distributorProduct.ForEach(x => x.AdditionalSalesTax = x.ProductDetail.AdditionalSalesTax);
                    distributorProduct.ForEach(x => x.ViewSalesTax = x.ProductDetail.SalesTax);
                    SessionHelper.AddDistributorWiseProduct = model.ProductDetails = distributorProduct.ToList();
                    return View("AddOrder", model);
                }
            }
            catch (Exception ex)
            {
                new ErrorLogBLL(_unitOfWork).AddExceptionLog(ex);
                var distributorProduct = discountAndPricesBll.Where(e => e.DistributorId == SessionHelper.LoginUser.DistributorId && e.ProductDetailId != null && (e.ProductDetail.ProductVisibilityId == ProductVisibility.Visible || e.ProductDetail.ProductVisibilityId == ProductVisibility.OrderDispatch)).OrderBy(x => x.ProductDescription).ToList();
                if (SessionHelper.DistributorPendingQuantity != null && SessionHelper.DistributorPendingQuantity.Count() > 0)
                {
                    distributorProduct.ForEach(x => x.PendingQuantity = SessionHelper.DistributorPendingQuantity.FirstOrDefault(e => e.ProductCode == x.SAPProductCode) == null ? 0 : SessionHelper.DistributorPendingQuantity.First(y => y.ProductCode == x.SAPProductCode).PendingQuantity);
                }
                distributorProduct.ForEach(x => x.SalesTax = SessionHelper.LoginUser.Distributor.IsSalesTaxApplicable ? x.ProductDetail.SalesTax : x.ProductDetail.SalesTax + x.ProductDetail.AdditionalSalesTax);
                distributorProduct.ForEach(x => x.IncomeTax = SessionHelper.LoginUser.Distributor.IsIncomeTaxApplicable ? x.ProductDetail.IncomeTax : x.ProductDetail.IncomeTax * 2);
                distributorProduct.ForEach(x => x.AdditionalSalesTax =  x.ProductDetail.AdditionalSalesTax);
                distributorProduct.ForEach(x => x.ViewSalesTax =  x.ProductDetail.SalesTax);
                SessionHelper.AddDistributorWiseProduct = model.ProductDetails = distributorProduct.ToList();
                return View("AddOrder", model);
            }
        }
        public ActionResult ApprovedOrderValue(int Product, int Quantity)
        {
            var list = SessionHelper.AddDistributorWiseProduct;
            list.First(e => e.ProductDetail.ProductMasterId == Product).ProductDetail.ProductMaster.Quantity = Quantity;
            SessionHelper.AddDistributorWiseProduct = list;
            var OrderVal = _OrderBLL.GetOrderValueModel(SessionHelper.AddDistributorWiseProduct);
            return PartialView("OrderValue", OrderVal);
        }
        public JsonResult GetCurrentOrderValue()
        {
            if (SessionHelper.AddDistributorWiseProduct != null && SessionHelper.TotalOrderValue != null)
            {
                return Json(new { data = SessionHelper.TotalOrderValue });
            }
            else
            {
                return Json(new { data = "0" });
            }
        }
        [HttpPost]
        public JsonResult SaveEdit(OrderViewModel model, SubmitStatus btnSubmit)
        {
            JsonResponse jsonResponse = new JsonResponse();
            OrderMaster master = new OrderMaster();
            string FolderPath = _IConfiguration.GetSection("Settings").GetSection("FolderPath").Value;
            try
            {
                ModelState.Remove("Id");
                if (!ModelState.IsValid)
                {
                    jsonResponse.Status = false;
                    jsonResponse.Message = NotificationMessage.RequiredFieldsValidation;
                    return Json(new { data = jsonResponse });
                }
                else
                {
                    List<int> distributorLicenses = new List<int>();
                    distributorLicenses = _DistributorLicenseBLL.Where(e => e.IsActive && e.Status == LicenseStatus.Verified && e.Expiry > DateTime.Now && e.DistributorId == SessionHelper.LoginUser.DistributorId).Select(x => x.LicenseId).ToList();
                    if (distributorLicenses.Count() == 0)
                    {
                        jsonResponse.Status = false;
                        jsonResponse.Message = NotificationMessage.AddVerifiedLicense;
                        return Json(new { data = jsonResponse });
                    }
                    if (SessionHelper.AddDistributorWiseProduct != null && SessionHelper.AddDistributorWiseProduct.Count > 0)
                    {
                        if (SessionHelper.AddDistributorWiseProduct.Where(x => x.ProductDetail.ProductMaster.Quantity != 0).Count() == 0)
                        {
                            jsonResponse.Status = false;
                            jsonResponse.Message = "At least add one quantity in products.";
                            return Json(new { data = jsonResponse });
                        }
                        master.DistributorWiseProduct = model.ProductDetails.Where(e => e.ProductDetail.ProductMaster.Quantity != 0).ToList();
                        master.DistributorWiseProduct = master.DistributorWiseProduct.Where(x => distributorLicenses.Contains(Convert.ToInt32(x.ProductDetail.LicenseControlId)) || x.ProductDetail.LicenseControlId == null).ToList();
                        master.ReferenceNo = model.ReferenceNo;
                        master.Remarks = model.Remarks;
                        master.AttachmentFormFile = model.AttachmentFormFile;
                        master.Id = model.Id;

                        if (SubmitStatus.Draft == btnSubmit)
                        {
                            master.Status = OrderStatus.Draft;
                        }
                        else
                        {
                            master.Status = OrderStatus.PendingApproval;
                        }
                        if (model.AttachmentFormFile != null)
                        {
                            var ext = Path.GetExtension(model.AttachmentFormFile.FileName).ToLowerInvariant();
                            if (string.IsNullOrEmpty(ext) || !permittedExtensions.Contains(ext))
                            {
                                jsonResponse.Status = false;
                                jsonResponse.Message = NotificationMessage.FileTypeAllowed;
                                return Json(new { data = jsonResponse });
                            }
                            if (model.AttachmentFormFile.Length >= Convert.ToInt64(_Configuration.FileSize))
                            {
                                jsonResponse.Status = false;
                                jsonResponse.Message = NotificationMessage.FileSizeAllowed;
                                return Json(new { data = jsonResponse });
                            }

                            Tuple<bool, string> tuple = FileUtility.UploadFile(model.AttachmentFormFile, FolderName.Order, FolderPath);
                            if (tuple.Item1)
                            {
                                master.Attachment = tuple.Item2;
                            }
                        }
                        jsonResponse = _OrderBLL.Save(master, Url);
                    }
                    else
                    {
                        jsonResponse.Status = false;
                        jsonResponse.Message = OrderContant.OrderItem;
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
        public IActionResult ViewOrder(string DPID)
        {
            int id = 0;
            if (!string.IsNullOrEmpty(DPID))
            {
                int.TryParse(EncryptDecrypt.Decrypt(DPID), out id);
            }
            var orderDetail = _orderDetailBll.Where(e => e.OrderId == id).ToList();
            ViewBag.OrderValue = _OrderBLL.GetOrderValueModel(_orderValueBll.GetOrderValueByOrderId(id));
            return View(orderDetail);
        }
        public IActionResult Approve(string DPID)
        {
            int.TryParse(EncryptDecrypt.Decrypt(DPID), out int id);
            var orderDetail = _orderDetailBll.Where(e => e.OrderId == id).ToList();
            ViewBag.OrderValue = _OrderBLL.GetOrderValueModel(_orderValueBll.GetOrderValueByOrderId(id));
            return View("ApproveOrder", orderDetail);
        }
        public IActionResult ApproveOrder(string DPID)
        {
            int.TryParse(EncryptDecrypt.Decrypt(DPID), out int id);
            var orderDetail = _orderDetailBll.Where(e => e.OrderId == id && !e.IsProductSelected).ToList();
            List<ProductDetail> productDetails = _productDetailBLL.GetAllProductDetail();
            orderDetail.ForEach(x => x.ProductDetail = productDetails.First(y => y.ProductMasterId == x.ProductMaster.Id));
            SessionHelper.OrderDetail = orderDetail;
            ViewBag.OrderValue = _OrderBLL.GetOrderValueModel(_orderValueBll.GetOrderValueByOrderId(id));
            return View("PartiallyApproveOrder", orderDetail);
        }
        public IActionResult OnHold(string DPID, string Comments)
        {
            JsonResponse jsonResponse = new JsonResponse();
            Notification notification = new Notification();
            try
            {
                int.TryParse(EncryptDecrypt.Decrypt(DPID), out int id);
                var order = _OrderBLL.GetOrderMasterById(id);
                order.Status = OrderStatus.Onhold;
                order.OnHoldComment = Comments;
                order.OnHoldBy = SessionHelper.LoginUser.Id;
                order.OnHoldDate = DateTime.Now;
                bool result = _OrderBLL.Update(order);
                if (result)
                {
                    jsonResponse.Status = true;
                    jsonResponse.Message = "Order on hold";
                    jsonResponse.RedirectURL = Url.Action("Index", "Order");
                    jsonResponse.SignalRResponse = new SignalRResponse() { UserId = order.CreatedBy.ToString(), Number = "Order #: " + order.SNo, Message = "Order marked on hold by Admin", Status = Enum.GetName(typeof(OrderStatus), order.Status) };
                    notification.CompanyId = SessionHelper.LoginUser.CompanyId;
                    notification.DistributorId = order.DistributorId;
                    notification.RequestId = order.SNo;
                    notification.Status = order.Status.ToString();
                    notification.Message = jsonResponse.SignalRResponse.Message;
                    notification.URL = "/OrderForm/ViewOrder?DPID=" + EncryptDecrypt.Encrypt(id.ToString());
                    _NotificationBLL.Add(notification);
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
        public IActionResult Reject(string DPID, string Comments)
        {
            JsonResponse jsonResponse = new JsonResponse();
            Notification notification = new Notification();
            try
            {
                int.TryParse(EncryptDecrypt.Decrypt(DPID), out int id);
                var order = _OrderBLL.GetOrderMasterById(id);
                if (order != null && (order.Status == OrderStatus.Rejected || order.Status == OrderStatus.Approved))
                {
                    jsonResponse.Status = false;
                    jsonResponse.Message = "Order already " + order.Status;
                    jsonResponse.RedirectURL = Url.Action("Index", "OrderForm");
                    return Json(new { data = jsonResponse });
                }
                order.Status = OrderStatus.Rejected;
                order.RejectedComment = Comments;
                order.RejectedBy = SessionHelper.LoginUser.Id;
                order.RejectedDate = DateTime.Now;
                bool result = _OrderBLL.Update(order);
                if (result)
                {
                    jsonResponse.Status = true;
                    jsonResponse.Message = "Order has been rejected";
                    jsonResponse.RedirectURL = Url.Action("Index", "Order");
                    jsonResponse.SignalRResponse = new SignalRResponse() { UserId = order.CreatedBy.ToString(), Number = "Order #: " + order.SNo, Message = "Order has been rejected by Admin", Status = Enum.GetName(typeof(OrderStatus), order.Status) };
                    notification.CompanyId = SessionHelper.LoginUser.CompanyId;
                    notification.DistributorId = order.DistributorId;
                    notification.RequestId = order.SNo;
                    notification.Status = order.Status.ToString();
                    notification.Message = jsonResponse.SignalRResponse.Message;
                    notification.URL = "/OrderForm/ViewOrder?DPID=" + EncryptDecrypt.Encrypt(id.ToString());
                    _NotificationBLL.Add(notification);
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
        public IActionResult OnApprove(string DPID, int[] companyId)
        {
            JsonResponse jsonResponse = new JsonResponse();
            Notification notification = new Notification();
            try
            {
                bool SAPOrderStatus = false;
                bool POrderStatus = false;
                int.TryParse(EncryptDecrypt.Decrypt(DPID), out int id);
                var order = _OrderBLL.GetOrderMasterById(id);
                if (order != null && (order.Status == OrderStatus.Rejected || order.Status == OrderStatus.Approved))
                {
                    jsonResponse.Status = false;
                    jsonResponse.Message = "Order already " + order.Status;
                    jsonResponse.RedirectURL = Url.Action("Index", "OrderForm");
                    return Json(new { data = jsonResponse });
                }

                if (companyId.Count() > 0)
                {
                    SessionHelper.OrderDetail = SessionHelper.OrderDetail.Where(x => companyId.Contains(x.ProductDetail.CompanyId)).ToList();
                }
                var OrderDetails = _orderDetailBll.Where(e => e.OrderId == id).ToList();
                List<ProductDetail> productDetails = _productDetailBLL.GetAllProductDetail();
                OrderDetails.ForEach(x => x.ProductDetail = productDetails.First(y => y.ProductMasterId == x.ProductMaster.Id));
                var SAPOrderDetail = SessionHelper.OrderDetail.Where(e => e.OrderId == id && e.ProductDetail.IsPlaceOrderInSAP && e.IsProductSelected).ToList();
                var POrderDetail = SessionHelper.OrderDetail.Where(e => e.OrderId == id && !e.ProductDetail.IsPlaceOrderInSAP && e.IsProductSelected).ToList();

                if (SAPOrderDetail != null && SAPOrderDetail.Count() > 0)
                {
                    List<SAPOrderStatus> sapProduct = _OrderBLL.PostDistributorOrder(SAPOrderDetail, _Configuration);

                    if (sapProduct != null && sapProduct.Count() > 0)
                    {
                        foreach (var item in sapProduct)
                        {
                            var product = SAPOrderDetail.FirstOrDefault(e => e.ProductMaster.SAPProductCode == item.ProductCode);
                            if (product != null)
                            {
                                if (!string.IsNullOrEmpty(item.SAPOrderNo))
                                {
                                    product.IsProductSelected = true;
                                    product.SAPOrderNumber = item.SAPOrderNo;
                                    product.OrderProductStatus = OrderStatus.InProcess;
                                    _orderDetailBll.Update(product);
                                }
                            }
                        }
                        var SAPOrderDetailUpdate = OrderDetails.Where(e => e.SAPOrderNumber == null).ToList();
                        order.Status = SAPOrderDetailUpdate.Count > 0 ? OrderStatus.PartiallyApproved : OrderStatus.Approved;
                        order.ApprovedBy = SessionHelper.LoginUser.Id;
                        order.ApprovedDate = DateTime.Now;
                        SAPOrderStatus = _OrderBLL.Update(order);
                        TempData["DistributorId"] = EncryptDecrypt.Encrypt(order.DistributorId.ToString());
                    }
                    else
                    {
                        jsonResponse.Status = false;
                        jsonResponse.Message = "Unable to approve order";
                        return Json(new { data = jsonResponse });
                    }
                }
                if (POrderDetail != null && POrderDetail.Count() > 0)
                {
                    foreach (var item in POrderDetail)
                    {
                        item.IsProductSelected = true;
                        item.SAPOrderNumber = item.OrderMaster.SNo.ToString();
                        item.OrderProductStatus = OrderStatus.CompletelyProcessed;
                        _orderDetailBll.Update(item);
                    }
                    POrderDetail = OrderDetails.Where(e => e.SAPOrderNumber == null).ToList();
                    order.Status = POrderDetail.Count > 0 ? OrderStatus.PartiallyApproved : OrderStatus.Approved;
                    order.ApprovedBy = SessionHelper.LoginUser.Id;
                    order.ApprovedDate = DateTime.Now;
                    POrderStatus = _OrderBLL.Update(order);
                    TempData["DistributorId"] = EncryptDecrypt.Encrypt(order.DistributorId.ToString());
                }
                //Sending Email
                if (SAPOrderStatus || POrderStatus)
                {
                    foreach (var item in OrderDetails.Select(x => x.ProductDetail.PlantLocationId).Distinct().ToArray())
                    {
                        string SAPOrder = string.Join("</br>" + Environment.NewLine, OrderDetails.Where(x => x.ProductDetail.PlantLocationId == item && x.ProductDetail.IsPlaceOrderInSAP).Select(x => x.SAPOrderNumber).Distinct().ToArray());
                        string DPOrder = string.Join("</br>" + Environment.NewLine, OrderDetails.Where(x => x.ProductDetail.PlantLocationId == item && !x.ProductDetail.IsPlaceOrderInSAP).Select(x => x.SAPOrderNumber).Distinct().ToArray());
                        string EmailTemplate = _env.WebRootPath + "\\Attachments\\EmailTemplates\\ApprovalOfSaleOrder.html";
                        ApprovedOrderEmailUserModel EmailUserModel = new ApprovedOrderEmailUserModel()
                        {
                            ToAcceptTemplate = System.IO.File.ReadAllText(EmailTemplate),
                            Date = Convert.ToDateTime(order.ApprovedDate).ToString("dd/MMM/yyyy"),
                            City = order.Distributor.City,
                            ShipToPartyName = order.Distributor.DistributorName,
                            SAPOrder = string.IsNullOrEmpty(SAPOrder) ? "" : "SAP Order Number",
                            SAPOrderNumber = string.IsNullOrEmpty(SAPOrder) ? "" : SAPOrder,
                            DPOrder = string.IsNullOrEmpty(DPOrder) ? "" : "Distrbutor Portal Order Number",
                            DPOrderNumber = string.IsNullOrEmpty(DPOrder) ? "" : OrderDetails.First().OrderMaster.SNo.ToString(),
                            Subject = "Order Delivery",
                            CreatedBy = SessionHelper.LoginUser.Id,
                            CCEmail = OrderDetails.First(x => x.ProductDetail.PlantLocationId == item).ProductDetail.PlantLocation.CCEmail,
                        };
                        List<User> UserList = _UserBLL.GetAllActiveUser().Where(x => x.IsStoreKeeper && x.PlantLocationId != null && x.PlantLocationId == item
                        && Enum.GetValues(typeof(EmailIntimation)).Cast<int>().ToArray().Where(x => x.Equals(1) || x.Equals(3)).Contains(Convert.ToInt32(x.EmailIntimationId))).ToList();

                        if (UserList != null && UserList.Count() > 0 && (!string.IsNullOrEmpty(SAPOrder) || !string.IsNullOrEmpty(DPOrder)))
                        {
                            _EmailLogBLL.OrderEmail(UserList, EmailUserModel);
                        }
                    }
                    jsonResponse.Status = true;
                    jsonResponse.Message = "Order has been approved successfully";
                    jsonResponse.SignalRResponse = new SignalRResponse() { UserId = order.CreatedBy.ToString(), Number = "Order #: " + order.SNo, Message = "Order has been approved", Status = Enum.GetName(typeof(OrderStatus), order.Status) };
                    notification.CompanyId = SessionHelper.LoginUser.CompanyId;
                    notification.DistributorId = order.DistributorId;
                    notification.RequestId = order.SNo;
                    notification.Status = order.Status.ToString();
                    notification.Message = jsonResponse.SignalRResponse.Message;
                    notification.URL = "/OrderForm/ViewOrder?DPID=" + EncryptDecrypt.Encrypt(id.ToString());
                    _NotificationBLL.Add(notification);
                    jsonResponse.RedirectURL = Url.Action("Index", "Order");
                    return Json(new { data = jsonResponse });
                }
                else
                {
                    jsonResponse.Status = false;
                    jsonResponse.Message = "Unable to approve order";
                    return Json(new { data = jsonResponse });
                }
            }
            catch (Exception ex)
            {
                new ErrorLogBLL(_unitOfWork).AddExceptionLog(ex);
                jsonResponse.Status = false;
                jsonResponse.Message = NotificationMessage.ErrorOccurred;
                return Json(new { data = jsonResponse });
            }
        }
        //[HttpPost]
        //public IActionResult SyncPartiallyApproved(string DPID)
        //{
        //    JsonResponse jsonResponse = new JsonResponse();
        //    Notification notification = new Notification();
        //    try
        //    {
        //        bool SAPOrderStatus = false;
        //        bool POrderStatus = false;
        //        int.TryParse(EncryptDecrypt.Decrypt(DPID), out int id);
        //        var order = _OrderBLL.GetOrderMasterById(id);
        //        var OrderDetail = _orderDetailBll.Where(e => e.OrderId == id).ToList();
        //        //var client = new RestClient(_Configuration.PostOrder);
        //        //var request = new RestRequest(Method.POST).AddJsonBody(_OrderBLL.PlaceOrderPartiallyApprovedToSAP(id), "json");
        //        //IRestResponse response = client.Execute(request);
        //        //var sapProduct = JsonConvert.DeserializeObject<List<SAPOrderStatus>>(response.Content);
        //        var OrderDetails = _orderDetailBll.Where(e => e.OrderId == id && e.ProductDetail.IsPlaceOrderInSAP).ToList();
        //        List<SAPOrderStatus> sapProduct = _OrderBLL.PostDistributorOrder(OrderDetails, _Configuration);
        //        var updatedOrderDetail = _orderDetailBll.Where(e => e.OrderId == id && e.SAPOrderNumber == null).ToList();
        //        if (sapProduct != null && sapProduct.Count() > 0)
        //        {
        //            foreach (var item in sapProduct)
        //            {
        //                var product = updatedOrderDetail.FirstOrDefault(e => e.ProductMaster.SAPProductCode == item.ProductCode);
        //                if (product != null)
        //                {
        //                    if (!string.IsNullOrEmpty(item.SAPOrderNo))
        //                    {
        //                        product.SAPOrderNumber = item.SAPOrderNo;
        //                        product.OrderProductStatus = OrderStatus.InProcess;
        //                        _orderDetailBll.Update(product);
        //                    }
        //                }
        //            }
        //            updatedOrderDetail = _orderDetailBll.Where(e => e.OrderId == id && e.SAPOrderNumber == null).ToList();
        //            order.Status = updatedOrderDetail.Count > 0 ? OrderStatus.PartiallyApproved : OrderStatus.InProcess;
        //            order.ApprovedBy = SessionHelper.LoginUser.Id;
        //            order.ApprovedDate = DateTime.Now;
        //            var result = _OrderBLL.Update(order);
        //            jsonResponse.Status = result;
        //            jsonResponse.Message = result ? "Order has been approved successfully" : NotificationMessage.ErrorOccurred;
        //            TempData["DistributorId"] = EncryptDecrypt.Encrypt(order.DistributorId.ToString());
        //            //Sending Email
        //            //Sending Email
        //            if (SAPOrderStatus || POrderStatus)
        //            {
        //                var OrderDetails = _orderDetailBll.Where(e => e.OrderId == id).ToList();
        //                foreach (var item in OrderDetails.Select(x => x.ProductDetail.PlantLocationId).Distinct().ToArray())
        //                {
        //                    string SAPOrder = string.Join("</br>" + Environment.NewLine, OrderDetails.Where(x => x.ProductDetail.PlantLocationId == item && x.ProductDetail.IsPlaceOrderInSAP).Select(x => x.SAPOrderNumber).Distinct().ToArray());
        //                    string DPOrder = string.Join("</br>" + Environment.NewLine, OrderDetails.Where(x => x.ProductDetail.PlantLocationId == item && !x.ProductDetail.IsPlaceOrderInSAP).Select(x => x.SAPOrderNumber).Distinct().ToArray());
        //                    string EmailTemplate = _env.WebRootPath + "\\Attachments\\EmailTemplates\\ApprovalOfSaleOrder.html";
        //                    ApprovedOrderEmailUserModel EmailUserModel = new ApprovedOrderEmailUserModel()
        //                    {
        //                        ToAcceptTemplate = System.IO.File.ReadAllText(EmailTemplate),
        //                        Date = Convert.ToDateTime(order.ApprovedDate).ToString("dd/MMM/yyyy"),
        //                        City = order.Distributor.City,
        //                        ShipToPartyName = order.Distributor.DistributorName,
        //                        SAPOrder = string.IsNullOrEmpty(SAPOrder) ? "" : "SAP Order Number",
        //                        SAPOrderNumber = SAPOrder,
        //                        DPOrder = string.IsNullOrEmpty(DPOrder) ? "" : "Distrbutor Portal Order Number",
        //                        DPOrderNumber = OrderDetails.First().OrderMaster.SNo.ToString(),
        //                        Subject = "Order Delivery",
        //                        CreatedBy = SessionHelper.LoginUser.Id,
        //                        CCEmail = OrderDetails.First(x => x.ProductDetail.PlantLocationId == item).ProductDetail.PlantLocation.CCEmail,
        //                    };
        //                    List<User> UserList = _UserBLL.GetAllActiveUser().Where(x => x.IsStoreKeeper && x.PlantLocationId != null && x.PlantLocationId == item
        //                    && Enum.GetValues(typeof(EmailIntimation)).Cast<int>().ToArray().Where(x => x.Equals(1) || x.Equals(3)).Contains(Convert.ToInt32(x.EmailIntimationId))).ToList();

        //                    if (UserList != null && UserList.Count() > 0)
        //                    {
        //                        _EmailLogBLL.OrderEmail(UserList, EmailUserModel);
        //                    }
        //                }
        //                jsonResponse.Status = true;
        //                jsonResponse.Message = "Order has been approved successfully";
        //                jsonResponse.SignalRResponse = new SignalRResponse() { UserId = order.CreatedBy.ToString(), Number = "Order #: " + order.SNo, Message = "Order has been approved", Status = Enum.GetName(typeof(OrderStatus), order.Status) };
        //                notification.CompanyId = SessionHelper.LoginUser.CompanyId;
        //                notification.DistributorId = order.DistributorId;
        //                notification.RequestId = order.SNo;
        //                notification.Status = order.Status.ToString();
        //                notification.Message = jsonResponse.SignalRResponse.Message;
        //                notification.URL = "/OrderForm/ViewOrder?DPID=" + EncryptDecrypt.Encrypt(id.ToString());
        //                _NotificationBLL.Add(notification);
        //                jsonResponse.RedirectURL = Url.Action("Index", "Order");
        //                return Json(new { data = jsonResponse });
        //            }
        //            jsonResponse.SignalRResponse = new SignalRResponse() { UserId = order.CreatedBy.ToString(), Number = "Order #: " + order.SNo, Message = "Order has been approved", Status = Enum.GetName(typeof(OrderStatus), order.Status) };
        //            notification.CompanyId = SessionHelper.LoginUser.CompanyId;
        //            notification.DistributorId = order.DistributorId;
        //            notification.RequestId = order.SNo;
        //            notification.Status = order.Status.ToString();
        //            notification.Message = jsonResponse.SignalRResponse.Message;
        //            notification.URL = "/OrderForm/ViewOrder?DPID=" + EncryptDecrypt.Encrypt(id.ToString());
        //            _NotificationBLL.Add(notification);
        //        }
        //        else
        //        {
        //            jsonResponse.Status = false;
        //            jsonResponse.Message = "Unable to approve order";
        //        }
        //        jsonResponse.RedirectURL = Url.Action("Index", "Order");
        //        return Json(new { data = jsonResponse });
        //    }
        //    catch (Exception ex)
        //    {
        //        new ErrorLogBLL(_unitOfWork).AddExceptionLog(ex);
        //        jsonResponse.Status = false;
        //        jsonResponse.Message = NotificationMessage.ErrorOccurred;
        //        return Json(new { data = jsonResponse });
        //    }
        //}
        public IActionResult UpdatePQ(string DPID)
        {
            JsonResponse jsonResponse = new JsonResponse();
            try
            {
                int.TryParse(EncryptDecrypt.Decrypt(DPID), out int DistributorId);
                var distributor = _distributorBll.FirstOrDefault(x => x.IsActive && !x.IsDeleted && x.Id == DistributorId);
                List<DistributorPendingQuantity> DistributorPendingQuantityList = _OrderBLL.GetDistributorOrderPendingQuantitys(distributor.DistributorSAPCode, _Configuration);

                if (DistributorPendingQuantityList != null)
                {
                    List<ProductDetail> productDetails = _productDetailBLL.GetAllProductDetail();
                    List<DistributorWiseProductDiscountAndPrices> DistributorWiseProductDiscountAndPrices = discountAndPricesBll.Where(e => e.DistributorId == DistributorId).ToList();
                    DistributorPendingQuantityList.ForEach(x => x.Rate = DistributorWiseProductDiscountAndPrices.FirstOrDefault(y => y.ProductDetail == null ? true : y.ProductDetail.ProductMaster.SAPProductCode == x.ProductCode) != null
                    ? DistributorWiseProductDiscountAndPrices.FirstOrDefault(y => y.ProductDetail.ProductMaster.SAPProductCode == x.ProductCode).Rate : 0);
                    DistributorPendingQuantityList.ForEach(x => x.Discount = DistributorWiseProductDiscountAndPrices.FirstOrDefault(y => y.ProductDetail.ProductMaster.SAPProductCode == x.ProductCode) != null
                    ? DistributorWiseProductDiscountAndPrices.FirstOrDefault(y => y.ProductDetail.ProductMaster.SAPProductCode == x.ProductCode).Discount : 0);
                    DistributorPendingQuantityList.ForEach(x => x.ProductDetail = productDetails.FirstOrDefault(y => y.ProductMaster.SAPProductCode == x.ProductCode) != null ? productDetails.FirstOrDefault(y => y.ProductMaster.SAPProductCode == x.ProductCode) : null);
                    DistributorPendingQuantityList.ForEach(x => x.PendingValue = (x.PendingQuantity * x.Rate) - (x.PendingQuantity * x.Rate / 100 * (-1 * x.Discount))
                    + (x.PendingQuantity * x.Rate / 100 * x.ProductDetail.SalesTax)
                    + (x.PendingQuantity * x.Rate / 100 * x.ProductDetail.IncomeTax)
                    + (x.PendingQuantity * x.Rate / 100 * x.ProductDetail.AdditionalSalesTax));
                    DistributorPendingQuantityList.ForEach(e => e.DistributorId = DistributorId);
                    DistributorPendingQuantityList.ForEach(e => e.CreatedBy = SessionHelper.LoginUser.Id);
                    DistributorPendingQuantityList.ForEach(e => e.CreatedDate = DateTime.Now);
                    _unitOfWork.Begin();
                    _DistributorPendingQuanityBLL.AddRange(DistributorPendingQuantityList.Where(x => !string.IsNullOrEmpty(x.ProductCode)).ToList(), DistributorId);
                    _unitOfWork.Commit();
                }
                return Json(new { data = jsonResponse });
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                new ErrorLogBLL(_unitOfWork).AddExceptionLog(ex);
                jsonResponse.Status = false;
                jsonResponse.Message = NotificationMessage.ErrorOccurred;
                return Json(new { data = jsonResponse });
            }
        }
        public void SelectProduct(string DPID)
        {
            var list = SessionHelper.OrderDetail;
            int.TryParse(EncryptDecrypt.Decrypt(DPID), out int id);
            var product = list.FirstOrDefault(e => e.ProductId == id);

            if (product != null)
            {
                if (product.IsProductSelected)
                {
                    list.FirstOrDefault(e => e.ProductId == id).IsProductSelected = false;
                }
                else
                {
                    list.FirstOrDefault(e => e.ProductId == id).IsProductSelected = true;
                }
            }

            SessionHelper.OrderDetail = list;
        }
        public void SelectAllProduct(bool IsSelected)
        {
            var list = SessionHelper.OrderDetail;
            if (IsSelected)
            {
                list.ForEach(x => x.IsProductSelected = true);
            }
            else
            {
                list.ForEach(x => x.IsProductSelected = false);
            }
            SessionHelper.OrderDetail = list;
        }
        public IActionResult GetCompanyWiseProduct(int[] companyId)
        {
            JsonResponse jsonResponse = new JsonResponse();
            List<OrderDetail> list = new List<OrderDetail>();

            if (companyId.Count() > 0)
            {
                list = SessionHelper.OrderDetail.Where(x => companyId.Contains(x.ProductDetail.CompanyId)).ToList();
            }
            else
            {
                list = SessionHelper.OrderDetail.ToList();
            }
            jsonResponse.HtmlString = RenderRazorViewToString("PartiallyApproveOrderGrid", list);
            return Json(new { data = jsonResponse, companyId });
        }
        private async Task<string> RenderRazorViewToString(string viewName, object model)
        {
            ViewData.Model = model;
            using var sw = new StringWriter();
            var viewResult = _viewEngine.FindView(ControllerContext, viewName, false);
            var viewContext = new ViewContext(ControllerContext, viewResult.View, ViewData, TempData, sw, new HtmlHelperOptions());
            await viewResult.View.RenderAsync(viewContext);
            return sw.GetStringBuilder().ToString();
        }
    }
}
