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
using Newtonsoft.Json;
using RestSharp;
using Utility;
using Utility.Constant;
using Utility.HelperClasses;
using static Utility.Constant.Common;
using System.IO;
using BusinessLogicLayer.GeneralSetup;
using Microsoft.AspNetCore.Hosting;

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
        public OrderFormController(IUnitOfWork unitOfWork, Configuration configuration, IConfiguration iConfiguration, IWebHostEnvironment env)
        {
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
        }
        public IActionResult Index()
        {
            var model = _productDetailBLL.GetAllProductDetail();
            if (SessionHelper.SAPOrderPendingQuantity != null)
            {
                model.ForEach(x => x.PendingQuantity = SessionHelper.SAPOrderPendingQuantity.FirstOrDefault(y => y.ProductCode == x.ProductMaster.SAPProductCode) != null ? Math.Floor(Convert.ToDouble(SessionHelper.SAPOrderPendingQuantity.FirstOrDefault(z => z.ProductCode == x.ProductMaster.SAPProductCode).PendingQuantity)).ToString() : "0");
            }
            return View(model);
        }
        public IActionResult Create(string DPID)
        {
            JsonResponse jsonResponse = new JsonResponse();
            OrderViewModel model = new OrderViewModel();
            try
            {
                List<int> LicenseId = new List<int>();
                int? drug = _DistributorLicenseBLL.Where(e => e.Status == LicenseStatus.Verified && e.LicenseId == 1 && e.Expiry > DateTime.Now && e.DistributorId == SessionHelper.LoginUser.DistributorId).Select(x => x.LicenseId).FirstOrDefault();
                int? narcotics = _DistributorLicenseBLL.Where(e => e.Status == LicenseStatus.Verified && e.LicenseId == 2 && e.Expiry > DateTime.Now && e.DistributorId == SessionHelper.LoginUser.DistributorId).Select(x => x.LicenseId).FirstOrDefault();

                if (drug != null)
                    LicenseId.Add((int)drug);

                if (narcotics != null)
                    LicenseId.Add((int)narcotics);

                ViewBag.LicenseId = LicenseId;
                if (!string.IsNullOrEmpty(DPID))
                {
                    int.TryParse(EncryptDecrypt.Decrypt(DPID), out int id);
                    var distributorProduct = discountAndPricesBll.Where(e => e.DistributorId == SessionHelper.LoginUser.DistributorId && e.ProductDetailId != null);
                    var orderDetails = _orderDetailBll.Where(e => e.OrderId == id);
                    if (SessionHelper.SAPOrderPendingQuantity != null)
                    {
                        distributorProduct.ForEach(x => x.ProductDetail.PendingQuantity = SessionHelper.SAPOrderPendingQuantity.FirstOrDefault(y => y.ProductCode == x.ProductDetail.ProductMaster.SAPProductCode) != null ? Math.Floor(Convert.ToDouble(SessionHelper.SAPOrderPendingQuantity.FirstOrDefault(z => z.ProductCode == x.ProductDetail.ProductMaster.SAPProductCode)?.PendingQuantity)).ToString() : "0");
                    }
                    distributorProduct.ForEach(x => x.ProductDetail.ProductMaster.Quantity = orderDetails.FirstOrDefault(e => e.ProductId == x.ProductDetail.ProductMaster.Id)?.Quantity ?? 0);
                    foreach (var item in orderDetails)
                    {
                        distributorProduct.First(e => e.ProductDetail.ProductMasterId == item.ProductId).ProductDetail.ProductMaster.Quantity = item.Quantity;
                    }

                    model.OrderValues = _OrderBLL.GetOrderValueModel(_orderValueBll.GetOrderValueByOrderId(id));
                    model.Id = id;
                    SessionHelper.AddDistributorWiseProduct = distributorProduct;
                    model.ProductDetails = distributorProduct;
                    return View("AddOrder", model);
                }
                else
                {
                    if (SessionHelper.LoginUser.Distributor != null && (SessionHelper.SAPOrderPendingQuantity == null || SessionHelper.SAPOrderPendingQuantity.Count() == 0))
                    {
                        List<ProductMaster> _ProductMaster = _ProductMasterBLL.GetAllProductMaster().ToList();
                        SessionHelper.SAPOrderPendingQuantity = _OrderBLL.GetDistributorOrderPendingQuantity(SessionHelper.LoginUser.Distributor.DistributorSAPCode, _Configuration);
                        var pendingQuantity = SessionHelper.SAPOrderPendingQuantity;
                        pendingQuantity.ForEach(x => x.Id = _ProductMaster.FirstOrDefault(y => y.SAPProductCode == x.ProductCode) != null ? _ProductMaster.FirstOrDefault(y => y.SAPProductCode == x.ProductCode).Id : 0);
                        pendingQuantity.ForEach(x => x.ProductName = _ProductMaster.FirstOrDefault(y => y.SAPProductCode == x.ProductCode) != null ? _ProductMaster.FirstOrDefault(y => y.SAPProductCode == x.ProductCode).ProductName + " " + _ProductMaster.FirstOrDefault(y => y.SAPProductCode == x.ProductCode).ProductDescription : null);
                        SessionHelper.SAPOrderPendingQuantity = pendingQuantity;
                        SessionHelper.SAPOrderPendingQuantity = SessionHelper.SAPOrderPendingQuantity.OrderByDescending(x => Convert.ToDouble(x.PendingQuantity)).ToList();
                    }
                    var distributorProduct = discountAndPricesBll.Where(e => e.DistributorId == SessionHelper.LoginUser.DistributorId && e.ProductDetailId != null);
                    if (SessionHelper.SAPOrderPendingQuantity != null)
                    {
                        distributorProduct.ForEach(x => x.ProductDetail.PendingQuantity = SessionHelper.SAPOrderPendingQuantity.FirstOrDefault(y => y.ProductCode == x.ProductDetail.ProductMaster.SAPProductCode) != null ? Math.Floor(Convert.ToDouble(SessionHelper.SAPOrderPendingQuantity.FirstOrDefault(z => z.ProductCode == x.ProductDetail.ProductMaster.SAPProductCode)?.PendingQuantity)).ToString() : "0");
                    }
                    SessionHelper.AddDistributorWiseProduct = distributorProduct;
                    model.ProductDetails = distributorProduct;
                    return View("AddOrder", model);
                }
            }
            catch (Exception ex)
            {
                new ErrorLogBLL(_unitOfWork).AddExceptionLog(ex);
                var distributorProduct = discountAndPricesBll.Where(e => e.DistributorId == SessionHelper.LoginUser.DistributorId && e.ProductDetailId != null);
                if (SessionHelper.SAPOrderPendingQuantity != null)
                {
                    distributorProduct.ForEach(x => x.ProductDetail.PendingQuantity = SessionHelper.SAPOrderPendingQuantity.FirstOrDefault(y => y.ProductCode == x.ProductDetail.ProductMaster.SAPProductCode) != null ? Math.Floor(Convert.ToDouble(SessionHelper.SAPOrderPendingQuantity.FirstOrDefault(z => z.ProductCode == x.ProductDetail.ProductMaster.SAPProductCode)?.PendingQuantity)).ToString() : "0");
                }
                SessionHelper.AddDistributorWiseProduct = distributorProduct;
                model.ProductDetails = distributorProduct;
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

                    List<int?> LicenseId = new List<int?>
                    {
                        _DistributorLicenseBLL.Where(e => e.Status == LicenseStatus.Verified && e.LicenseId == 1 && e.Expiry > DateTime.Now && e.DistributorId == SessionHelper.LoginUser.DistributorId).Select(x => x.LicenseId).FirstOrDefault(),
                        _DistributorLicenseBLL.Where(e => e.Status == LicenseStatus.Verified && e.LicenseId == 2 && e.Expiry > DateTime.Now && e.DistributorId == SessionHelper.LoginUser.DistributorId).Select(x => x.LicenseId).FirstOrDefault()
                    };
                    if (LicenseId[0] == null && LicenseId[1] == null)
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

                            Tuple<bool, string> tuple = FileUtility.UploadFile(model.AttachmentFormFile, FolderName.OrderReturn, FolderPath);
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
                var result = _OrderBLL.Update(order);
                if (result > 0)
                {
                    jsonResponse.Status = true;
                    jsonResponse.Message = "Order on hold";
                    jsonResponse.RedirectURL = Url.Action("Index", "Order");
                    jsonResponse.SignalRResponse = new SignalRResponse() { UserId = order.CreatedBy.ToString(), Number = "Order #: " + order.SNo, Message = "Order marked on hold by Admin", Status = Enum.GetName(typeof(OrderStatus), order.Status) };
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
                if (order != null && (order.Status == OrderStatus.Reject || order.Status == OrderStatus.Approved))
                {
                    jsonResponse.Status = false;
                    jsonResponse.Message = "Order already " + order.Status;
                    jsonResponse.RedirectURL = Url.Action("Index", "OrderForm");
                    return Json(new { data = jsonResponse });
                }
                order.Status = OrderStatus.Reject;
                order.RejectedComment = Comments;
                order.RejectedBy = SessionHelper.LoginUser.Id;
                order.RejectedDate = DateTime.Now;
                var result = _OrderBLL.Update(order);
                if (result > 0)
                {
                    jsonResponse.Status = true;
                    jsonResponse.Message = "Order Rejected successfully";
                    jsonResponse.RedirectURL = Url.Action("Index", "Order");
                    jsonResponse.SignalRResponse = new SignalRResponse() { UserId = order.CreatedBy.ToString(), Number = "Order #: " + order.SNo, Message = "Order has been rejected by Admin", Status = Enum.GetName(typeof(OrderStatus), order.Status) };
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
        public IActionResult OnApprove(string DPID)
        {
            JsonResponse jsonResponse = new JsonResponse();
            Notification notification = new Notification();
            try
            {
                int.TryParse(EncryptDecrypt.Decrypt(DPID), out int id);
                var order = _OrderBLL.GetOrderMasterById(id);
                if (order != null && (order.Status == OrderStatus.Reject || order.Status == OrderStatus.Approved))
                {
                    jsonResponse.Status = false;
                    jsonResponse.Message = "Order already " + order.Status;
                    jsonResponse.RedirectURL = Url.Action("Index", "OrderForm");
                    return Json(new { data = jsonResponse });
                }
                order.ApprovedBy = SessionHelper.LoginUser.Id;
                order.ApprovedDate = DateTime.Now;

                //var client = new RestClient(_Configuration.PostOrder);
                //var request = new RestRequest(Method.POST).AddJsonBody(_OrderBLL.PlaceOrderToSAP(id), "json");
                //IRestResponse response = client.Execute(request);
                //var sapProduct = JsonConvert.DeserializeObject<List<SAPOrderStatus>>(response.Content);
                List<SAPOrderStatus> sapProduct = _OrderBLL.PostDistributorOrder(id);
                var detail = _orderDetailBll.Where(e => e.OrderId == id).ToList();
                if (sapProduct != null)
                {
                    foreach (var item in sapProduct)
                    {
                        var product = detail.FirstOrDefault(e => e.ProductMaster.SAPProductCode == item.ProductCode);
                        if (product != null)
                        {
                            if (!string.IsNullOrEmpty(item.SAPOrderNo))
                            {
                                product.SAPOrderNumber = item.SAPOrderNo;
                                product.OrderProductStatus = OrderStatus.InProcess;
                                _orderDetailBll.Update(product);
                            }
                        }
                    }
                    var updatedOrderDetail = _orderDetailBll.Where(e => e.OrderId == id && e.SAPOrderNumber == null).ToList();
                    order.Status = updatedOrderDetail.Count > 0 ? OrderStatus.PartiallyApproved : OrderStatus.Approved;
                    order.ApprovedBy = SessionHelper.LoginUser.Id;
                    order.ApprovedDate = DateTime.Now;
                    var result = _OrderBLL.Update(order);
                    jsonResponse.Status = result > 0;
                    jsonResponse.Message = result > 0 ? "Order has been approved successfully" : NotificationMessage.ErrorOccurred;

                    //Sending Email
                    if (jsonResponse.Status)
                    {
                        string EmailTemplate = _env.WebRootPath + "\\Attachments\\EmailTemplates\\ApprovalOfSaleOrder.html";
                        ApprovedOrderEmailUserModel EmailUserModel = new ApprovedOrderEmailUserModel()
                        {
                            ToAcceptTemplate = System.IO.File.ReadAllText(EmailTemplate),
                            Date = Convert.ToDateTime(order.ApprovedDate).ToString("dd/MMM/yyyy"),
                            City = order.Distributor.City,
                            ShipToPartyName = order.Distributor.DistributorName,
                            OrderNumber = order.SNo.ToString(),
                            Subject = "Order Delivery",
                            CreatedBy = SessionHelper.LoginUser.Id,
                        };
                        int[] PlantLocationId = _productDetailBLL.Where(x => detail.Select(x => x.ProductMaster.Id).Contains(x.ProductMasterId)).Select(x => x.PlantLocationId).Distinct().ToArray();
                        if (PlantLocationId.Count() > 0)
                        {
                            List<User> UserList = _UserBLL.GetAllUser().Where(x => x.PlantLocationId != null && PlantLocationId.Contains((int)x.PlantLocationId)).ToList();
                            _EmailLogBLL.OrderEmail(UserList, EmailUserModel);
                        }
                    }
                }
                else
                {
                    jsonResponse.Status = false;
                    jsonResponse.Message = "Unable to approved order";
                }
                jsonResponse.RedirectURL = Url.Action("Index", "Order");
                jsonResponse.SignalRResponse = new SignalRResponse() { UserId = order.CreatedBy.ToString(), Number = "Order #: " + order.SNo, Message = "Order has been approved", Status = Enum.GetName(typeof(OrderStatus), order.Status) };
                notification.DistributorId = order.DistributorId;
                notification.RequestId = order.SNo;
                notification.Status = order.Status.ToString();
                notification.Message = jsonResponse.SignalRResponse.Message;
                notification.URL = "/OrderForm/ViewOrder?DPID=" + EncryptDecrypt.Encrypt(id.ToString());
                _NotificationBLL.Add(notification);
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
        [HttpPost]
        public IActionResult SyncPartiallyApproved(string DPID)
        {
            JsonResponse jsonResponse = new JsonResponse();
            Notification notification = new Notification();
            try
            {
                int.TryParse(EncryptDecrypt.Decrypt(DPID), out int id);
                var order = _OrderBLL.GetOrderMasterById(id);
                var client = new RestClient(_Configuration.PostOrder);
                var request = new RestRequest(Method.POST).AddJsonBody(_OrderBLL.PlaceOrderPartiallyApprovedToSAP(id), "json");
                IRestResponse response = client.Execute(request);
                var sapProduct = JsonConvert.DeserializeObject<List<SAPOrderStatus>>(response.Content);
                var updatedOrderDetail = _orderDetailBll.Where(e => e.OrderId == id && e.SAPOrderNumber == null).ToList();
                if (sapProduct != null)
                {
                    foreach (var item in sapProduct)
                    {
                        var product = updatedOrderDetail.FirstOrDefault(e => e.ProductMaster.SAPProductCode == item.ProductCode);
                        if (product != null)
                        {
                            if (!string.IsNullOrEmpty(item.SAPOrderNo))
                            {
                                product.SAPOrderNumber = item.SAPOrderNo;
                                product.OrderProductStatus = OrderStatus.InProcess;
                                _orderDetailBll.Update(product);
                            }
                        }
                    }
                    updatedOrderDetail = _orderDetailBll.Where(e => e.OrderId == id && e.SAPOrderNumber == null).ToList();
                    order.Status = updatedOrderDetail.Count > 0 ? OrderStatus.PartiallyApproved : OrderStatus.InProcess;
                    order.ApprovedBy = SessionHelper.LoginUser.Id;
                    order.ApprovedDate = DateTime.Now;
                    var result = _OrderBLL.Update(order);
                    jsonResponse.Status = result > 0;
                    jsonResponse.Message = result > 0 ? "Order has been approved successfully" : NotificationMessage.ErrorOccurred;
                    jsonResponse.RedirectURL = Url.Action("Index", "Order");
                    jsonResponse.SignalRResponse = new SignalRResponse() { UserId = order.CreatedBy.ToString(), Number = "Order #: " + order.SNo, Message = "Order has been approved", Status = Enum.GetName(typeof(OrderStatus), order.Status) };
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
    }
}
