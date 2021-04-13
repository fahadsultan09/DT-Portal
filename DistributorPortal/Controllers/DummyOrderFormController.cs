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

namespace DistributorPortal.Controllers
{
    public class DummyOrderFormController : Controller
    {
        private ProductDetailBLL _productDetailBLL;
        private IUnitOfWork _unitOfWork;
        private OrderBLL _OrderBLL;
        private DistributorWiseProductDiscountAndPricesBLL discountAndPricesBll;
        private IConfiguration _IConfiguration;
        private Configuration _Configuration;
        private OrderDetailBLL _orderDetailBll;
        private OrderValueBLL _orderValueBll;
        public DummyOrderFormController(IUnitOfWork unitOfWork, Configuration _configuration, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _OrderBLL = new OrderBLL(_unitOfWork);
            _productDetailBLL = new ProductDetailBLL(unitOfWork);
            _IConfiguration = configuration;
            _Configuration = _configuration;
            _orderDetailBll = new OrderDetailBLL(_unitOfWork);
            discountAndPricesBll = new DistributorWiseProductDiscountAndPricesBLL(_unitOfWork);
            _orderValueBll = new OrderValueBLL(_unitOfWork);
        }
        public IActionResult Index()
        {
            var model = _productDetailBLL.GetAllProductDetail();
            model.ForEach(x => x.PendingQuantity = SessionHelper.SAPOrderPendingQuantity.FirstOrDefault(y => y.ProductCode == x.ProductMaster.SAPProductCode) != null ? Math.Floor(Convert.ToDouble(SessionHelper.SAPOrderPendingQuantity.FirstOrDefault(z => z.ProductCode == x.ProductMaster.SAPProductCode).PendingQuantity)).ToString() : "0");

            return View(model);
        }

