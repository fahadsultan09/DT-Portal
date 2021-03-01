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
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Utility;
using Utility.Constant;
using Utility.HelperClasses;

namespace DistributorPortal.Controllers
{
    public class OrderController : BaseController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly OrderBLL _OrderBLL;
        private readonly OrderDetailBLL _orderDetailBLL;
        private readonly OrderValueBLL _OrderValueBLL;
        private readonly ProductDetailBLL _productDetailBLL;
        private readonly DistributorLicenseBLL _DistributorLicenseBLL;
        private readonly LicenseControlBLL _licenseControlBLL;
        private readonly IConfiguration _IConfiguration;
        private ICompositeViewEngine _viewEngine;
        private readonly Configuration _Configuration;

        public OrderController(IUnitOfWork unitOfWork, Configuration _configuration, IConfiguration configuration, ICompositeViewEngine viewEngine)
        {
            _unitOfWork = unitOfWork;
            _OrderBLL = new OrderBLL(_unitOfWork);
            _productDetailBLL = new ProductDetailBLL(_unitOfWork);
            _orderDetailBLL = new OrderDetailBLL(_unitOfWork);
            _OrderValueBLL = new OrderValueBLL(_unitOfWork);
            _DistributorLicenseBLL = new DistributorLicenseBLL(_unitOfWork);
            _licenseControlBLL = new LicenseControlBLL(_unitOfWork);
            _IConfiguration = configuration;
            _viewEngine = viewEngine;
            _Configuration = _configuration;
        }
        // GET: Order
        public ActionResult Index()
        {
            return View(GetOrderList());
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
            return View("AddDetail", BindOrderMaster(id));
        }
        [HttpGet]
        public IActionResult Approve(string DPID)
        {
            int id = 0;
            if (!string.IsNullOrEmpty(DPID))
            {
                int.TryParse(EncryptDecrypt.Decrypt(DPID), out id);
            }
            var order = BindOrderMaster(id, true);
            SessionHelper.DistributorBalance = _OrderBLL.GetBalance(order.Distributor.DistributorSAPCode, _Configuration);
            ViewBag.Status = order.Status;
            return View("OrderApprove", order);
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
                int id = 0;
                int.TryParse(EncryptDecrypt.Decrypt(DPID), out id);
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
        public IActionResult OrderView(string DPID)
        {
            int id = 0;
            int.TryParse(EncryptDecrypt.Decrypt(DPID), out id);
            ViewBag.View = true;
            return View(BindOrderMaster(id));
        }

        [HttpPost]
        public IActionResult ApproveOrder(List<ProductDetail> model, int[] companyId)
        {
            JsonResponse jsonResponse = new JsonResponse();
            try
            {
                var OrderId = model.First().OrderNumber;
                var OrderDetail = _orderDetailBLL.Where(e => e.OrderId == OrderId).ToList();
                if (companyId.Count() > 0)
                {
                    model = model.Where(x => companyId.Contains(x.Company.Id)).ToList();
                }
                foreach (var item in model)
                {
                    var Detail = OrderDetail.First(e => e.ProductId == item.ProductMasterId);
                    Detail.ApprovedQuantity = item.ProductMaster.ApprovedQuantity;
                    Detail.IsProductSelected = Detail.ApprovedQuantity == 0 ? false : SessionHelper.AddProduct.FirstOrDefault(x => x.ProductMasterId == item.ProductMasterId).IsProductSelected;
                    _orderDetailBLL.Update(Detail);
                }
                var Client = new RestClient(_Configuration.PostOrder);
                var orderdddd = _OrderBLL.PlaceOrderToSAP(OrderId).ToDataTable();
                var request = new RestRequest(Method.POST).AddJsonBody(_OrderBLL.PlaceOrderToSAP(OrderId), "json");
                IRestResponse response = Client.Execute(request);
                var SAPProduct = JsonConvert.DeserializeObject<List<SAPOrderStatus>>(response.Content);
                var detail = _orderDetailBLL.Where(e => e.OrderId == OrderId).ToList();
                foreach (var item in SAPProduct)
                {
                    var product = detail.FirstOrDefault(e => e.ProductMaster.SAPProductCode == item.ProductCode);
                    if (product != null)
                    {
                        if (!string.IsNullOrEmpty(item.SAPOrderNo))
                        {
                            product.SAPOrderNumber = item.SAPOrderNo;
                            product.OrderProductStatus = OrderStatus.NotYetProcess;
                            _orderDetailBLL.Update(product);
                        }                        
                    }
                }
                var order = _OrderBLL.GetOrderMasterById(OrderId);
                var UpdatedOrderDetail = _orderDetailBLL.Where(e => e.OrderId == OrderId && e.SAPOrderNumber == null).ToList();
                order.Status = UpdatedOrderDetail.Count > 0 ? OrderStatus.PartiallyApproved : OrderStatus.InProcess;
                order.ApprovedBy = SessionHelper.LoginUser.Id;
                order.ApprovedDate = DateTime.Now;
                var result = _OrderBLL.Update(order);
                jsonResponse.Status = result > 0;
                jsonResponse.Message = result > 0 ? "Order has been approved" : "Unable to approve order";
                jsonResponse.RedirectURL = Url.Action("Index", "Order");
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
                    jsonResponse.Message = "Product Added Successfully";
                    jsonResponse.RedirectURL = string.Empty;
                    jsonResponse.HtmlString = RenderRazorViewToString("AddToGrid", SessionHelper.AddProduct.OrderByDescending(e => e.OrderNumber).ToList());
                }
            }
            else
            {
                jsonResponse.Status = false;
                jsonResponse.Message = "Product Already Exists";
                jsonResponse.RedirectURL = string.Empty;
                jsonResponse.HtmlString = RenderRazorViewToString("AddToGrid", SessionHelper.AddProduct.OrderByDescending(e => e.OrderNumber).ToList());
            }
            return Json(new { data = jsonResponse });
        }
        public IActionResult Delete(string DPID)
        {
            int id = 0;
            int.TryParse(EncryptDecrypt.Decrypt(DPID), out id);
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
                        jsonResponse = _OrderBLL.Save(model, _IConfiguration, Url);
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
        private OrderMaster BindOrderMaster(int Id, bool forApprove = false)
        {
            OrderMaster model = new OrderMaster();
            if (Id > 0)
            {
                model = _OrderBLL.GetOrderMasterById(Id);
                List<OrderDetail> orderDetail = forApprove ? _orderDetailBLL.GetOrderDetailByIdByMasterId(Id).Where(e => e.OrderProductStatus is null).ToList() : _orderDetailBLL.GetOrderDetailByIdByMasterId(Id);
                model.productDetails = _productDetailBLL.GetAllProductDetailById(_orderDetailBLL.Where(e => e.OrderId == Id && e.OrderProductStatus == null).ToList().Select(e => e.ProductId).ToArray(), Id);
                model.productDetails.ForEach(e => e.OrderNumber = Id);
                model.OrderValueViewModel = _OrderBLL.GetOrderValueModel(_OrderValueBLL.GetOrderValueByOrderId(Id));
                if (model.Status == OrderStatus.PendingApproval)
                {
                    model.productDetails.ForEach(x => x.IsProductSelected = true);
                }
                else
                {
                    model.productDetails.ForEach(x => x.IsProductSelected = orderDetail.First(y => y.ProductId == x.ProductMasterId).IsProductSelected);
                    model.productDetails.ForEach(x => x.ProductMaster.ApprovedQuantity = (int)orderDetail.First(y => y.ProductId == x.ProductMasterId).ApprovedQuantity);
                }
                List<SAPOrderPendingQuantity> _SAPOrderPendingQuantity = _OrderBLL.GetDistributorPendingQuantity(model.Distributor.DistributorSAPCode, _Configuration).ToList();
                model.productDetails.ForEach(x => x.PendingQuantity = _SAPOrderPendingQuantity.FirstOrDefault(y => y.ProductCode == x.ProductMaster.SAPProductCode) != null ? _SAPOrderPendingQuantity.FirstOrDefault(z => z.ProductCode == x.ProductMaster.SAPProductCode).PendingQuantity : "0");
                SessionHelper.AddProduct = model.productDetails;
            }
            else
            {
                model.productDetails = new List<ProductDetail>();
                model.OrderValueViewModel = new OrderValueViewModel();
            }

            if (SessionHelper.LoginUser.IsDistributor)
            {
                SessionHelper.SAPOrderPendingValue = _OrderBLL.GetPendingOrderValue(SessionHelper.LoginUser.Distributor.DistributorSAPCode, _Configuration).ToList();
            }
            else
            {
                SessionHelper.SAPOrderPendingValue = _OrderBLL.GetPendingOrderValue(model.Distributor.DistributorSAPCode, _Configuration).ToList();
            }
            model.Distributor = SessionHelper.LoginUser.Distributor ?? new DistributorBLL(_unitOfWork).Where(x => x.Id == model.DistributorId).First();
            model.ProductList = new ProductMasterBLL(_unitOfWork).DropDownProductList();
            return model;
        }
        public ActionResult UpdateOrderValue()
        {
            var OrderVal = _OrderBLL.GetOrderValueModel(SessionHelper.AddProduct);
            return PartialView("OrderValue", OrderVal);
        }

        public ActionResult ApprovedOrderValue(int Product, int Quantity, int Order)
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
                return _OrderBLL.GetAllOrderMaster().Where(x => x.DistributorId == SessionHelper.LoginUser.DistributorId).OrderByDescending(x => x.Id).ToList();
            }
            else
            {
                return _OrderBLL.GetAllOrderMaster().OrderByDescending(x => x.Id).ToList();
            }
        }
        public JsonResult CheckProductLicense(int ProductMasterId)
        {
            JsonResponse jsonResponse = new JsonResponse();
            try
            {
                ProductDetail Product = _productDetailBLL.Where(x => x.ProductMasterId == ProductMasterId).First();
                List<LicenseControl> LicenseControlList = _licenseControlBLL.Where(e => e.IsActive == true && e.IsDeleted == false).ToList();
                List<DistributorLicense> DistributorLicenseList = _DistributorLicenseBLL.Where(x => x.Status == LicenseStatus.Verified);

                if (Product.LicenseControlId is null)
                {
                    var Challan = DistributorLicenseList.Where(x => x.Type == LicenseType.Challan && x.Status == LicenseStatus.Verified && x.Expiry > DateTime.Now).OrderBy(x => x.CreatedDate).FirstOrDefault();
                    var License = DistributorLicenseList.Where(x => x.Type == LicenseType.License && x.Status == LicenseStatus.Verified).OrderBy(x => x.CreatedDate).FirstOrDefault() ?? new DistributorLicense();

                    foreach (var item in LicenseControlList)
                    {
                        if (item.IsMandatory)
                        {
                            var result = DistributorLicenseList.Where(x => x.LicenseId == item.Id && DateTime.Now < License.Expiry.AddDays(License.LicenseControl.LicenseAcceptanceInDay)).FirstOrDefault();
                            if (result is null)
                            {
                                jsonResponse.Status = false;
                                jsonResponse.Message = "Add verified license or challan before placing the order.";
                                return Json(new { data = jsonResponse });
                            }
                        }
                    }
                    var LicenseControl = LicenseControlList.FirstOrDefault(x => x.Id == License.LicenseId);

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
                        jsonResponse.Message = "Your license has been expired, but your are temporay allowed to place the order.";
                    }
                    else if (Challan == null && LicenseControl != null && DateTime.Now > License.Expiry.AddDays(License.LicenseControl.LicenseAcceptanceInDay))
                    {
                        jsonResponse.Status = false;
                        jsonResponse.Message = "Your " + License.LicenseControl.LicenseName + " license has been expired.";
                    }
                    else
                    {
                        jsonResponse.Status = false;
                        jsonResponse.Message = "Add verified license or challan before placing the order.";
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
                                jsonResponse.Message = "Add verified license or challan before placing the order.";
                                return Json(new { data = jsonResponse });
                            }
                        }
                    }
                    if (Challan is null && License is null)
                    {
                        jsonResponse.Status = false;
                        jsonResponse.Message = "Add verified license or challan before placing the order.";
                    }
                    if (Challan != null)
                    {
                        jsonResponse.Status = true;
                    }
                    else if (Challan == null && LicenseControl != null && License != null && DateTime.Now < License.Expiry)
                    {
                        jsonResponse.Status = true;
                    }
                    else if (Challan == null && LicenseControl != null && License != null && DateTime.Now > License.Expiry && DateTime.Now < License.Expiry.AddDays(License.LicenseControl.LicenseAcceptanceInDay))
                    {
                        jsonResponse.Status = true;
                        jsonResponse.Message = "Your license has been expired, but your are temporay allowed to place the order.";
                    }
                    else if (Challan == null && LicenseControl != null && License != null && DateTime.Now > License.Expiry.AddDays(License.LicenseControl.LicenseAcceptanceInDay))
                    {
                        jsonResponse.Status = false;
                        jsonResponse.Message = "Your " + License.LicenseControl.LicenseName + " license has been expired.";
                    }
                    else
                    {
                        jsonResponse.Status = false;
                        jsonResponse.Message = "Add verified license or challan before placing the order.";
                    }
                }
                return Json(new { data = jsonResponse });
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

        public IActionResult Search(OrderSearch orderSearch)
        {
            return Json("");
        }
        public IActionResult GetCompanyProduct(int[] companyId, int OrderId)
        {
            JsonResponse jsonResponse = new JsonResponse();
            object list = (dynamic)null;

            var order = BindOrderMaster(OrderId);
            ViewBag.Status = order.Status;
            if (companyId.Count() > 0)

            {
                list = SessionHelper.AddProduct.Where(x => companyId.Contains(x.CompanyId)).ToList();
            }
            else
            {
                list = SessionHelper.AddProduct.ToList();
            }
            jsonResponse.HtmlString = RenderRazorViewToString("Grid", list);
            return Json(new { data = jsonResponse, companyId = companyId });
        }
        public void SelectProduct(string DPID)
        {
            int id = 0;
            int.TryParse(EncryptDecrypt.Decrypt(DPID), out id);
            var list = SessionHelper.AddProduct;
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
    }
}
