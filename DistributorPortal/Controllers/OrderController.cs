using BusinessLogicLayer.Application;
using BusinessLogicLayer.ApplicationSetup;
using BusinessLogicLayer.ErrorLog;
using BusinessLogicLayer.HelperClasses;
using DataAccessLayer.WorkProcess;
using DistributorPortal.Resource;
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
using Utility.Constant;
using Utility.HelperClasses;
using static Utility.Constant.Common;

namespace DistributorPortal.Controllers
{
    public class OrderController : BaseController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly OrderBLL _OrderBLL;
        private readonly OrderDetailBLL _orderDetailBLL;
        private readonly OrderValueBLL _OrderValueBLL;
        private readonly ProductMasterBLL _ProductMasterBLL;
        private readonly ProductDetailBLL _productDetailBLL;
        private readonly DistributorLicenseBLL _DistributorLicenseBLL;
        private readonly LicenseControlBLL _licenseControlBLL;
        private readonly ICompositeViewEngine _viewEngine;
        private readonly Configuration _Configuration;
        private readonly IConfiguration _IConfiguration;
        public OrderController(IUnitOfWork unitOfWork, ICompositeViewEngine viewEngine, Configuration configuration, IConfiguration iConfiguration)
        {
            _unitOfWork = unitOfWork;
            _OrderBLL = new OrderBLL(_unitOfWork);
            _ProductMasterBLL = new ProductMasterBLL(_unitOfWork);
            _productDetailBLL = new ProductDetailBLL(_unitOfWork);
            _orderDetailBLL = new OrderDetailBLL(_unitOfWork);
            _OrderValueBLL = new OrderValueBLL(_unitOfWork);
            _DistributorLicenseBLL = new DistributorLicenseBLL(_unitOfWork);
            _licenseControlBLL = new LicenseControlBLL(_unitOfWork);
            _viewEngine = viewEngine;
            _Configuration = configuration;
            _IConfiguration = iConfiguration;
        }
        // GET: Order
        public ActionResult Index(OrderStatus? orderStatus)
        {
            List<OrderMaster> orderList = GetOrderList();
            if (orderStatus != null)
            {
                orderList = orderList.Where(x => x.Status == orderStatus).ToList();
            }
            if (SessionHelper.LoginUser.IsDistributor)
            {
                DistributorLicense DistributorLicense = _DistributorLicenseBLL.FirstOrDefault(x => x.DistributorId == SessionHelper.LoginUser.DistributorId && x.Type == LicenseType.License && x.Status == LicenseStatus.Verified);
                if (DistributorLicense != null)
                {
                    int Days = (DistributorLicense.Expiry.Date - DateTime.Now.Date).Days;
                    if (Days <= 0)
                    {
                        ViewBag.Expired = true;
                    }
                    var license = _licenseControlBLL.FirstOrDefault(x => x.IsActive == true && x.IsDeleted == false && x.IsMandatory);
                    if (license != null)
                    {
                        if (Days <= license.DaysIntimateBeforeExpiry)
                        {
                            ViewBag.Days = Days;
                        }
                        else
                        {
                            ViewBag.Days = string.Empty;
                        }
                    }
                }
            }
            return View(orderList);
        }
        public IActionResult List()
        {
            return PartialView("List", GetOrderList());
        }
        [HttpGet]
        public IActionResult Add(string DPID)
        {
            int id = 0;
            if (!string.IsNullOrEmpty(DPID))
            {
                int.TryParse(EncryptDecrypt.Decrypt(DPID), out id);
            }
            SessionHelper.AddProduct = new List<ProductDetail>();

            OrderMaster model = _OrderBLL.GetOrderMasterById(id);

            if (!SessionHelper.LoginUser.IsDistributor)
            {
                SessionHelper.SAPOrderPendingValue = _OrderBLL.GetPendingOrderValue(model.Distributor.DistributorSAPCode, _Configuration).ToList();
                SessionHelper.SAPOrderPendingQuantity = _OrderBLL.GetDistributorPendingQuantity(model.Distributor.DistributorSAPCode, _Configuration).ToList();
                SessionHelper.DistributorBalance = _OrderBLL.GetBalance(model.Distributor.DistributorSAPCode, _Configuration);
            }
            return View("AddDetail", BindOrderMaster(model, id));
        }
        [HttpGet]
        public IActionResult Approve(string DPID)
        {
            int id = 0;
            if (!string.IsNullOrEmpty(DPID))
            {
                int.TryParse(EncryptDecrypt.Decrypt(DPID), out id);
            }
            OrderMaster model = _OrderBLL.GetOrderMasterById(id);

            if (!SessionHelper.LoginUser.IsDistributor)
            {
                SessionHelper.SAPOrderPendingValue = _OrderBLL.GetPendingOrderValue(model.Distributor.DistributorSAPCode, _Configuration).ToList();
                SessionHelper.SAPOrderPendingQuantity = _OrderBLL.GetDistributorPendingQuantity(model.Distributor.DistributorSAPCode, _Configuration).ToList();
                SessionHelper.DistributorBalance = _OrderBLL.GetBalance(model.Distributor.DistributorSAPCode, _Configuration);
            }
            var order = BindOrderMaster(model, id, true);
            ViewBag.Status = order.Status;
            return View("OrderApprove", order);
        }
        public IActionResult OnHold(string DPID, string Comments)
        {
            try
            {
                int.TryParse(EncryptDecrypt.Decrypt(DPID), out int id);
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
                    jsonResponse.SignalRResponse = new SignalRResponse() { UserId = order.CreatedBy.ToString(), Number = "Order #: " + order.SNo, Message = "Order marked on hold by Admin", Status = Enum.GetName(typeof(OrderStatus), order.Status) };

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
                int.TryParse(EncryptDecrypt.Decrypt(DPID), out int id);
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
                    jsonResponse.Message = "Order rejected";
                    jsonResponse.RedirectURL = Url.Action("Index", "Order");
                    jsonResponse.SignalRResponse = new SignalRResponse() { UserId = order.CreatedBy.ToString(), Number = "Order #: " + order.SNo, Message = "Order has been rejected by Admin", Status = Enum.GetName(typeof(OrderStatus), order.Status) };
                }
                return Json(jsonResponse);
            }
            catch (Exception ex)
            {
                new ErrorLogBLL(_unitOfWork).AddExceptionLog(ex);
                throw;
            }
        }
        [HttpPost]
        public IActionResult Cancel(string DPID)
        {
            try
            {
                int.TryParse(EncryptDecrypt.Decrypt(DPID), out int id);
                JsonResponse jsonResponse = new JsonResponse();
                var order = _OrderBLL.GetOrderMasterById(id);
                order.Status = OrderStatus.Cancel;
                var result = _OrderBLL.Update(order);
                if (result > 0)
                {
                    jsonResponse.Status = true;
                    jsonResponse.Message = "Order has been cancelled";
                    jsonResponse.RedirectURL = Url.Action("Index", "Order");
                }
                return Json(jsonResponse);
            }
            catch (Exception ex)
            {
                new ErrorLogBLL(_unitOfWork).AddExceptionLog(ex);
                throw;
            }
        }
        [HttpPost]
        public IActionResult DeleteOrder(string DPID)
        {
            try
            {
                int.TryParse(EncryptDecrypt.Decrypt(DPID), out int id);
                JsonResponse jsonResponse = new JsonResponse();
                var result = _OrderBLL.Delete(id);
                if (result > 0)
                {
                    jsonResponse.Status = true;
                    jsonResponse.Message = "Order has been deleted";
                    jsonResponse.RedirectURL = Url.Action("Index", "Order");
                }
                return Json(jsonResponse);
            }
            catch (Exception ex)
            {
                new ErrorLogBLL(_unitOfWork).AddExceptionLog(ex);
                throw;
            }
        }
        public IActionResult OrderView(string DPID, string RedirectURL)
        {
            TempData["RedirectURL"] = RedirectURL;
            int.TryParse(EncryptDecrypt.Decrypt(DPID), out int id);
            ViewBag.View = true;
            OrderMaster model = _OrderBLL.GetOrderMasterById(id);

            if (!SessionHelper.LoginUser.IsDistributor)
            {
                SessionHelper.SAPOrderPendingValue = _OrderBLL.GetPendingOrderValue(model.Distributor.DistributorSAPCode, _Configuration).ToList();
                SessionHelper.SAPOrderPendingQuantity = _OrderBLL.GetDistributorPendingQuantity(model.Distributor.DistributorSAPCode, _Configuration).ToList();
                SessionHelper.DistributorBalance = _OrderBLL.GetBalance(model.Distributor.DistributorSAPCode, _Configuration);
            }
            return View(BindOrderMaster(model, id));
        }
        [HttpPost]
        public IActionResult ApproveOrder(List<ProductDetail> model, int[] companyId)
        {
            JsonResponse jsonResponse = new JsonResponse();
            try
            {
                var OrderId = model.First().OrderNumber;
                var order = _OrderBLL.GetOrderMasterById(OrderId);
                var OrderDetail = _orderDetailBLL.Where(e => e.OrderId == OrderId).ToList();
                if (SessionHelper.AddProduct.Where(x => x.IsProductSelected == true).Count() == 0)
                {
                    jsonResponse.Status = false;
                    jsonResponse.Message = "Select at least one product";
                    return Json(new { data = jsonResponse });
                }
                if (companyId.Count() > 0)
                {
                    model = model.Where(x => companyId.Contains(x.Company.Id)).ToList();
                }
                foreach (var item in model)
                {
                    var Detail = OrderDetail.First(e => e.ProductId == item.ProductMasterId);
                    Detail.ApprovedQuantity = item.ProductMaster.ApprovedQuantity;
                    Detail.IsProductSelected = item.ProductMaster.ApprovedQuantity == 0 ? false : SessionHelper.AddProduct.FirstOrDefault(x => x.ProductMasterId == item.ProductMasterId).IsProductSelected;
                    _orderDetailBLL.Update(Detail);
                }
                //var Client = new RestClient(_Configuration.PostOrder);
                //var orderdddd = _OrderBLL.PlaceOrderToSAP(OrderId).ToDataTable();
                //var request = new RestRequest(Method.POST).AddJsonBody(_OrderBLL.PlaceOrderToSAP(OrderId), "json");
                //IRestResponse response = Client.Execute(request);
                //var SAPProduct = JsonConvert.DeserializeObject<List<SAPOrderStatus>>(response.Content);
                List<SAPOrderStatus> SAPProduct = _OrderBLL.PostDistributorOrder(OrderId, _Configuration);
                var detail = _orderDetailBLL.Where(e => e.OrderId == OrderId).ToList();
                if (SAPProduct != null)
                {
                    foreach (var item in SAPProduct)
                    {
                        var product = detail.FirstOrDefault(e => e.ProductMaster.SAPProductCode == item.ProductCode);
                        if (product != null)
                        {
                            if (!string.IsNullOrEmpty(item.SAPOrderNo))
                            {
                                product.SAPOrderNumber = item.SAPOrderNo;
                                product.OrderProductStatus = OrderStatus.InProcess;
                                _orderDetailBLL.Update(product);
                            }
                        }
                    }
                    var UpdatedOrderDetail = _orderDetailBLL.Where(e => e.OrderId == OrderId && e.SAPOrderNumber == null).ToList();
                    order.Status = UpdatedOrderDetail.Count > 0 ? OrderStatus.PartiallyApproved : OrderStatus.InProcess;
                    order.ApprovedBy = SessionHelper.LoginUser.Id;
                    order.ApprovedDate = DateTime.Now;
                    var result = _OrderBLL.Update(order);
                    jsonResponse.Status = result > 0;
                    jsonResponse.Message = result > 0 ? "Order has been approved" : "Unable to approved order";
                }
                else
                {
                    jsonResponse.Status = false;
                    jsonResponse.Message = "Unable to approved order";
                }
                jsonResponse.RedirectURL = Url.Action("Index", "Order");
                jsonResponse.SignalRResponse = new SignalRResponse() { UserId = order.CreatedBy.ToString(), Number = "Order #: " + order.SNo, Message = "Order has been approved", Status = Enum.GetName(typeof(OrderStatus), order.Status) };
                return Json(new { data = jsonResponse });
            }
            catch (Exception ex)
            {
                jsonResponse.Status = false;
                jsonResponse.Message = ex.Message;
                jsonResponse.RedirectURL = Url.Action("Index", "Order");
                new ErrorLogBLL(_unitOfWork).AddExceptionLog(ex);
                return Json(new { data = jsonResponse });
            }
        }
        public JsonResult AddProduct(int Quantity, int Product)
        {
            JsonResponse jsonResponse = new JsonResponse();
            if (SessionHelper.AddProduct is null)
            {
                SessionHelper.AddProduct = new List<ProductDetail>();
            }
            if (!SessionHelper.AddProduct.Any(e => e.ProductMasterId == Product))
            {
                var master = _productDetailBLL.GetProductDetailByMasterId(Product);
                if (master != null)
                {
                    master.ProductMaster.Quantity = Quantity;
                    master.TotalPrice = master.ProductMaster.Quantity * master.ProductMaster.Rate;
                    master.Discount = master.ProductMaster.Discount;
                    var list = SessionHelper.AddProduct;
                    master.OrderNumber = list.Count == 0 ? 1 : list.Max(e => e.OrderNumber) + 1;
                    list.Add(master);
                    SessionHelper.AddProduct = list;
                    jsonResponse.Status = true;
                    jsonResponse.Message = "Product added successfully";
                    jsonResponse.RedirectURL = string.Empty;
                    jsonResponse.HtmlString = RenderRazorViewToString("AddToGrid", SessionHelper.AddProduct.OrderByDescending(e => e.OrderNumber).ToList());
                }
            }
            else
            {
                jsonResponse.Status = false;
                jsonResponse.Message = "Product already exists";
                jsonResponse.RedirectURL = string.Empty;
                jsonResponse.HtmlString = RenderRazorViewToString("AddToGrid", SessionHelper.AddProduct.OrderByDescending(e => e.OrderNumber).ToList());
            }
            return Json(new { data = jsonResponse });
        }
        public IActionResult Delete(string DPID)
        {
            int.TryParse(EncryptDecrypt.Decrypt(DPID), out int id);
            var list = SessionHelper.AddProduct;
            var item = list.FirstOrDefault(e => e.ProductMasterId == id);
            if (item != null)
            {
                list.Remove(item);
            }
            SessionHelper.AddProduct = list;
            return PartialView("AddToGrid", SessionHelper.AddProduct.OrderByDescending(e => e.OrderNumber));
        }
        [HttpPost]
        public JsonResult SaveEdit(OrderMaster model, SubmitStatus btnSubmit)
        {
            JsonResponse jsonResponse = new JsonResponse();
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
                    if (SessionHelper.AddProduct.Count > 0)
                    {
                        if (btnSubmit == SubmitStatus.Draft)
                        {
                            model.Status = OrderStatus.Draft;
                        }
                        else
                        {
                            model.Status = OrderStatus.PendingApproval;
                        }
                        string FolderPath = _IConfiguration.GetSection("Settings").GetSection("FolderPath").Value;
                        string[] permittedExtensions = Common.permittedExtensions;
                        if (model.AttachmentFormFile != null)
                        {
                            var ext = Path.GetExtension(model.AttachmentFormFile.FileName).ToLowerInvariant();
                            if (model.AttachmentFormFile.Length >= Convert.ToInt64(_Configuration.FileSize))
                            {
                                Tuple<bool, string> tuple = FileUtility.UploadFile(model.AttachmentFormFile, FolderName.Order, FolderPath);
                                if (tuple.Item1)
                                {
                                    model.Attachment = tuple.Item2;
                                }
                            }
                        }
                        jsonResponse = _OrderBLL.Save(model, Url);
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
        private OrderMaster BindOrderMaster(OrderMaster model, int Id, bool forApprove = false)
        {
            if (Id > 0)
            {
                List<OrderDetail> OrderDetailList = _orderDetailBLL.GetOrderDetailByIdByMasterId(Id);
                List<OrderDetail> orderDetail = forApprove ? OrderDetailList.Where(e => e.OrderProductStatus is null).ToList() : OrderDetailList;
                model.productDetails = _productDetailBLL.GetAllProductDetailById(OrderDetailList.Where(e => e.OrderId == Id && (forApprove ? e.OrderProductStatus == null : true)).ToList().Select(e => e.ProductId).ToArray(), Id);
                model.productDetails.ForEach(e => e.OrderNumber = Id);
                model.OrderValueViewModel = _OrderBLL.GetOrderValueModel(_OrderValueBLL.GetOrderValueByOrderId(Id));
                if (model.Status == OrderStatus.PendingApproval || model.Status == OrderStatus.PartiallyApproved)
                {
                    model.productDetails.ForEach(x => x.IsProductSelected = true);
                }
                else
                {
                    model.productDetails.ForEach(x => x.IsProductSelected = orderDetail.First(y => y.ProductId == x.ProductMasterId).IsProductSelected);
                    model.productDetails.ForEach(x => x.ProductMaster.Amount = orderDetail.First(y => y.ProductId == x.ProductMasterId).Amount);
                    model.productDetails.ForEach(x => x.ProductMaster.ApprovedQuantity = orderDetail.First(y => y.ProductId == x.ProductMasterId).ApprovedQuantity);
                }
                model.productDetails.ForEach(x => x.PendingQuantity = SessionHelper.SAPOrderPendingQuantity.FirstOrDefault(y => y.ProductCode == x.ProductMaster.SAPProductCode) != null ? Math.Floor(Convert.ToDouble(SessionHelper.SAPOrderPendingQuantity.FirstOrDefault(z => z.ProductCode == x.ProductMaster.SAPProductCode).PendingQuantity)).ToString() : "0");
                model.productDetails.ForEach(x => x.DispatchQuantity = SessionHelper.SAPOrderPendingQuantity.FirstOrDefault(y => y.ProductCode == x.ProductMaster.SAPProductCode) != null ? Math.Floor(Convert.ToDouble(SessionHelper.SAPOrderPendingQuantity.FirstOrDefault(z => z.ProductCode == x.ProductMaster.SAPProductCode).DispatchQuantity)).ToString() : "0");
                SessionHelper.AddProduct = model.productDetails;
            }
            else
            {
                model = new OrderMaster();
                model.productDetails = new List<ProductDetail>();
                model.OrderValueViewModel = new OrderValueViewModel();
            }
            model.Distributor = SessionHelper.LoginUser.Distributor ?? new DistributorBLL(_unitOfWork).Where(x => x.Id == model.DistributorId).First();
            model.ProductList = _productDetailBLL.DropDownProductList();
            return model;
        }
        public ActionResult UpdateOrderValue()
        {
            var OrderVal = _OrderBLL.GetOrderValueModel(SessionHelper.AddProduct);
            ViewBag.OrderValue = OrderVal;
            return PartialView("OrderValue", OrderVal);
        }
        public ActionResult ApprovedOrderValue(int Product, int Quantity)
        {
            var list = SessionHelper.AddProduct;
            list.First(e => e.ProductMasterId == Product).ProductMaster.Quantity = Quantity;
            SessionHelper.AddProduct = list;
            var OrderVal = _OrderBLL.GetOrderValueModel(SessionHelper.AddProduct);
            return PartialView("OrderValue", OrderVal);
        }
        private async Task<string> RenderRazorViewToString(string viewName, object model)
        {
            ViewData.Model = model;
            using (var sw = new StringWriter())
            {
                var viewResult = _viewEngine.FindView(ControllerContext, viewName, false);
                var viewContext = new ViewContext(ControllerContext, viewResult.View,
                                             ViewData, TempData, sw, new HtmlHelperOptions());
                await viewResult.View.RenderAsync(viewContext);
                return sw.GetStringBuilder().ToString();
            }
        }
        private List<OrderMaster> GetOrderList()
        {
            if (SessionHelper.LoginUser.IsDistributor)
            {
                return _OrderBLL.Where(x => x.IsDeleted == false && x.DistributorId == SessionHelper.LoginUser.DistributorId).ToList();
            }
            else
            {
                return _OrderBLL.Where(x => x.IsDeleted == false && x.Status != OrderStatus.Cancel && x.Status != OrderStatus.Draft).ToList();
            }
        }
        public JsonResult CheckProductLicense(int ProductMasterId)
        {
            JsonResponse jsonResponse = new JsonResponse();
            try
            {
                ProductMaster productMaster = new ProductMaster();
                ProductDetail Product = _productDetailBLL.Where(x => x.ProductMasterId == ProductMasterId).First();
                List<LicenseControl> LicenseControlList = _licenseControlBLL.Where(e => e.IsActive == true && e.IsDeleted == false && e.IsMandatory == true).ToList();
                List<DistributorLicense> DistributorLicenseList = _DistributorLicenseBLL.Where(x => x.Status == LicenseStatus.Verified && x.DistributorId == SessionHelper.LoginUser.DistributorId);

                if (LicenseControlList.Count() == 0 || LicenseControlList is null)
                {
                    jsonResponse.Status = true;
                    return Json(new { data = jsonResponse });
                }
                if (Product.LicenseControlId is null)
                {
                    var Challan = DistributorLicenseList.Where(x => x.Type == LicenseType.Challan && x.Status == LicenseStatus.Verified && x.Expiry > DateTime.Now).OrderBy(x => x.CreatedDate).FirstOrDefault();
                    var License = DistributorLicenseList.Where(x => x.Type == LicenseType.License && x.Status == LicenseStatus.Verified).OrderBy(x => x.CreatedDate).FirstOrDefault();
                    var LicenseControl = LicenseControlList.FirstOrDefault(x => License != null && x.Id == License.LicenseId);

                    if (Challan != null)
                    {
                        jsonResponse.Status = true;
                    }
                    else if (Challan == null && LicenseControl != null && DateTime.Now < License.Expiry)
                    {
                        jsonResponse.Status = true;
                    }
                    else if (Challan == null && LicenseControl != null && DateTime.Now > License.Expiry && DateTime.Now < License.Expiry.AddDays(License.LicenseControl.LicenseAcceptanceInDay))
                    {
                        jsonResponse.Status = true;
                        jsonResponse.Message = NotificationMessage.OrderTemporarilyAllowed;
                    }
                    else if (Challan == null && LicenseControl != null && DateTime.Now > License.Expiry.AddDays(License.LicenseControl.LicenseAcceptanceInDay))
                    {
                        jsonResponse.Status = false;
                        jsonResponse.Message = "Your " + License.LicenseControl.LicenseName + " license has been expired.";
                    }
                    else
                    {
                        jsonResponse.Status = false;
                        jsonResponse.Message = NotificationMessage.AddVerifiedLicense;
                    }
                }
                if (Product.LicenseControlId != null)
                {
                    var Challan = DistributorLicenseList.Where(x => x.LicenseId == Product.LicenseControlId && x.Type == LicenseType.Challan && x.Status == LicenseStatus.Verified && x.Expiry > DateTime.Now).OrderBy(x => x.CreatedDate).FirstOrDefault();
                    var License = DistributorLicenseList.Where(x => x.LicenseId == Product.LicenseControlId && x.Type == LicenseType.License && x.Status == LicenseStatus.Verified).OrderBy(x => x.CreatedDate).FirstOrDefault();
                    var LicenseControl = LicenseControlList.FirstOrDefault(x => x.Id == Product.LicenseControlId);

                    foreach (var item in LicenseControlList)
                    {
                        if (item.IsMandatory)
                        {
                            var result = DistributorLicenseList.Where(x => x.LicenseId == item.Id).FirstOrDefault();
                            if (result is null)
                            {
                                jsonResponse.Status = false;
                                jsonResponse.Message = "Add verified " + item.LicenseName + " license or challan before placing the order.";
                                return Json(new { data = jsonResponse });
                            }
                        }
                    }

                    if (LicenseControl != null)
                    {
                        if (LicenseControl.IsMandatory)
                        {
                            var result = DistributorLicenseList.Where(x => x.LicenseId == LicenseControl.Id).FirstOrDefault();
                            if (result is null)
                            {
                                jsonResponse.Status = false;
                                jsonResponse.Message = "Add verified " + LicenseControl.LicenseName + " license or challan before placing the order.";
                                return Json(new { data = jsonResponse });
                            }
                        }
                    }

                    if (Challan is null && License is null)
                    {
                        jsonResponse.Status = false;
                        jsonResponse.Message = NotificationMessage.AddVerifiedLicense;
                    }
                    if (Challan != null || License != null)
                    {
                        if (Challan == null && LicenseControl != null && License != null && DateTime.Now > License.Expiry.AddDays(License.LicenseControl.LicenseAcceptanceInDay))
                        {
                            jsonResponse.Status = false;
                            jsonResponse.Message = "Your " + License.LicenseControl.LicenseName + " license has been expired.";
                        }
                        else if (License != null && DateTime.Now > License.Expiry && DateTime.Now < License.Expiry.AddDays(License.LicenseControl.LicenseAcceptanceInDay))
                        {
                            jsonResponse.Status = true;
                            jsonResponse.Message = NotificationMessage.OrderTemporarilyAllowed;
                        }
                        else if (License.Expiry.AddDays(License.LicenseControl.LicenseAcceptanceInDay) < DateTime.Now)
                        {
                            jsonResponse.Status = false;
                            jsonResponse.Message = "Add verified " + License.LicenseControl.LicenseName + " license or challan before placing the order.";

                        }
                        else
                        {
                            jsonResponse.Status = true;
                        }
                    }
                    else if (Challan == null && LicenseControl != null && License != null && DateTime.Now < License.Expiry)
                    {
                        jsonResponse.Status = true;
                    }
                    else if (Challan == null && LicenseControl != null && License != null && DateTime.Now > License.Expiry && DateTime.Now < License.Expiry.AddDays(License.LicenseControl.LicenseAcceptanceInDay))
                    {
                        jsonResponse.Status = true;
                        jsonResponse.Message = NotificationMessage.OrderTemporarilyAllowed;
                    }
                    else if (Challan == null && LicenseControl != null && License != null && DateTime.Now > License.Expiry.AddDays(License.LicenseControl.LicenseAcceptanceInDay))
                    {
                        jsonResponse.Status = false;
                        jsonResponse.Message = "Your " + License.LicenseControl.LicenseName + " license has been expired.";
                    }
                    else
                    {
                        jsonResponse.Status = false;
                        jsonResponse.Message = NotificationMessage.AddVerifiedLicense;
                    }
                }

                if (jsonResponse.Status)
                {
                    productMaster = _ProductMasterBLL.GetProductMasterById(ProductMasterId);
                }
                return Json(new { data = jsonResponse, productMaster = productMaster });
            }
            catch (Exception ex)
            {
                jsonResponse.Status = false;
                jsonResponse.Message = ex.Message;
                jsonResponse.RedirectURL = string.Empty;
                new ErrorLogBLL(_unitOfWork).AddExceptionLog(ex);
                return Json(new { data = jsonResponse });
            }
        }

        //public JsonResult CheckProductLicense(int ProductMasterId)
        //{
        //    JsonResponse jsonResponse = new JsonResponse();
        //    try
        //    {
        //        ProductMaster productMaster = new ProductMaster();
        //        ProductDetail Product = _productDetailBLL.Where(x => x.ProductMasterId == ProductMasterId).First();
        //        List<DistributorLicense> DistributorLicenseList = _DistributorLicenseBLL.Where(x => x.Status == LicenseStatus.Verified && x.DistributorId == SessionHelper.LoginUser.DistributorId);

        //        if (Product.LicenseControlId == null)
        //        {
        //            var LicenseControlList = _licenseControlBLL.Where(e => e.IsActive == true && e.IsDeleted == false && e.IsMandatory).Select(e => e.Id).ToList();
        //            var MandatoryLicense = DistributorLicenseList.Where(e => LicenseControlList.Contains((int)e.LicenseId)).ToList();
        //            if (MandatoryLicense.Where(e => e.Expiry > DateTime.Now).Count() > 0)
        //            {
        //                var ExpireLicense = MandatoryLicense.Where(e => e.Expiry > DateTime.Now).First();
        //                if (ExpireLicense.Expiry.AddDays(ExpireLicense.LicenseControl.LicenseAcceptanceInDay) > DateTime.Now)
        //                {
        //                    jsonResponse.Status = true;
        //                    jsonResponse.Message = "Your license has been expired, but your are temporay allowed to place the order.";
        //                }
        //            }
        //            return Json(new { data = jsonResponse });
        //        }
        //        else
        //        {
        //            var LicenseControlList = _licenseControlBLL.Where(e => e.IsActive == true && e.IsDeleted == false && e.IsMandatory).Select(e => e.Id).ToList();
        //            var MandatoryLicense = DistributorLicenseList.Where(e => LicenseControlList.Contains((int)e.LicenseId)).ToList();
        //            if (MandatoryLicense.Where(e => e.Expiry > DateTime.Now).Count() > 0)
        //            {
        //                var ExpireLicense = MandatoryLicense.Where(e => e.Expiry > DateTime.Now).First();
        //                if (ExpireLicense.Expiry.AddDays(ExpireLicense.LicenseControl.LicenseAcceptanceInDay) > DateTime.Now)
        //                {
        //                    jsonResponse.Status = true;
        //                    jsonResponse.Message = "Your license has been expired, but your are temporay allowed to place the order.";
        //                    return Json(new { data = jsonResponse });
        //                }
        //                else
        //                {
        //                    jsonResponse.Status = false;
        //                    jsonResponse.Message = "Please add " + ExpireLicense.LicenseControl.LicenseName + " License before placing the order";
        //                    return Json(new { data = jsonResponse });
        //                }
        //            }
        //        }

        //        if (jsonResponse.Status)
        //        {
        //            productMaster = _ProductMasterBLL.GetProductMasterById(ProductMasterId);
        //        }
        //        return Json(new { data = jsonResponse, productMaster = productMaster });
        //    }
        //    catch (Exception ex)
        //    {
        //        jsonResponse.Status = false;
        //        jsonResponse.Message = ex.Message;
        //        jsonResponse.RedirectURL = string.Empty;
        //        new ErrorLogBLL(_unitOfWork).AddExceptionLog(ex);
        //        return Json(new { data = jsonResponse });
        //    }
        //}

        public IActionResult Search(OrderSearch model, string Search)
        {
            if (!string.IsNullOrEmpty(Search))
            {
                model = List(model);
            }
            else
            {
                model.OrderMaster = GetOrderList();
            }
            return PartialView("List", model.OrderMaster);
        }
        public OrderSearch List(OrderSearch model)
        {
            if (model.DistributorId is null && model.Status is null && model.FromDate is null && model.ToDate is null && model.OrderNo is null)
            {
                model.OrderMaster = GetOrderList();
            }
            else
            {
                model.OrderMaster = _OrderBLL.Search(model).Where(x => SessionHelper.LoginUser.IsDistributor == true ? x.DistributorId == SessionHelper.LoginUser.DistributorId : x.Status != OrderStatus.Cancel && x.Status != OrderStatus.Draft).ToList();
            }
            return model;
        }
        public IActionResult GetCompanyProduct(int[] companyId, int OrderId)
        {
            JsonResponse jsonResponse = new JsonResponse();
            List<ProductDetail> list = new List<ProductDetail>();

            OrderMaster model = _OrderBLL.GetOrderMasterById(OrderId);
            OrderMaster orderMaster = _OrderBLL.GetOrderMasterById(OrderId);
            ViewBag.Status = orderMaster.Status;
            if (companyId.Count() > 0)
            {
                list = SessionHelper.AddProduct.Where(x => companyId.Contains(x.CompanyId)).ToList();
            }
            jsonResponse.HtmlString = RenderRazorViewToString("Grid", list);
            return Json(new { data = jsonResponse, companyId = companyId });
        }
        public void SelectProduct(string DPID)
        {
            var list = SessionHelper.AddProduct;
            int.TryParse(EncryptDecrypt.Decrypt(DPID), out int id);
            var product = list.FirstOrDefault(e => e.ProductMasterId == id);

            if (product != null)
            {
                if (product.IsProductSelected)
                {
                    list.FirstOrDefault(e => e.ProductMasterId == id).IsProductSelected = false;
                }
                else
                {
                    list.FirstOrDefault(e => e.ProductMasterId == id).IsProductSelected = true;
                }
            }

            SessionHelper.AddProduct = list;
        }
        public void SelectAllProduct(bool IsSelected)
        {
            var list = SessionHelper.AddProduct;
            if (IsSelected)
            {
                list.ForEach(x => x.IsProductSelected = true);
            }
            else
            {
                list.ForEach(x => x.IsProductSelected = false);
            }
            SessionHelper.AddProduct = list;
        }
    }
}