        public IActionResult Dummy(string DPID)
        {
            int id = 0;
            OrderViewModel model = new OrderViewModel();
            if (!string.IsNullOrEmpty(DPID))
            {
                int.TryParse(EncryptDecrypt.Decrypt(DPID), out id);
                var distributorProduct = discountAndPricesBll.Where(e => e.DistributorId == SessionHelper.LoginUser.DistributorId && e.ProductDetailId != null);
                if (SessionHelper.SAPOrderPendingQuantity != null)
                {
                    distributorProduct.ForEach(x => x.ProductDetail.PendingQuantity = SessionHelper.SAPOrderPendingQuantity.FirstOrDefault(y => y.ProductCode == x.ProductDetail.ProductMaster.SAPProductCode) != null ? Math.Floor(Convert.ToDouble(SessionHelper.SAPOrderPendingQuantity.FirstOrDefault(z => z.ProductCode == x.ProductDetail.ProductMaster.SAPProductCode)?.PendingQuantity)).ToString() : "0");
                }
                SessionHelper.AddDistributorWiseProduct = distributorProduct;
                model.ProductDetails = distributorProduct;
                return View("AddOrder", model);
            }
            else
            {
                
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
        public JsonResult SaveEdit(OrderViewModel model)
        {
            JsonResponse jsonResponse = new JsonResponse();
            OrderMaster master = new OrderMaster();
            master.DistributorWiseProduct = model.ProductDetails.Where(e => e.ProductDetail.ProductMaster.Quantity != 0).ToList();
            master.ReferenceNo = model.ReferenceNo;
            master.Remarks = model.Remarks;
            master.AttachmentFormFile = model.AttachmentFormFile;
            try
            {
                ModelState.Remove("Id");
                if (!ModelState.IsValid)
                {
                    jsonResponse.Status = false;
                    jsonResponse.Message = NotificationMessage.RequiredFieldsValidation;
                }
                else
                {
                    if (SessionHelper.AddDistributorWiseProduct.Count > 0)
                    {
                        if (model.SubmitStatus == SubmitStatus.Draft)
                        {
                            master.Status = OrderStatus.Draft;
                        }
                        else
                        {
                            master.Status = OrderStatus.PendingApproval;
                        }
                        jsonResponse = _OrderBLL.Save(master, _IConfiguration, Url);
                    }
                    else
                    {
                        jsonResponse.Status = false;
                        jsonResponse.Message = Common.OrderContant.OrderItem;
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
            int id = 0;
            if (!string.IsNullOrEmpty(DPID))
            {
                int.TryParse(EncryptDecrypt.Decrypt(DPID), out id);
            }
            var orderDetail = _orderDetailBll.Where(e => e.OrderId == id).ToList();
            ViewBag.OrderValue = _OrderBLL.GetOrderValueModel(_orderValueBll.GetOrderValueByOrderId(id));
            return View("ApproveOrder", orderDetail);
        }
        public IActionResult OnHold(string DPID, string Comments)
        {
            try
            {
                int id = 0;
                int.TryParse(EncryptDecrypt.Decrypt(DPID), out id);
                JsonResponse jsonResponse = new JsonResponse();
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
                    jsonResponse.SignalRResponse = new SignalRResponse() { UserId = order.CreatedBy.ToString(), Number = "Order #: " + string.Format("{0:1000000000}", order.Id), Message = "Order marked on hold by Admin", Status = Enum.GetName(typeof(OrderStatus), order.Status) };

                }
                return Json(jsonResponse);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public IActionResult Reject(string DPID, string Comments)
        {
            try
            {
                int id = 0;
                int.TryParse(EncryptDecrypt.Decrypt(DPID), out id);
                JsonResponse jsonResponse = new JsonResponse();
                var order = _OrderBLL.GetOrderMasterById(id);
                order.Status = OrderStatus.Reject;
                order.RejectedComment = Comments;
                order.RejectedBy = SessionHelper.LoginUser.Id;
                order.RejectedDate = DateTime.Now;
                var result = _OrderBLL.Update(order);
                if (result > 0)
                {
                    jsonResponse.Status = true;
                    jsonResponse.Message = "Order Rejected";
                    jsonResponse.RedirectURL = Url.Action("Index", "Order");
                    jsonResponse.SignalRResponse = new SignalRResponse() { UserId = order.CreatedBy.ToString(), Number = "Order #: " + string.Format("{0:1000000000}", order.Id), Message = "Order has been rejected by Admin", Status = Enum.GetName(typeof(OrderStatus), order.Status) };
                }
                return Json(jsonResponse);
            }
            catch (Exception ex)
            {
                new ErrorLogBLL(_unitOfWork).AddExceptionLog(ex);
                throw;
            }
        }

        public IActionResult OnApprove(string DPID)
        {
            try
            {
                int id = 0;
                int.TryParse(EncryptDecrypt.Decrypt(DPID), out id);
                JsonResponse jsonResponse = new JsonResponse();
                var order = _OrderBLL.GetOrderMasterById(id);
                order.Status = OrderStatus.Approved;
                order.ApprovedBy = SessionHelper.LoginUser.Id;
                order.ApprovedDate = DateTime.Now;

                var client = new RestClient(_Configuration.PostOrder);
                var request = new RestRequest(Method.POST).AddJsonBody(_OrderBLL.PlaceOrderToSAP(id), "json");
                IRestResponse response = client.Execute(request);
                var sapProduct = JsonConvert.DeserializeObject<List<SAPOrderStatus>>(response.Content);
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
                                product.OrderProductStatus = OrderStatus.NotYetProcess;
                                _orderDetailBll.Update(product);
                            }
                        }
                    }

                    var updatedOrderDetail =
                        _orderDetailBll.Where(e => e.OrderId == id && e.SAPOrderNumber == null).ToList();
                    order.Status = updatedOrderDetail.Count > 0 ? OrderStatus.PartiallyApproved : OrderStatus.InProcess;
                    order.ApprovedBy = SessionHelper.LoginUser.Id;
                    order.ApprovedDate = DateTime.Now;
                    var result = _OrderBLL.Update(order);
                    jsonResponse.Status = result > 0;
                    jsonResponse.Message = result > 0 ? "Order has been approved" : "Unable to approve order";
                }
                else
                {
                    jsonResponse.Status = false;
                    jsonResponse.Message = "Unable to approve order";
                }
                jsonResponse.RedirectURL = Url.Action("Index", "DummyOrderForm");
                jsonResponse.SignalRResponse = new SignalRResponse() { UserId = order.CreatedBy.ToString(), Number = "Order #: " + string.Format("{0:1000000000}", order.Id), Message = "Order Has been Approved", Status = Enum.GetName(typeof(OrderStatus), order.Status) };
                return Json(jsonResponse);
            }
            catch (Exception ex)
            {
                new ErrorLogBLL(_unitOfWork).AddExceptionLog(ex);
                throw;
            }
        }
    }
}
