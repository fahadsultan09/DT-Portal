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
            new AuditLogBLL(_unitOfWork).AddAuditLog("OrderMaster", "Index", " Form");
            return View(GetOrderList());
        }
        public IActionResult List()
        {
            return PartialView("List", GetOrderList());
        }
        [HttpGet]
        public IActionResult Add(int id)
        {
            SessionHelper.AddProduct = new List<ProductDetail>();
            return View("AddDetail", BindOrderMaster(id));
        }
        [HttpGet]
        public IActionResult Approve(int id)
        {
            var order = BindOrderMaster(id);
            SessionHelper.DistributorBalance = _OrderBLL.GetBalance(order.Distributor.DistributorSAPCode, _Configuration);
            ViewBag.Status = order.Status;
            return View("OrderApprove", order);
        }
        public IActionResult OnHold(int id, string Comments)
        {
            try
            {
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

        public IActionResult Reject(int id, string Comments)
        {
            try
            {
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
        public IActionResult OrderView(int id)
        {
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
                var request = new RestRequest(Method.POST).AddJsonBody(_OrderBLL.PlaceOrderToSAP(OrderId), "json");
                IRestResponse response = Client.Execute(request);
                var SAPProduct = JsonConvert.DeserializeObject<List<SAPOrderStatus>>(response.Content);
                var detail = _orderDetailBLL.Where(e => e.OrderId == OrderId).ToList();
                foreach (var item in SAPProduct)
                {
                    var product = detail.FirstOrDefault(e => e.ProductMaster.SAPProductCode == item.ProductCode);
                    if (product != null)
                    {
                        product.SAPOrderNumber = item.SAPOrderNo;
                        product.OrderProductStatus = OrderStatus.NotYetProcess;
                        _orderDetailBLL.Update(product);
                    }
                }
                var order = _OrderBLL.GetOrderMasterById(OrderId);
                order.Status = OrderStatus.InProcess;
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
        public IActionResult Delete(int Id)
        {
            var list = SessionHelper.AddProduct;
            var item = list.FirstOrDefault(e => e.ProductMasterId == Id);
            if (item != null)
            {
                list.Remove(item);
            }
            SessionHelper.AddProduct = list;
            new AuditLogBLL(_unitOfWork).AddAuditLog("OrderMaster", "Delete", "End Click on Delete Button of ");
            return PartialView("AddToGrid", SessionHelper.AddProduct.OrderByDescending(e => e.OrderNumber));
        }
        [HttpPost]
        public JsonResult SaveEdit(OrderMaster model, SubmitStatus btnSubmit)
        {
            JsonResponse jsonResponse = new JsonResponse();
            try
            {
                new AuditLogBLL(_unitOfWork).AddAuditLog("OrderMaster", "SaveEdit", "Start Click on SaveEdit Button of ");
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
                new AuditLogBLL(_unitOfWork).AddAuditLog("OrderMaster", "SaveEdit", "End Click on Save Button of ");
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
        private OrderMaster BindOrderMaster(int Id)
        {
            OrderMaster model = new OrderMaster();
            if (Id > 0)
            {
                model = _OrderBLL.GetOrderMasterById(Id);
                List<OrderDetail> orderDetail = _orderDetailBLL.GetOrderDetailByIdByMasterId(Id);
                model.productDetails = _productDetailBLL.GetAllProductDetailById(_orderDetailBLL.Where(e => e.OrderId == Id).ToList().Select(e => e.ProductId).ToArray(), Id);
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
        public async Task<string> RenderRazorViewToString(string viewName, object model)
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
        public List<OrderMaster> GetOrderList()
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
                ProductDetail productDetail = _productDetailBLL.Where(x => x.ProductMasterId == ProductMasterId).First();
                var licenseControl = _licenseControlBLL.Where(e => e.IsMandatory == true && e.IsActive == true && e.IsDeleted == false).ToList();
                if (productDetail.LicenseControlId != null)
                {
                    licenseControl.Add(productDetail.LicenseControl);
                }
                var distributorLicense = _DistributorLicenseBLL.Where(e => e.Status == LicenseStatus.Verified);
                jsonResponse.Status = true;
                foreach (var item in licenseControl)
                {
                    var license = distributorLicense.FirstOrDefault(e => e.LicenseId == item.Id);
                    if (license != null)
                    {
                        if (license.Expiry.AddDays(license.LicenseControl.LicenseAcceptanceInDay) > DateTime.Now)
                        {
                            jsonResponse.Status = false;
                            jsonResponse.Message = "Your " + license.LicenseControl.LicenseName + " license has been expired. Please renew the license";
                            return Json(new { data = jsonResponse });
                        }
                        else
                        {
                            jsonResponse.Status = true;
                        }
                    }
                    else
                    {
                        jsonResponse.Status = false;
                        jsonResponse.Message = item.LicenseName + " license required for selected product";
                        return Json(new { data = jsonResponse });
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
        public void SelectProduct(int Id)
        {
            var list = SessionHelper.AddProduct;
            var product = list.FirstOrDefault(e => e.ProductMasterId == Id);
            if (product != null)
            {
                if (product.IsProductSelected)
                {
                    list.FirstOrDefault(e => e.ProductMasterId == Id).IsProductSelected = false;
                }
                else
                {
                    list.FirstOrDefault(e => e.ProductMasterId == Id).IsProductSelected = true;
                }
            }
            SessionHelper.AddProduct = list;
        }
    }
}
