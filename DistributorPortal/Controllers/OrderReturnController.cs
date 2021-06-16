using BusinessLogicLayer.Application;
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
using MoreLinq;
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
using static Utility.Constant.Common;

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
        private readonly DistributorWiseProductDiscountAndPricesBLL _DistributorWiseProductDiscountAndPricesBLL;
        private readonly ICompositeViewEngine _viewEngine;
        private readonly Configuration _Configuration;
        private readonly IConfiguration _IConfiguration;
        private readonly NotificationBLL _NotificationBLL;
        private readonly EmailLogBLL _EmailLogBLL;
        private readonly IWebHostEnvironment _env;
        private readonly UserBLL _UserBLL;
        public OrderReturnController(IUnitOfWork unitOfWork, ICompositeViewEngine viewEngine, Configuration configuration, IConfiguration iConfiguration, IWebHostEnvironment env)
        {
            _unitOfWork = unitOfWork;
            _env = env;
            _OrderReturnBLL = new OrderReturnBLL(_unitOfWork);
            _OrderReturnDetailBLL = new OrderReturnDetailBLL(_unitOfWork);
            _ProductMasterBLL = new ProductMasterBLL(_unitOfWork);
            _ProductDetailBLL = new ProductDetailBLL(_unitOfWork);
            _orderReturnReasonBLL = new OrderReturnReasonBLL(_unitOfWork);
            _DistributorWiseProductDiscountAndPricesBLL = new DistributorWiseProductDiscountAndPricesBLL(_unitOfWork);
            _NotificationBLL = new NotificationBLL(_unitOfWork);
            _viewEngine = viewEngine;
            _Configuration = configuration;
            _IConfiguration = iConfiguration;
            _EmailLogBLL = new EmailLogBLL(_unitOfWork, _Configuration);
            _UserBLL = new UserBLL(_unitOfWork);
        }
        public IActionResult Index()
        {
            OrderReturnViewModel model = new OrderReturnViewModel();
            model.OrderReturnMaster = GetOrderReturnList();
            if (SessionHelper.LoginUser.IsDistributor)
            {
                model.OrderReturnMaster = _OrderReturnBLL.Search(model).Where(x => SessionHelper.LoginUser.IsDistributor == true ? x.DistributorId == SessionHelper.LoginUser.DistributorId : true).ToList();
            }
            else
            {
                List<int> ProductMasterIds = _ProductDetailBLL.Where(x => x.PlantLocationId == SessionHelper.LoginUser.PlantLocationId).Select(x => x.ProductMasterId).Distinct().ToList();
                List<int> OrderReturnIds = _OrderReturnDetailBLL.Where(x => ProductMasterIds.Contains(x.ProductMaster.Id)).Select(x => x.OrderReturnId).ToList();
                model.OrderReturnMaster = _OrderReturnBLL.Search(model).Where(x => x.IsDeleted == false && OrderReturnIds.Contains(x.Id) && x.Status != OrderReturnStatus.Draft).ToList();
            }
            model.DistributorList = new DistributorBLL(_unitOfWork).DropDownDistributorList(null);
            return View(model);
        }
        public OrderReturnViewModel List(OrderReturnViewModel model)
        {
            if (model.DistributorId is null && model.Status is null && model.FromDate is null && model.ToDate is null && model.OrderReturnNo is null && model.TRNo is null)
            {
                model.OrderReturnMaster = GetOrderReturnList();
            }
            else
            {
                if (SessionHelper.LoginUser.IsDistributor)
                {
                    model.OrderReturnMaster = _OrderReturnBLL.Search(model).Where(x => SessionHelper.LoginUser.IsDistributor == true ? x.DistributorId == SessionHelper.LoginUser.DistributorId : true).ToList();
                }
                else
                {
                    List<int> ProductMasterIds = _ProductDetailBLL.Where(x => x.PlantLocationId == SessionHelper.LoginUser.PlantLocationId).Select(x => x.ProductMasterId).Distinct().ToList();
                    List<int> OrderReturnIds = _OrderReturnDetailBLL.Where(x => ProductMasterIds.Contains(x.ProductMaster.Id)).Select(x => x.OrderReturnId).ToList();
                    model.OrderReturnMaster = _OrderReturnBLL.Search(model).Where(x => OrderReturnIds.Contains(x.Id)).ToList();
                }
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
        public IActionResult View(string DPID, string RedirectURL)
        {
            TempData["RedirectURL"] = RedirectURL;
            int.TryParse(EncryptDecrypt.Decrypt(DPID), out int id);
            SessionHelper.AddReturnProduct = new List<OrderReturnDetail>();
            return View("View", BindOrderReturnMaster(id));
        }
        [HttpPost]
        public JsonResult SaveEdit(OrderReturnMaster model, OrderReturnStatus btnSubmit)
        {
            JsonResponse jsonResponse = new JsonResponse();
            try
            {
                string[] permittedExtensions = Common.permittedExtensions;
                string FolderPath = _IConfiguration.GetSection("Settings").GetSection("FolderPath").Value;
                if (model.AttachmentFormFile != null)
                {
                    var ext = Path.GetExtension(model.AttachmentFormFile.FileName).ToLowerInvariant();
                    if (string.IsNullOrEmpty(ext) || !permittedExtensions.Contains(ext))
                    {
                        return Json(new { Result = false, Message = NotificationMessage.FileTypeAllowed });
                    }
                    if (model.AttachmentFormFile.Length >= Convert.ToInt64(_Configuration.FileSize))
                    {
                        return Json(new { Result = false, Message = NotificationMessage.FileSizeAllowed });
                    }

                    Tuple<bool, string> tuple = FileUtility.UploadFile(model.AttachmentFormFile, FolderName.OrderReturn, FolderPath);
                    if (tuple.Item1)
                    {
                        model.Attachment = tuple.Item2;
                    }
                }
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
                if (SessionHelper.AddReturnProduct is null || SessionHelper.AddReturnProduct.Count() == 0)
                {
                    SessionHelper.AddReturnProduct = new List<OrderReturnDetail>();
                }
                if (model.Quantity == 0)
                {
                    jsonResponse.Status = false;
                    jsonResponse.Message = "Product quantity cannot be 0";
                    jsonResponse.RedirectURL = string.Empty;
                    jsonResponse.HtmlString = RenderRazorViewToString("ProductGrid", SessionHelper.AddReturnProduct.OrderByDescending(e => e.OrderReturnNumber).ToList());
                    return Json(new { data = jsonResponse });
                }
                if (!SessionHelper.AddReturnProduct.Any(e => e.ProductId == model.ProductId && e.BatchNo == model.BatchNo))
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
                        jsonResponse.Message = "Product added successfully";
                        jsonResponse.RedirectURL = string.Empty;
                        jsonResponse.HtmlString = RenderRazorViewToString("ProductGrid", SessionHelper.AddReturnProduct.OrderByDescending(e => e.OrderReturnNumber).ToList());
                    }
                }
                else
                {
                    jsonResponse.Status = false;
                    jsonResponse.Message = "Product already exists";
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
            int.TryParse(EncryptDecrypt.Decrypt(DPID), out int id);
            SessionHelper.AddReturnProduct = new List<OrderReturnDetail>();
            return View("Approve", BindOrderReturnMaster(id, true));
        }
        public void SelectProduct(string DPID)
        {
            var list = SessionHelper.AddReturnProduct;
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
            SessionHelper.AddReturnProduct = list;
        }
        public void SelectAllProduct(bool IsSelected)
        {
            var list = SessionHelper.AddReturnProduct;
            if (IsSelected)
            {
                list.ForEach(x => x.IsProductSelected = true);
            }
            else
            {
                list.ForEach(x => x.IsProductSelected = false);
            }
            SessionHelper.AddReturnProduct = list;
        }
        [HttpPost]
        public JsonResult ApprovedQuantity(OrderReturnMaster model)
        {
            JsonResponse jsonResponse = new JsonResponse();
            Notification notification = new Notification();
            try
            {
                if (SessionHelper.AddReturnProduct.Where(x => x.IsProductSelected == true).Count() == 0)
                {
                    jsonResponse.Status = false;
                    jsonResponse.Message = "Select at least one product";
                    return Json(new { data = jsonResponse });
                }
                var master = _OrderReturnBLL.FirstOrDefault(e => e.Id == model.Id);
                if (master != null && (master.Status == OrderReturnStatus.Received || master.Status == OrderReturnStatus.Rejected))
                {
                    jsonResponse.Status = false;
                    jsonResponse.Message = "Order return already " + model.Status;
                    jsonResponse.RedirectURL = Url.Action("Index", "OrderReturn");
                    return Json(new { data = jsonResponse });
                }
                _unitOfWork.Begin();
                var OrderreturnProduct = _OrderReturnDetailBLL.Where(e => e.OrderReturnId == model.Id && model.OrderReturnDetail.Select(e => e.ProductId).Contains(e.ProductId)).ToList();
                foreach (var item in model.OrderReturnDetail)
                {
                    OrderReturnDetail orderReturnDetail = new OrderReturnDetail();
                    if (OrderreturnProduct.FirstOrDefault(e => e.ProductId == item.ProductId && e.BatchNo.Trim() == item.BatchNo.Trim()) == null)
                    {
                        var detail = SessionHelper.AddReturnProduct.First(e => e.ProductId == item.ProductId && e.BatchNo.Trim() == item.BatchNo.Trim());
                        detail.IsProductSelected = SessionHelper.AddReturnProduct.FirstOrDefault(x => x.ProductId == item.ProductId).IsProductSelected;
                        detail.OrderReturnId = model.Id;
                        detail.CreatedBy = SessionHelper.LoginUser.Id;
                        detail.CreatedDate = DateTime.Now;
                        detail.ReceivedQty = item.ReceivedQty;
                        detail.ReceivedBy = SessionHelper.LoginUser.Id;
                        detail.ReceivedDate = DateTime.Now;
                        detail.ProductMaster = null;
                        detail.PlantLocation = null;
                        _OrderReturnDetailBLL.Add(detail);
                        _unitOfWork.Save();
                    }
                    else
                    {
                        var product = OrderreturnProduct.First(e => e.ProductId == item.ProductId && e.BatchNo.Trim() == item.BatchNo.Trim());
                        product.ReceivedQty = item.ReceivedQty;
                        product.IsProductSelected = SessionHelper.AddReturnProduct.FirstOrDefault(e => e.ProductId == item.ProductId && e.BatchNo.Trim() == item.BatchNo.Trim()).IsProductSelected;
                        product.ReceivedBy = SessionHelper.LoginUser.Id;
                        product.ReceivedDate = DateTime.Now;
                        _OrderReturnDetailBLL.Update(product);
                        _unitOfWork.Save();
                    }
                }
                var Client = new RestClient(_Configuration.PostReturnOrder);
                var request = new RestRequest(Method.POST).AddJsonBody(_OrderReturnBLL.PlaceReturnOrderToSAP(model.Id), "json");
                IRestResponse response = Client.Execute(request);
                var SAPProduct = JsonConvert.DeserializeObject<List<SAPOrderStatus>>(response.Content);
                if (SAPProduct != null || SAPProduct.Count() == 0)
                {
                    OrderreturnProduct = _OrderReturnDetailBLL.Where(e => e.OrderReturnId == model.Id && model.OrderReturnDetail.Select(e => e.ProductId).Contains(e.ProductId)).ToList();
                    foreach (var item in SAPProduct)
                    {
                        var productList = OrderreturnProduct.Where(e => e.ProductMaster.SAPProductCode == item.ProductCode).ToList();
                        if (productList != null)
                        {
                            foreach (var product in productList)
                            {
                                product.ReturnOrderNumber = item.SAPOrderNo;
                                product.ReturnOrderStatus = OrderStatus.InProcess;
                                product.ReceivedBy = SessionHelper.LoginUser.Id;
                                product.ReceivedDate = DateTime.Now;
                                _OrderReturnDetailBLL.Update(product);
                                _unitOfWork.Save();

                            }
                        }
                    }
                    var UpdatedOrderDetail = _OrderReturnDetailBLL.Where(e => e.OrderReturnId == model.Id && e.ReturnOrderNumber == null).ToList();
                    master.Status = UpdatedOrderDetail.Count > 0 ? OrderReturnStatus.PartiallyReceived : OrderReturnStatus.Received;
                    master.ReceivedBy = SessionHelper.LoginUser.Id;
                    master.ReceivedDate = DateTime.Now;
                    var result = _OrderReturnBLL.Update(master);
                    _unitOfWork.Commit();
                    jsonResponse.Status = result;
                    jsonResponse.Message = result ? NotificationMessage.ReceivedReturn : NotificationMessage.ErrorOccurred;
                    jsonResponse.SignalRResponse = new SignalRResponse() { UserId = master.CreatedBy.ToString(), Number = "Order Return #: " + master.SNo, Message = jsonResponse.Message, Status = Enum.GetName(typeof(OrderReturnStatus), model.Status) };
                    notification.DistributorId = master.DistributorId;
                    notification.RequestId = master.SNo;
                    notification.Status = master.Status.ToString();
                    notification.Message = jsonResponse.SignalRResponse.Message;
                    notification.URL = "/OrderReturn/View?DPID=" + EncryptDecrypt.Encrypt(master.Id.ToString());
                    _NotificationBLL.Add(notification);

                    //Sending Email
                    if (jsonResponse.Status)
                    {
                        string EmailTemplate = _env.WebRootPath + "\\Attachments\\EmailTemplates\\ReturnOrderFromCustomer.html";
                        ReturnOrderEmailUserModel EmailUserModel = new ReturnOrderEmailUserModel()
                        {
                            ToAcceptTemplate = System.IO.File.ReadAllText(EmailTemplate),
                            Date = Convert.ToDateTime(master.ReceivedDate).ToString("dd/MMM/yyyy"),
                            City = master.Distributor.City,
                            ShipToPartyName = master.Distributor.DistributorName,
                            RetrunOrderNumber = master.SNo.ToString(),
                            Subject = "Return Delivery",
                            CreatedBy = SessionHelper.LoginUser.Id,
                        };
                        int[] PlantLocationId = OrderreturnProduct.Select(x => x.PlantLocationId).Distinct().ToArray();
                        if (PlantLocationId.Count() > 0)
                        {
                            List<User> UserList = _UserBLL.Where(x => PlantLocationId.Contains((int)x.PlantLocationId)).ToList();
                            _EmailLogBLL.RetrunOrderEmail(UserList, EmailUserModel);
                        }
                    }
                }
                else
                {
                    jsonResponse.Status = false;
                    jsonResponse.Message = "Unable to received order";
                }
                jsonResponse.RedirectURL = Url.Action("Index", "OrderReturn");
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
                    TRNo = x.OrderReturnMaster.TRNo,
                    SNo = x.OrderReturnMaster.SNo,
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
                model.OrderReturnDetail = _OrderReturnDetailBLL.Where(e => e.OrderReturnId == Id && (forApprove ? string.IsNullOrEmpty(e.ReturnOrderNumber) : true)).ToList();
                var allproducts = _ProductMasterBLL.Where(x => model.OrderReturnDetail.Select(x => x.ProductId).Contains(x.Id));
                List<ProductDetail> allproductDetail = _ProductDetailBLL.GetAllProductDetail();
                List<DistributorWiseProductDiscountAndPrices> distributorWiseProductDiscountAndPrices = _DistributorWiseProductDiscountAndPricesBLL.Where(x => x.DistributorId == model.DistributorId);
                if (SessionHelper.LoginUser.IsStoreKeeper)
                {
                    allproductDetail = allproductDetail.Where(e => e.PlantLocationId == SessionHelper.LoginUser.PlantLocationId && model.OrderReturnDetail.Select(x => x.ProductId).Contains(e.ProductMasterId)).ToList();
                }
                else
                {
                    allproductDetail = allproductDetail.Where(e => model.OrderReturnDetail.Select(x => x.ProductId).Contains(e.ProductMasterId)).ToList();
                }
                foreach (var item in model.OrderReturnDetail)
                {
                    ProductMaster productMaster = allproducts.FirstOrDefault(e => e.Id == item.ProductId);
                    ProductDetail productDetail = allproductDetail.FirstOrDefault(e => e.ProductMasterId == item.ProductId);
                    OrderReturnDetail detail = new OrderReturnDetail();

                    if (productMaster != null && productDetail != null)
                    {
                        var list = SessionHelper.AddReturnProduct;
                        detail.Id = item.Id;
                        detail.BatchNo = item.BatchNo;
                        detail.MRP = item.MRP;
                        detail.TotalPrice = distributorWiseProductDiscountAndPrices.First(x => x.ProductDetailId == allproductDetail.First(x => x.ProductMasterId == item.ProductId).Id).ProductPrice;
                        detail.Discount = item.Discount;
                        detail.Quantity = item.Quantity;
                        detail.ReceivedQty = item.ReceivedQty;
                        detail.Discount = item.Discount;
                        detail.ManufactureDate = item.ManufactureDate;
                        detail.IntimationDate = item.IntimationDate;
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
                        detail.IsProductSelected = true;
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
        public IActionResult Delete(string DPID, string BatchNo)
        {
            int id = 0;
            int.TryParse(EncryptDecrypt.Decrypt(DPID), out id);
            var list = SessionHelper.AddReturnProduct;
            var item = list.FirstOrDefault(e => e.ProductId == id && e.BatchNo.Trim() == BatchNo.Trim());
            if (item != null)
            {
                list.Remove(item);
            }
            SessionHelper.AddReturnProduct = list;
            return PartialView("ProductGrid", SessionHelper.AddReturnProduct.OrderByDescending(e => e.OrderReturnNumber));
        }
        public JsonResult AddApproveReturnProduct(OrderReturnDetail model, int OrderId)
        {
            JsonResponse jsonResponse = new JsonResponse();
            try
            {
                if (!SessionHelper.AddReturnProduct.Any(e => e.ProductId == model.ProductId && e.BatchNo.Trim() == model.BatchNo.Trim()))
                {
                    var detail = AddProductonExistingList(OrderId, model);
                    jsonResponse.Status = true;
                    jsonResponse.Message = "Product added successfully";
                    jsonResponse.RedirectURL = string.Empty;
                    jsonResponse.HtmlString = RenderRazorViewToString("ApproveGrid", detail);
                }
                else
                {
                    jsonResponse.Status = false;
                    jsonResponse.Message = "Product already exists";
                    jsonResponse.RedirectURL = string.Empty;
                    jsonResponse.HtmlString = RenderRazorViewToString("ProductGrid", SessionHelper.AddReturnProduct.OrderByDescending(e => e.OrderReturnNumber).ToList());
                }
                return Json(new { data = jsonResponse });
            }
            catch (Exception ex)
            {
                new ErrorLogBLL(_unitOfWork).AddExceptionLog(ex);
                jsonResponse.Status = false;
                jsonResponse.Message = "Unable to add product";
                return Json(new { data = jsonResponse });
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
            detail.IntimationDate = model.IntimationDate;
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
            detail.IsProductSelected = true;
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
        public IActionResult Reject(string DPID, string Comments)
        {
            JsonResponse jsonResponse = new JsonResponse();
            Notification notification = new Notification();
            try
            {
                int.TryParse(EncryptDecrypt.Decrypt(DPID), out int id);
                var order = _OrderReturnBLL.GetById(id);
                OrderReturnMaster orderReturnMaster = _OrderReturnBLL.GetById(id);
                if (orderReturnMaster != null && (orderReturnMaster.Status == OrderReturnStatus.Received || orderReturnMaster.Status == OrderReturnStatus.Rejected))
                {
                    jsonResponse.Status = false;
                    jsonResponse.Message = "Order return already " + orderReturnMaster.Status;
                    jsonResponse.RedirectURL = Url.Action("Index", "OrderReturn");
                    return Json(new { data = jsonResponse });
                }
                order.Status = OrderReturnStatus.Rejected;
                order.RejectedComment = Comments;
                order.RejectedBy = SessionHelper.LoginUser.Id;
                order.RejectedDate = DateTime.Now;
                var result = _OrderReturnBLL.Update(order);
                if (result)
                {
                    jsonResponse.Status = true;
                    jsonResponse.Message = "Order return rejected successfully.";
                    jsonResponse.RedirectURL = Url.Action("Index", "OrderReturn");
                    jsonResponse.SignalRResponse = new SignalRResponse() { UserId = order.CreatedBy.ToString(), Number = "Order Return #: " + order.SNo, Message = "Order has been rejected by Admin", Status = Enum.GetName(typeof(OrderStatus), order.Status) };
                    notification.DistributorId = order.DistributorId;
                    notification.RequestId = order.SNo;
                    notification.Status = order.Status.ToString();
                    notification.Message = jsonResponse.SignalRResponse.Message;
                    notification.URL = "/OrderReturn/View?DPID=" + EncryptDecrypt.Encrypt(id.ToString());
                    _NotificationBLL.Add(notification);
                }
                else
                {
                    jsonResponse.Status = false;
                    jsonResponse.Message = NotificationMessage.ErrorOccurred;
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
        [HttpPost]
        public IActionResult SyncPartiallyApproved(string DPID)
        {
            JsonResponse jsonResponse = new JsonResponse();
            Notification notification = new Notification();
            try
            {
                int.TryParse(EncryptDecrypt.Decrypt(DPID), out int id);
                var order = _OrderReturnBLL.GetById(id);
                var client = new RestClient(_Configuration.PostOrder);
                var request = new RestRequest(Method.POST).AddJsonBody(_OrderReturnBLL.PlaceReturnOrderPartiallyApprovedToSAP(id), "json");
                IRestResponse response = client.Execute(request);
                var sapProduct = JsonConvert.DeserializeObject<List<SAPOrderStatus>>(response.Content);
                var updatedOrderDetail = _OrderReturnDetailBLL.Where(e => e.OrderReturnId == id && e.ReturnOrderNumber == null).ToList();
                if (sapProduct != null)
                {
                    foreach (var item in sapProduct)
                    {
                        var product = updatedOrderDetail.FirstOrDefault(e => e.ProductMaster.SAPProductCode == item.ProductCode);
                        if (product != null)
                        {
                            if (!string.IsNullOrEmpty(item.SAPOrderNo))
                            {
                                product.ReturnOrderNumber = item.SAPOrderNo;
                                product.ReturnOrderStatus = OrderStatus.InProcess;
                                _OrderReturnDetailBLL.Update(product);
                            }
                        }
                    }
                    updatedOrderDetail = _OrderReturnDetailBLL.Where(e => e.OrderReturnId == id && e.ReturnOrderNumber == null).ToList();
                    order.Status = updatedOrderDetail.Count > 0 ? OrderReturnStatus.PartiallyReceived : OrderReturnStatus.Received;
                    order.ReceivedBy = SessionHelper.LoginUser.Id;
                    order.ReceivedDate = DateTime.Now;
                    var result = _OrderReturnBLL.Update(order);
                    jsonResponse.Status = result;
                    jsonResponse.Message = result ? "Order return has been approved successfully" : NotificationMessage.ErrorOccurred;
                    jsonResponse.RedirectURL = Url.Action("Index", "OrderReturn");
                    jsonResponse.SignalRResponse = new SignalRResponse() { UserId = order.CreatedBy.ToString(), Number = "Order return #: " + order.SNo, Message = "Order return has been approved", Status = Enum.GetName(typeof(OrderStatus), order.Status) };
                    notification.DistributorId = order.DistributorId;
                    notification.RequestId = order.SNo;
                    notification.Status = order.Status.ToString();
                    notification.Message = jsonResponse.SignalRResponse.Message;
                    notification.URL = "/OrderReturn/View?DPID=" + EncryptDecrypt.Encrypt(id.ToString());
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

