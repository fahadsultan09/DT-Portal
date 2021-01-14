using BusinessLogicLayer.Application;
using BusinessLogicLayer.ErrorLog;
using BusinessLogicLayer.HelperClasses;
using DataAccessLayer.WorkProcess;
using DistributorPortal.BusinessLogicLayer.ApplicationSetup;
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
using System.Text;
using System.Threading.Tasks;
using Utility;
using Utility.Constant;
using Utility.HelperClasses;
using static Utility.Constant.Common;

namespace DistributorPortal.Controllers
{
    public class OrderController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly OrderBLL _OrderBLL;
        private readonly OrderDetailBLL _orderDetailBLL;
        private readonly OrderValueBLL _OrderValueBLL;
        private readonly ProductDetailBLL _productDetailBLL;
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
            _IConfiguration = configuration;
            _viewEngine = viewEngine;
            _Configuration = _configuration;
        }
        // GET: Order
        public ActionResult Index()
        {
            new AuditTrailBLL(_unitOfWork).AddAuditTrail("OrderMaster", "Index", " Form");
            return View(GetOrderList());
        }
        public IActionResult List()
        {
            return PartialView("List", GetOrderList());
        }
        [HttpGet]
        public IActionResult Add(int id)
        {
            new AuditTrailBLL(_unitOfWork).AddAuditTrail("OrderMaster", "Add", "Click on Add  Button of ");
            SessionHelper.AddProduct = new List<ProductDetail>();
            return View("AddDetail", BindOrderMaster(id));
        }
        [HttpGet]
        public IActionResult Approve(int id)
        {
            return View("OrderApprove", BindOrderMaster(id));
        }
        public IActionResult OnHold(int id, string Comments)
        {
            try
            {
                JsonResponse jsonResponse = new JsonResponse();
                var order = _OrderBLL.GetOrderMasterById(id);
                order.Status = OrderStatus.Onhold;
                order.Comments = Comments;
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
                order.Comments = Comments;
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
            return View(BindOrderMaster(id));
        }

        [HttpGet]
        public IActionResult ApproveOrder(int id)
        {
            JsonResponse jsonResponse = new JsonResponse();
            try
            {                
                var Client = new RestClient(_Configuration.PostOrder);
                var request = new RestRequest(Method.POST).AddJsonBody(_OrderBLL.PlaceOrderToSAP(id), "json");
                IRestResponse response = Client.Execute(request);
                var SAPProduct = JsonConvert.DeserializeObject<List<SAPOrderStatus>>(response.Content);
                var detail = _orderDetailBLL.Where(e => e.OrderId == id).ToList();
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
                var order = _OrderBLL.GetOrderMasterById(id);
                order.Status = OrderStatus.InProcess;
                var result = _OrderBLL.Update(order);
                jsonResponse.Status = result > 0;
                jsonResponse.Message = result > 0 ? "Order has been approved" : "Unable to approve order";
                jsonResponse.RedirectURL = Url.Action("Index", "Order");
                return Json(jsonResponse);
            }
            catch (Exception ex)
            {
                jsonResponse.Status = false;
                jsonResponse.Message = ex.Message;
                jsonResponse.RedirectURL = Url.Action("Index", "Order");
                new ErrorLogBLL(_unitOfWork).AddExceptionLog(ex);
                return Json(jsonResponse);
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
            new AuditTrailBLL(_unitOfWork).AddAuditTrail("OrderMaster", "Delete", "Start Click on Delete Button of ");
            var list = SessionHelper.AddProduct;
            var item = list.FirstOrDefault(e => e.ProductMasterId == Id);
            if (item != null)
            {
                list.Remove(item);
            }
            SessionHelper.AddProduct = list;
            new AuditTrailBLL(_unitOfWork).AddAuditTrail("OrderMaster", "Delete", "End Click on Delete Button of ");
            return PartialView("AddToGrid", SessionHelper.AddProduct.OrderByDescending(e => e.OrderNumber));
        }
        [HttpPost]
        public JsonResult SaveEdit(OrderMaster model, SubmitStatus btnSubmit)
        {
            JsonResponse jsonResponse = new JsonResponse();            
            try
            {
                new AuditTrailBLL(_unitOfWork).AddAuditTrail("OrderMaster", "SaveEdit", "Start Click on SaveEdit Button of ");
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
                new AuditTrailBLL(_unitOfWork).AddAuditTrail("OrderMaster", "SaveEdit", "End Click on Save Button of ");
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
                model.productDetails = _productDetailBLL.GetAllProductDetailById(_orderDetailBLL.Where(e => e.OrderId == Id).ToList().Select(e => e.ProductId).ToArray(), Id);
                model.OrderValueViewModel = _OrderBLL.GetOrderValueModel(_OrderValueBLL.GetOrderValueByOrderId(Id));
                SessionHelper.AddProduct = model.productDetails;
            }
            else
            {
                model.productDetails = new List<ProductDetail>();
                model.OrderValueViewModel = new OrderValueViewModel();
            }
            model.Distributor = SessionHelper.LoginUser.Distributor;
            model.ProductList = new ProductMasterBLL(_unitOfWork).DropDownProductList();
            return model;
        }
        public ActionResult UpdateOrderValue()
        {
            var OrderVal = _OrderBLL.GetOrderValueModel(SessionHelper.AddProduct);
            return PartialView("OrderValue", OrderVal);
        }
        public async Task<string> RenderRazorViewToString(string viewName, object model)
        {
            ViewData.Model = model;
            using (var sw = new StringWriter())
            {
                var viewResult = _viewEngine.FindView(ControllerContext,viewName, false);
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
    }
}
