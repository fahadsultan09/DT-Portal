
using BusinessLogicLayer.Application;
using BusinessLogicLayer.ApplicationSetup;
using BusinessLogicLayer.ErrorLog;
using BusinessLogicLayer.GeneralSetup;
using BusinessLogicLayer.HelperClasses;
using DataAccessLayer.WorkProcess;
using DistributorPortal.Resource;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
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
using Utility.HelperClasses;
using MoreLinq;

namespace DistributorPortal.Controllers
{
    public class OrderReturnController : BaseController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly OrderReturnBLL _OrderReturnBLL;
        private readonly OrderReturnDetailBLL _OrderReturnDetailBLL;
        private readonly ProductMasterBLL _ProductMasterBLL;
        private readonly ProductDetailBLL _ProductDetailBLL;
        private readonly OrderReturnReasonBLL _orderReturnReasonBLL;
        private ICompositeViewEngine _viewEngine;
        private readonly Configuration _Configuration;
        public OrderReturnController(IUnitOfWork unitOfWork, ICompositeViewEngine viewEngine, Configuration _configuration)
        {
            _unitOfWork = unitOfWork;
            _OrderReturnBLL = new OrderReturnBLL(_unitOfWork);
            _OrderReturnDetailBLL = new OrderReturnDetailBLL(_unitOfWork);
            _ProductMasterBLL = new ProductMasterBLL(_unitOfWork);
            _ProductDetailBLL = new ProductDetailBLL(_unitOfWork);
            _orderReturnReasonBLL = new OrderReturnReasonBLL(_unitOfWork);
            _viewEngine = viewEngine;
            _Configuration = _configuration;
        }
        public IActionResult Index()
        {
            OrderReturnViewModel model = new OrderReturnViewModel();
            model.OrderReturnMaster = GetOrderReturnList();
            model.DistributorList = new DistributorBLL(_unitOfWork).DropDownDistributorList(null);
            return View(model);
        }
        public OrderReturnViewModel List(OrderReturnViewModel model)
        {
            if (model.DistributorId is null && model.Status is null && model.FromDate is null && model.ToDate is null)
            {
                model.OrderReturnMaster = GetOrderReturnList();
            }
            else
            {
                model.OrderReturnMaster = _OrderReturnBLL.Search(model).Where(x => SessionHelper.LoginUser.IsDistributor == true ? x.DistributorId == SessionHelper.LoginUser.DistributorId : true).ToList();
            }
            return model;
        }
        [HttpGet]
        public IActionResult Add(string DPID)
        {
            int id = 0;
            if (!string.IsNullOrEmpty(DPID))
            {
                int.TryParse(EncryptDecrypt.Decrypt(DPID), out id);
            }
            SessionHelper.AddReturnProduct = new List<OrderReturnDetail>();
            return View("Add", BindOrderReturnMaster(id));
        }
        [HttpGet]
        public IActionResult View(string DPID)
        {
            int id = 0;
            int.TryParse(EncryptDecrypt.Decrypt(DPID), out id);
            SessionHelper.AddReturnProduct = new List<OrderReturnDetail>();
            return View("View", BindOrderReturnMaster(id));
        }
        [HttpPost]
        public JsonResult SaveEdit(OrderReturnMaster model, SubmitStatus btnSubmit)
        {
            JsonResponse jsonResponse = new JsonResponse();
            try
            {
                jsonResponse = _OrderReturnBLL.UpdateOrderReturn(model, btnSubmit, Url);
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

        public JsonResult UpdateQuantity(int Product, int Quantity)
        {
            JsonResponse jsonResponse = new JsonResponse();
            var list = SessionHelper.AddReturnProduct;
            list.First(e => e.ProductId == Product).Quantity = Quantity;
            SessionHelper.AddReturnProduct = list;
            return Json(new { data = jsonResponse });
        }
        public JsonResult AddProduct(OrderReturnDetail model)
        {
            JsonResponse jsonResponse = new JsonResponse();
            try
            {
                if (!SessionHelper.AddReturnProduct.Any(e => e.ProductId == model.ProductId))
                {
                    ProductMaster productMaster = _ProductMasterBLL.GetProductMasterById(model.ProductId);
                    ProductDetail productDetail = _ProductDetailBLL.Where(e => e.ProductMasterId == productMaster.Id).FirstOrDefault();
                    if (productMaster != null)
                    {
                        var list = SessionHelper.AddReturnProduct;
                        model.PlantLocationId = productDetail.PlantLocationId;
                        model.PlantLocation = productDetail.PlantLocation;
                        model.ProductMaster = productMaster;
                        model.PlantLocation = new PlantLocationBLL(_unitOfWork).GetAllPlantLocation().Where(x => x.Id == model.PlantLocationId).FirstOrDefault();
                        model.OrderReturnNumber = list.Count == 0 ? 1 : list.Max(e => e.OrderReturnNumber) + 1;
                        model.NetAmount = model.Quantity * model.MRP;
                        model.Company = productDetail.Company;
                        list.Add(model);
                        SessionHelper.AddReturnProduct = list;
                        jsonResponse.Status = true;
                        jsonResponse.Message = "Product Added Successfully";
                        jsonResponse.RedirectURL = string.Empty;
                        jsonResponse.HtmlString = RenderRazorViewToString("ProductGrid", SessionHelper.AddReturnProduct.OrderByDescending(e => e.OrderReturnNumber).ToList());
                    }
                }
                else
                {
                    jsonResponse.Status = false;
                    jsonResponse.Message = "Product Already Exists";
                    jsonResponse.RedirectURL = string.Empty;
                    jsonResponse.HtmlString = RenderRazorViewToString("ProductGrid", SessionHelper.AddReturnProduct.OrderByDescending(e => e.OrderReturnNumber).ToList());
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
        public IActionResult Approve(string DPID)
        {
            int id = 0;
            int.TryParse(EncryptDecrypt.Decrypt(DPID), out id);
            SessionHelper.AddReturnProduct = new List<OrderReturnDetail>();
            return View("Approve", BindOrderReturnMaster(id, true));
        }

        public void SelectProduct(string DPID)
        {
            int id = 0;
            int.TryParse(EncryptDecrypt.Decrypt(DPID), out id);
            var list = SessionHelper.AddReturnProduct;
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
            SessionHelper.AddReturnProduct = list;
        }

        [HttpPost]
        public JsonResult ApprovedQuantity(OrderReturnMaster model)
        {
            JsonResponse jsonResponse = new JsonResponse();
            try
            {
                _unitOfWork.Begin();
                //var ProductIds = model.OrderReturnDetail.Where(e => e.OrderReturnId == model.Id).Select(e => e.ProductId).ToArray();
                var ProductIds = model.OrderReturnDetail.Select(e => e.ProductId).ToList();
                var OrderreturnProduct = _OrderReturnDetailBLL.Where(e => e.OrderReturnId == model.Id && ProductIds.Contains(e.ProductId)).ToList();
                var master = _OrderReturnBLL.FirstOrDefault(e => e.Id == model.Id);
                if (master != null)
                {

                }
                foreach (var item in model.OrderReturnDetail.Where(x => x.IsProductSelected == true))
                {
                    var product = OrderreturnProduct.First(e => e.ProductId == item.ProductId);
                    product.IsProductSelected = true;
                    product.ReceivedQty = item.ReceivedQty;
                    product.ReceivedBy = SessionHelper.LoginUser.Id;
                    product.ReceivedDate = DateTime.Now;
                    _OrderReturnDetailBLL.Update(product);
                }
                var Client = new RestClient(_Configuration.PostReturnOrder);
                var request = new RestRequest(Method.POST).AddJsonBody(_OrderReturnBLL.PlaceReturnOrderToSAP(model.Id), "json");
                IRestResponse response = Client.Execute(request);
                var SAPProduct = JsonConvert.DeserializeObject<List<SAPOrderStatus>>(response.Content);
                if (SAPProduct != null)
                {
                    foreach (var item in SAPProduct)
                    {
                        var product = OrderreturnProduct.FirstOrDefault(e => e.ProductMaster.SAPProductCode == item.ProductCode);
                        if (product != null)
                        {
                            product.ReturnOrderNumber = item.SAPOrderNo;
                            product.ReturnOrderStatus = OrderStatus.NotYetProcess;
                            product.ReceivedBy = SessionHelper.LoginUser.Id;
                            product.ReceivedDate = DateTime.Now;
                            _OrderReturnDetailBLL.Update(product);
                        }
                    }
                }
                if (_OrderReturnDetailBLL.Where(e => e.ReturnOrderNumber == null && e.OrderReturnId == model.Id).ToList().Count > 0)
                {
                    master.Status = OrderReturnStatus.PartiallyReceived;
                }
                else
                {
                    master.Status = OrderReturnStatus.Received;
                }
                _OrderReturnBLL.Update(master);
                _unitOfWork.Commit();
                jsonResponse.Status = true;
                jsonResponse.Message = NotificationMessage.ReceivedReturn;
                jsonResponse.RedirectURL = Url.Action("Index", "OrderReturn");
                return Json(new { data = jsonResponse });
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                jsonResponse.Status = false;
                jsonResponse.Message = NotificationMessage.FailedReturn;
                return Json(new { data = jsonResponse });
            }
        }
        [HttpPost]
        public IActionResult Search(OrderReturnViewModel model, string Search)
        {
            if (!string.IsNullOrEmpty(Search))
            {
                model = List(model);
            }
            else
            {
                model.OrderReturnMaster = GetOrderReturnList();
            }
            return PartialView("List", model.OrderReturnMaster);
        }
        public List<OrderReturnMaster> GetOrderReturnList()
        {
            List<OrderReturnMaster> list = new List<OrderReturnMaster>();
            if (SessionHelper.LoginUser.IsStoreKeeper)
            {
                List<int> ProductMasterIds = _ProductDetailBLL.GetAllProductDetail().Select(x => x.ProductMasterId).Distinct().ToList();
                list = _OrderReturnDetailBLL.GetAllOrderReturnDetail().Where(x => x.PlantLocationId == SessionHelper.LoginUser.PlantLocationId && ProductMasterIds.Contains(x.ProductId)).DistinctBy(e => e.OrderReturnId).Select(x => new OrderReturnMaster
                {
                    Id = x.OrderReturnMaster.Id,
                    Distributor = x.OrderReturnMaster.Distributor,
                    Status = x.OrderReturnMaster.Status,
                    CreatedBy = x.OrderReturnMaster.CreatedBy,
                    CreatedDate = x.OrderReturnMaster.CreatedDate
                }).OrderByDescending(x => x.Id).ToList();
            }
            else
            {
                list = _OrderReturnBLL.GetAllOrderReturn().Where(x => SessionHelper.LoginUser.IsDistributor == true ? x.DistributorId == SessionHelper.LoginUser.DistributorId : true).OrderByDescending(x => x.Id).ToList();
            }
            return list;
        }
        private OrderReturnMaster BindOrderReturnMaster(int Id, bool forApprove = false)
        {
            OrderReturnMaster model = new OrderReturnMaster();
            if (Id > 0)
            {
                model = _OrderReturnBLL.GetById(Id);
                model.Distributor = model.Distributor;
                model.OrderReturnDetail = _OrderReturnDetailBLL.Where(e => e.OrderReturnId == Id && (forApprove ? string.IsNullOrEmpty(e.ReturnOrderNumber) : true)).ToList();
                var allproducts = _ProductMasterBLL.GetAllProductMaster();
                List<ProductDetail> allproductDetail;
                if (SessionHelper.LoginUser.IsStoreKeeper)
                {
                    allproductDetail = _ProductDetailBLL.Where(e => e.PlantLocationId == SessionHelper.LoginUser.PlantLocationId).ToList();
                }
                else
                {
                    allproductDetail = _ProductDetailBLL.GetAllProductDetail();
                }
                OrderReturnDetail detail = new OrderReturnDetail();
                foreach (var item in model.OrderReturnDetail)
                {
                    ProductMaster productMaster = allproducts.FirstOrDefault(e => e.Id == item.ProductId);
                    ProductDetail productDetail = allproductDetail.FirstOrDefault(e => e.ProductMasterId == item.ProductId);
                    if (productMaster != null && productDetail != null)
                    {
                        var list = SessionHelper.AddReturnProduct;
                        detail.BatchNo = item.BatchNo;
                        detail.MRP = item.MRP;
                        detail.TotalPrice = item.TotalPrice;
                        detail.Discount = item.Discount;
                        detail.Quantity = item.Quantity;
                        detail.ReceivedQty = item.ReceivedQty;
                        detail.Discount = item.Discount;
                        detail.ManufactureDate = item.ManufactureDate;
                        detail.ExpiryDate = item.ExpiryDate;
                        detail.InvoiceNo = item.InvoiceNo;
                        detail.InvoiceDate = item.InvoiceDate;
                        detail.IntimationDate = item.IntimationDate;
                        detail.ProductMaster = productMaster;
                        detail.PlantLocation = item.PlantLocation;
                        detail.OrderReturnNumber = list.Count == 0 ? 1 : list.Max(e => e.OrderReturnNumber) + 1;
                        detail.NetAmount = detail.Quantity * detail.MRP;
                        detail.Remarks = item.Remarks;
                        detail.ProductId = item.ProductId;
                        detail.OrderReturnId = item.OrderReturnId;
                        detail.PlantLocationId = item.PlantLocationId;
                        detail.Company = productDetail.Company;
                        detail.ReturnOrderNumber = item.ReturnOrderNumber;
                        detail.ReturnOrderStatus = item.ReturnOrderStatus;
                        list.Add(detail);
                        SessionHelper.AddReturnProduct = list;
                    }
                }
                model.OrderReturnDetail = SessionHelper.AddReturnProduct;
                model.ProductList = _ProductDetailBLL.DropDownProductList();
                model.returnReasonList = _orderReturnReasonBLL.DropDownOrderReturnReasonList();
            }
            else
            {
                model.ProductList = _ProductDetailBLL.DropDownProductList();
                model.returnReasonList = _orderReturnReasonBLL.DropDownOrderReturnReasonList();
                model.OrderReturnDetail = new List<OrderReturnDetail>();
                model.Distributor = SessionHelper.LoginUser.Distributor;
            }
            return model;
        }
        public async Task<string> RenderRazorViewToString(string viewName, object model)
        {
            ViewData.Model = model;
            using (var sw = new StringWriter())
            {
                var viewResult = _viewEngine.FindView(ControllerContext, viewName, false);
                var viewContext = new ViewContext(ControllerContext, viewResult.View, ViewData, TempData, sw, new HtmlHelperOptions());
                await viewResult.View.RenderAsync(viewContext);
                return sw.GetStringBuilder().ToString();
            }
        }
        public IActionResult Delete(string DPID)
        {
            int id = 0;
            int.TryParse(EncryptDecrypt.Decrypt(DPID), out id);
            var list = SessionHelper.AddReturnProduct;
            var item = list.FirstOrDefault(e => e.ProductId == id);
            if (item != null)
            {
                list.Remove(item);
            }
            SessionHelper.AddReturnProduct = list;
            return PartialView("ProductGrid", SessionHelper.AddReturnProduct.OrderByDescending(e => e.OrderReturnNumber));
        }
        public JsonResult AddApproveReturnProduct(OrderReturnDetail model, int OrderId)
        {
            JsonResponse response = new JsonResponse();
            try
            {
                var detail = AddProductonExistingList(OrderId, model);
                response.Status = true;
                response.Message = "Product Added Successfully";
                response.RedirectURL = string.Empty;
                response.HtmlString = RenderRazorViewToString("ApproveGrid", detail);
                return Json(new { data = response });
            }
            catch (Exception ex)
            {
                response.Message = "Unable to add Product";
                response.Status = false;
                return Json(new { data = response });
            }
        }

        private OrderReturnMaster AddProductonExistingList(int Id, OrderReturnDetail model)
        {
            OrderReturnDetail detail = new OrderReturnDetail();
            ProductMaster productMaster = _ProductMasterBLL.FirstOrDefault(e => e.Id == model.ProductId);
            ProductDetail productDetail = _ProductDetailBLL.FirstOrDefault(e => e.ProductMasterId == model.ProductId);
            var list = SessionHelper.AddReturnProduct;
            detail.BatchNo = model.BatchNo;
            detail.MRP = model.MRP;
            detail.TotalPrice = model.TotalPrice;
            detail.Discount = model.Discount;
            detail.Quantity = model.Quantity;
            detail.ReceivedQty = model.ReceivedQty;
            detail.Discount = model.Discount;
            detail.ManufactureDate = model.ManufactureDate;
            detail.ExpiryDate = model.ExpiryDate;
            detail.InvoiceNo = model.InvoiceNo;
            detail.InvoiceDate = model.InvoiceDate;
            detail.ProductMaster = productMaster;
            detail.PlantLocation = model.PlantLocation;
            detail.OrderReturnNumber = list.Count == 0 ? 1 : list.Max(e => e.OrderReturnNumber) + 1;
            detail.NetAmount = detail.Quantity * detail.MRP;
            detail.Remarks = model.Remarks;
            detail.ProductId = model.ProductId;
            detail.OrderReturnId = model.OrderReturnId;
            detail.PlantLocationId = productDetail.PlantLocationId;
            detail.PlantLocation = productDetail.PlantLocation;
            detail.Company = productDetail.Company;
            detail.ReturnOrderNumber = model.ReturnOrderNumber;
            detail.ReturnOrderStatus = model.ReturnOrderStatus;
            list.Add(detail);
            SessionHelper.AddReturnProduct = list;
            OrderReturnMaster order = _OrderReturnBLL.GetById(Id);
            order.OrderReturnDetail = SessionHelper.AddReturnProduct;
            return order;
        }
        public JsonResult GetTRNoById(string DPID)
        {
            int id = 0;
            int.TryParse(EncryptDecrypt.Decrypt(DPID), out id);
            var TRNoPlantLocation = _OrderReturnDetailBLL.Where(x => x.OrderReturnId == id).Select(x => new { TRNo = x.TRNo, PlantLocationId = x.PlantLocationId }).Distinct().ToArray();
            return Json(new { data = TRNoPlantLocation });
        }
    }
}

