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
            model.OrderReturnMaster = GetOrderReturnList().Where(x => x.Status == OrderReturnStatus.Submitted || x.Status == OrderReturnStatus.PartiallyReceived).ToList();
            if (SessionHelper.LoginUser.IsDistributor)
            {
                model.OrderReturnMaster = model.OrderReturnMaster.Where(x => SessionHelper.LoginUser.IsDistributor == true ? x.DistributorId == SessionHelper.LoginUser.DistributorId : true).ToList();
            }
            else
            {
                List<int> ProductMasterIds = _ProductDetailBLL.Where(x => x.PlantLocationId == SessionHelper.LoginUser.PlantLocationId).Select(x => x.ProductMasterId).Distinct().ToList();
                List<int> OrderReturnIds = _OrderReturnDetailBLL.Where(x => ProductMasterIds.Contains(x.ProductMaster.Id)).Select(x => x.OrderReturnId).ToList();
                model.OrderReturnMaster = model.OrderReturnMaster.Where(x => x.IsDeleted == false && OrderReturnIds.Contains(x.Id) && SessionHelper.LoginUser.IsDistributor ? true : x.Status != OrderReturnStatus.Draft).ToList();
            }
            model.DistributorList = new DistributorBLL(_unitOfWork).DropDownDistributorList(null);

            if (SessionHelper.DistributorWiseProductOrderReturn is null || SessionHelper.DistributorWiseProductOrderReturn.Count() == 0)
            {
                SessionHelper.DistributorWiseProductOrderReturn = new List<DistributorWiseProductDiscountAndPrices>();
            }
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
                    model.OrderReturnMaster = _OrderReturnBLL.Search(model).Where(x => OrderReturnIds.Contains(x.Id) && SessionHelper.LoginUser.IsDistributor ? true : x.Status != OrderReturnStatus.Draft).ToList();
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
                        model.Attachment = tuple.Item2;
                    }
                }
                if (SessionHelper.AddReturnProduct.Where(x => x.Quantity == 0).Count() > 0)
                {
                    jsonResponse.Status = false;
                    jsonResponse.Message = "Add quanity in products";
                    return Json(new { data = jsonResponse });
                }
                if (SessionHelper.AddReturnProduct.Where(x => string.IsNullOrEmpty(x.BatchNo)).Count() > 0)
                {
                    jsonResponse.Status = false;
                    jsonResponse.Message = "Add batch no in products";
                    return Json(new { data = jsonResponse });
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
        public JsonResult UpdateBatch(int ProductId, string BatchNo, string NewBatchNo)
        {
            JsonResponse jsonResponse = new JsonResponse();
            var list = SessionHelper.AddReturnProduct;
            if (!string.IsNullOrEmpty(BatchNo) && !string.IsNullOrEmpty(NewBatchNo))
            {
                list.FirstOrDefault(x => x.ProductId == ProductId && x.BatchNo == BatchNo).BatchNo = NewBatchNo;
            }
            else
            {
                list.FirstOrDefault(x => x.ProductId == ProductId).BatchNo = NewBatchNo;
            }
            SessionHelper.AddReturnProduct = list;
            return Json(new { NetAmount = Math.Round((decimal)list.First(e => e.ProductId == ProductId).NetAmount, 2).ToString() });
        }
        public JsonResult UpdateQuantity(int ProductId, int Quantity, string BatchNo)
        {
            JsonResponse jsonResponse = new JsonResponse();
            var list = SessionHelper.AddReturnProduct;
            list.First(x => x.ProductId == ProductId && x.BatchNo == BatchNo).Quantity = Quantity;
            list.First(x => x.ProductId == ProductId).NetAmount = Convert.ToDouble((list.First(e => e.ProductId == ProductId).TradePrice - (-1 * list.First(e => e.ProductId == ProductId).TradePrice / 100) * list.First(e => e.ProductId == ProductId).Discount) * Quantity);
            SessionHelper.AddReturnProduct = list;
            return Json(new { NetAmount = Math.Round((decimal)list.First(e => e.ProductId == ProductId).NetAmount, 2).ToString() });
        }
        public JsonResult BQMRP(int Id, int ProductId, int ReceivedQty, double ReceivedMRP, string BatchNo, string ReceivedBatchNo)
        {
            JsonResponse jsonResponse = new JsonResponse();
            var list = SessionHelper.AddReturnProduct;
            if (Id > 0)
            {
                list.First(e => e.Id == Id && e.BatchNo == BatchNo).ReceivedQty = ReceivedQty;
                list.First(e => e.Id == Id && e.BatchNo == BatchNo).ReceivedMRP = ReceivedMRP;
                list.First(e => e.Id == Id && e.BatchNo == BatchNo).ReceivedBatchNo = ReceivedBatchNo;

            }
            else
            {
                list.First(e => e.ProductMaster.Id == ProductId && e.BatchNo == BatchNo).ReceivedQty = ReceivedQty;
                list.First(e => e.ProductMaster.Id == ProductId && e.BatchNo == BatchNo).ReceivedMRP = ReceivedMRP;
                list.First(e => e.ProductMaster.Id == ProductId && e.BatchNo == BatchNo).ReceivedBatchNo = ReceivedBatchNo;
            }
            SessionHelper.AddReturnProduct = list;
            return Json(new { data = jsonResponse });
        }
        public JsonResult AddProduct(OrderReturnDetail model)
        {
            JsonResponse jsonResponse = new JsonResponse();
            List<DistributorWiseProductDiscountAndPrices> distributorWiseProductDiscountAndPrices = new List<DistributorWiseProductDiscountAndPrices>();
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
                    if (SessionHelper.DistributorWiseProductOrderReturn == null || SessionHelper.DistributorWiseProductOrderReturn.Count() == 0)
                    {
                        distributorWiseProductDiscountAndPrices = SessionHelper.DistributorWiseProductOrderReturn = _DistributorWiseProductDiscountAndPricesBLL.Where(e => (!SessionHelper.LoginUser.IsDistributor || e.DistributorId == SessionHelper.LoginUser.DistributorId) && e.ProductDetailId != null && (e.ProductDetail.ProductVisibilityId == ProductVisibility.Visible || e.ProductDetail.ProductVisibilityId == ProductVisibility.OrderReturn)).ToList();
                    }
                    else
                    {
                        distributorWiseProductDiscountAndPrices = SessionHelper.DistributorWiseProductOrderReturn;
                    }
                    if (productMaster != null)
                    {
                        var list = SessionHelper.AddReturnProduct;
                        model.Company = productDetail.Company;
                        model.PlantLocationId = productDetail.PlantLocationId;
                        model.PlantLocation = productDetail.PlantLocation;
                        model.ProductMaster = productMaster;
                        //model.PlantLocation = new PlantLocationBLL(_unitOfWork).GetAllPlantLocation().Where(x => x.Id == model.PlantLocationId).FirstOrDefault();
                        model.OrderReturnNumber = list.Count == 0 ? 1 : list.Max(e => e.OrderReturnNumber) + 1;
                        model.TradePrice = model.MRP - (model.MRP / 100) * distributorWiseProductDiscountAndPrices.First(x => x.ProductDetailId == productDetail.Id).ReturnMRPDicount;
                        model.NetAmount = Math.Round(Convert.ToDouble((model.TradePrice - (-1 * model.TradePrice / 100) * distributorWiseProductDiscountAndPrices.First(x => x.ProductDetailId == productDetail.Id).Discount) * model.Quantity), 2);
                        model.TotalPrice = distributorWiseProductDiscountAndPrices.First(x => x.ProductDetailId == productDetail.Id).ProductPrice;
                        model.Discount = distributorWiseProductDiscountAndPrices.First(x => x.ProductDetailId == productDetail.Id).Discount;
                        model.Company = productDetail.Company;
                        model.IsFOCProduct = false;
                        model.ParentDistributor = !string.IsNullOrEmpty(productDetail.ParentDistributor) ? productDetail.ParentDistributor : SessionHelper.LoginUser.Distributor.DistributorSAPCode;
                        model.R_OrderType = productDetail.R_OrderType;
                        model.SaleOrganization = productDetail.SaleOrganization;
                        model.DistributionChannel = productDetail.DistributionChannel;
                        model.Division = productDetail.Division;
                        model.DispatchPlant = productDetail.DispatchPlant;
                        model.R_StorageLocation = productDetail.R_StorageLocation;
                        model.ReturnItemCategory = productDetail.ReturnItemCategory;
                        list.Add(model);
                        if (!string.IsNullOrEmpty(productDetail.FOCProductCode))
                        {
                            var focProductMaster = _ProductMasterBLL.Where(e => e.SAPProductCode == productDetail.FOCProductCode).FirstOrDefault();
                            if (focProductMaster != null)
                            {
                                var focProductDetail = _ProductDetailBLL.Where(e => e.ProductMasterId == focProductMaster.Id).FirstOrDefault();
                                OrderReturnDetail orderReturnDetail = new OrderReturnDetail();
                                orderReturnDetail.Company = model.Company;
                                orderReturnDetail.PlantLocationId = model.PlantLocationId;
                                orderReturnDetail.PlantLocation = model.PlantLocation;
                                orderReturnDetail.ProductMaster = _ProductMasterBLL.GetProductMasterById(focProductDetail.ProductMasterId);
                                orderReturnDetail.PlantLocation = productDetail.PlantLocation;
                                orderReturnDetail.OrderReturnNumber = list.Count == 0 ? 1 : list.Max(e => e.OrderReturnNumber) + 1;
                                orderReturnDetail.NetAmount = 0;
                                orderReturnDetail.IsFOCProduct = true;
                                orderReturnDetail.Quantity = productDetail.FOCQuantityRatio == null ? model.Quantity : (int)productDetail.FOCQuantityRatio * model.Quantity;
                                orderReturnDetail.BatchNo = string.Empty;
                                orderReturnDetail.InvoiceNo = model.InvoiceNo;
                                orderReturnDetail.InvoiceDate = model.InvoiceDate;
                                orderReturnDetail.ProductId = focProductDetail.ProductMasterId;
                                orderReturnDetail.ParentDistributor = model.ParentDistributor;
                                orderReturnDetail.R_OrderType = model.R_OrderType;
                                orderReturnDetail.SaleOrganization = model.SaleOrganization;
                                orderReturnDetail.DistributionChannel = model.DistributionChannel;
                                orderReturnDetail.Division = model.Division;
                                orderReturnDetail.DispatchPlant = model.DispatchPlant;
                                orderReturnDetail.R_StorageLocation = model.R_StorageLocation;
                                orderReturnDetail.ReturnItemCategory = productDetail.ReturnItemCategory;
                                list.Add(orderReturnDetail);
                            }
                        }
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
                bool SAPOrderStatus = false;
                bool POrderStatus = false;
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
                List<ProductDetail> productDetails = _ProductDetailBLL.GetAllProductDetail();
                var OrderReturnDetails = _OrderReturnDetailBLL.Where(e => e.OrderReturnId == model.Id && model.OrderReturnDetail.Select(e => e.ProductId).Contains(e.ProductId)).ToList();
                foreach (var item in model.OrderReturnDetail)
                {
                    OrderReturnDetail orderReturnDetail = new OrderReturnDetail();
                    if (OrderReturnDetails.FirstOrDefault(e => e.Id == item.Id) == null)
                    {
                        var allproducts = _ProductMasterBLL.Where(x => model.OrderReturnDetail.Select(x => x.ProductId).Contains(x.Id));
                        List<DistributorWiseProductDiscountAndPrices> distributorWiseProductDiscountAndPrices = new List<DistributorWiseProductDiscountAndPrices>();
                        if (SessionHelper.DistributorWiseProductOrderReturn == null || SessionHelper.DistributorWiseProductOrderReturn.Count() == 0)
                        {
                            distributorWiseProductDiscountAndPrices = SessionHelper.DistributorWiseProductOrderReturn = _DistributorWiseProductDiscountAndPricesBLL.Where(e => (!SessionHelper.LoginUser.IsDistributor || e.DistributorId == SessionHelper.LoginUser.DistributorId) && e.ProductDetailId != null && (e.ProductDetail.ProductVisibilityId == ProductVisibility.Visible || e.ProductDetail.ProductVisibilityId == ProductVisibility.OrderReturn)).ToList();
                        }
                        else
                        {
                            distributorWiseProductDiscountAndPrices = SessionHelper.DistributorWiseProductOrderReturn;
                        }
                        productDetails = productDetails.Where(e => model.OrderReturnDetail.Select(x => x.ProductId).Contains(e.ProductMasterId)).ToList();
                        ProductMaster productMaster = allproducts.FirstOrDefault(e => e.Id == item.ProductId);
                        ProductDetail productDetail = productDetails.FirstOrDefault(e => e.ProductMasterId == item.ProductId);
                        var detail = SessionHelper.AddReturnProduct.First(e => e.ProductId == item.ProductId && e.ReceivedBatchNo.Trim() == item.ReceivedBatchNo.Trim());

                        detail.ReceivedMRP = detail.MRP = Convert.ToDouble(item.ReceivedMRP);
                        detail.ReceivedBatchNo = detail.BatchNo = item.ReceivedBatchNo;
                        detail.ReceivedQty = detail.Quantity = item.ReceivedQty;
                        detail.IsProductSelected = SessionHelper.AddReturnProduct.FirstOrDefault(e => e.ProductId == item.ProductId && e.ReceivedBatchNo.Trim() == item.ReceivedBatchNo.Trim()).IsProductSelected;
                        detail.TradePrice = item.ReceivedMRP - (item.ReceivedMRP / 100) * distributorWiseProductDiscountAndPrices.First(x => x.ProductDetailId == productDetail.Id).ReturnMRPDicount;
                        detail.NetAmount = Convert.ToDouble((detail.TradePrice - (-1 * detail.TradePrice / 100) * distributorWiseProductDiscountAndPrices.First(x => x.ProductDetailId == productDetail.Id).Discount) * item.ReceivedQty);
                        detail.TotalPrice = distributorWiseProductDiscountAndPrices.First(x => x.ProductDetailId == productDetail.Id).ProductPrice;
                        detail.Discount = distributorWiseProductDiscountAndPrices.First(x => x.ProductDetailId == productDetail.Id).Discount;
                        detail.OrderReturnId = model.Id;
                        detail.CreatedBy = SessionHelper.LoginUser.Id;
                        detail.CreatedDate = DateTime.Now;
                        detail.ReceivedBy = SessionHelper.LoginUser.Id;
                        detail.ReceivedDate = DateTime.Now;
                        detail.ProductMaster = null;
                        detail.PlantLocation = null;
                        _OrderReturnDetailBLL.Add(detail);
                        _unitOfWork.Save();
                    }
                    else
                    {
                        var product = OrderReturnDetails.First(e => e.Id == item.Id);
                        product.ReceivedMRP = item.ReceivedMRP;
                        product.ReceivedBatchNo = item.ReceivedBatchNo;
                        product.ReceivedQty = item.ReceivedQty;
                        product.IsProductSelected = SessionHelper.AddReturnProduct.FirstOrDefault(e => e.Id == item.Id).IsProductSelected;
                        product.ReceivedBy = SessionHelper.LoginUser.Id;
                        product.ReceivedDate = DateTime.Now;
                        _OrderReturnDetailBLL.Update(product);
                        _unitOfWork.Save();
                    }
                }
                OrderReturnDetails = _OrderReturnDetailBLL.Where(e => e.OrderReturnId == model.Id && model.OrderReturnDetail.Select(e => e.ProductId).Contains(e.ProductId)).ToList();
                OrderReturnDetails.ForEach(x => x.ProductDetail = productDetails.First(y => y.ProductMasterId == x.ProductMaster.Id));
                var SAPOrderDetail = OrderReturnDetails.Where(e => e.ProductDetail.IsPlaceOrderInSAP && e.IsProductSelected && SessionHelper.LoginUser.PlantLocationId == e.PlantLocationId).ToList();
                var POrderDetail = OrderReturnDetails.Where(e => !e.ProductDetail.IsPlaceOrderInSAP && e.IsProductSelected && SessionHelper.LoginUser.PlantLocationId == e.PlantLocationId).ToList();

                if (SAPOrderDetail != null && SAPOrderDetail.Count() > 0)
                {
                    List<SAPOrderStatus> SAPProduct = _OrderReturnBLL.PostDistributorOrderReturn(SAPOrderDetail, _Configuration);
                    if (SAPProduct != null && SAPProduct.Count() > 0)
                    {
                        foreach (var item in SAPProduct)
                        {
                            var productList = OrderReturnDetails.Where(e => e.ProductMaster.SAPProductCode == item.ProductCode).ToList();
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
                        SAPOrderStatus = _OrderReturnBLL.Update(master);
                    }
                    else
                    {
                        jsonResponse.Status = false;
                        jsonResponse.Message = "Unable to receive order";
                    }
                }
                if (POrderDetail != null && POrderDetail.Count() > 0)
                {
                    foreach (var item in POrderDetail)
                    {
                        item.ReturnOrderNumber = item.OrderReturnMaster.SNo.ToString();
                        item.ReturnOrderStatus = OrderStatus.CompletelyProcessed;
                        _OrderReturnDetailBLL.Update(item);
                    }
                    var UpdatedOrderDetail = _OrderReturnDetailBLL.Where(e => e.OrderReturnId == model.Id && e.ReturnOrderNumber == null).ToList();
                    master.Status = UpdatedOrderDetail.Count > 0 ? OrderReturnStatus.PartiallyReceived : OrderReturnStatus.Received;
                    master.ReceivedBy = SessionHelper.LoginUser.Id;
                    master.ReceivedDate = DateTime.Now;
                    POrderStatus = _OrderReturnBLL.Update(master);
                }

                //Sending Email
                if (SAPOrderStatus || POrderStatus)
                {
                    _unitOfWork.Commit();
                    foreach (var item in OrderReturnDetails.Select(x => x.ProductDetail.PlantLocationId).Distinct().ToArray())
                    {
                        string SAPOrder = string.Join("</br>" + Environment.NewLine, OrderReturnDetails.Where(x => x.ProductDetail.PlantLocationId == item && x.ProductDetail.IsPlaceOrderInSAP).Select(x => x.ReturnOrderNumber).Distinct().ToArray());
                        string DPOrder = string.Join("</br>" + Environment.NewLine, OrderReturnDetails.Where(x => x.ProductDetail.PlantLocationId == item && !x.ProductDetail.IsPlaceOrderInSAP).Select(x => x.ReturnOrderNumber).Distinct().ToArray());

                        string EmailTemplate = _env.WebRootPath + "\\Attachments\\EmailTemplates\\ReturnOrderFromCustomer.html";
                        ReturnOrderEmailUserModel EmailUserModel = new ReturnOrderEmailUserModel()
                        {
                            ToAcceptTemplate = System.IO.File.ReadAllText(EmailTemplate),
                            Date = Convert.ToDateTime(master.ReceivedDate).ToString("dd/MMM/yyyy"),
                            City = master.Distributor.City,
                            ShipToPartyName = master.Distributor.DistributorName,
                            SAPOrder = string.IsNullOrEmpty(SAPOrder) ? "" : "SAP Return Order No",
                            SAPOrderNumber = string.IsNullOrEmpty(SAPOrder) ? "" : SAPOrder,
                            DPOrder = string.IsNullOrEmpty(DPOrder) ? "" : "Distrbutor Portal Return Order No",
                            DPOrderNumber = string.IsNullOrEmpty(DPOrder) ? "" : OrderReturnDetails.First().OrderReturnMaster.SNo.ToString(),
                            Subject = "Return Delivery",
                            CreatedBy = SessionHelper.LoginUser.Id,
                            URL = _Configuration.URL
                        };
                        List<User> UserList = _UserBLL.GetAllActiveUser().Where(x => x.IsStoreKeeper && x.PlantLocationId != null && x.PlantLocationId == item
                        && Enum.GetValues(typeof(EmailIntimation)).Cast<int>().ToArray().Where(x => x.Equals(1) || x.Equals(2)).Contains(Convert.ToInt32(x.EmailIntimationId))).ToList();

                        if (UserList != null && UserList.Count() > 0 && (!string.IsNullOrEmpty(SAPOrder) || !string.IsNullOrEmpty(DPOrder)))
                        {
                            _EmailLogBLL.RetrunOrderEmail(UserList, EmailUserModel);
                        }
                    }
                    jsonResponse.Status = true;
                    jsonResponse.Message = NotificationMessage.ReceivedReturn;
                    jsonResponse.SignalRResponse = new SignalRResponse() { UserId = master.CreatedBy.ToString(), Number = "Order Return No: " + master.SNo, Message = jsonResponse.Message, Status = Enum.GetName(typeof(OrderReturnStatus), true) };
                    jsonResponse.RedirectURL = Url.Action("Index", "OrderReturn");
                    notification.CompanyId = SessionHelper.LoginUser.CompanyId;
                    notification.DistributorId = master.DistributorId;
                    notification.RequestId = master.SNo;
                    notification.Status = master.Status.ToString();
                    notification.Message = jsonResponse.SignalRResponse.Message;
                    notification.URL = "/OrderReturn/View?DPID=" + EncryptDecrypt.Encrypt(master.Id.ToString());
                    _NotificationBLL.Add(notification);
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
                list = _OrderReturnDetailBLL.Where(x => x.PlantLocationId == SessionHelper.LoginUser.PlantLocationId && x.OrderReturnMaster.Status != OrderReturnStatus.Draft
                && ProductMasterIds.Contains(x.ProductId)).DistinctBy(e => e.OrderReturnId).Select(x => new OrderReturnMaster
                {
                    TRNo = x.OrderReturnMaster.TRNo,
                    SNo = x.OrderReturnMaster.SNo,
                    Id = x.OrderReturnMaster.Id,
                    Distributor = x.OrderReturnMaster.Distributor,
                    Status = x.OrderReturnMaster.Status,
                    CreatedBy = x.OrderReturnMaster.CreatedBy,
                    CreatedDate = x.OrderReturnMaster.CreatedDate
                }).ToList();
            }
            else
            {
                list = _OrderReturnBLL.Where(x => SessionHelper.LoginUser.IsDistributor == true ? x.DistributorId == SessionHelper.LoginUser.DistributorId : true).ToList();
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
                List<ProductDetail> allproductDetails = new List<ProductDetail>();
                List<ProductDetail> allproductDetail = allproductDetails = _ProductDetailBLL.GetAllProductDetail();
                if (SessionHelper.LoginUser.IsStoreKeeper)
                {
                    int[] productIds = model.OrderReturnDetail.Where(x => x.PlantLocationId == SessionHelper.LoginUser.PlantLocationId).Select(x => x.ProductId).ToArray();
                    allproductDetail = allproductDetail.Where(e => productIds.Contains(e.ProductMasterId)).ToList();
                }
                else
                {
                    allproductDetail = allproductDetail.Where(e => model.OrderReturnDetail.Select(x => x.ProductId).Contains(e.ProductMasterId)).ToList();
                }
                foreach (var item in model.OrderReturnDetail)
                {
                    ProductMaster productMaster = allproducts.First(e => e.Id == item.ProductId);
                    ProductDetail productDetail = allproductDetail.FirstOrDefault(e => e.ProductMasterId == item.ProductId);
                    OrderReturnDetail detail = new OrderReturnDetail();

                    if (productMaster != null && productDetail != null)
                    {
                        var list = SessionHelper.AddReturnProduct;
                        detail.Id = item.Id;
                        detail.ReceivedBy = item.ReceivedBy;
                        detail.ReceivedBatchNo = detail.BatchNo = item.BatchNo;
                        detail.ReceivedMRP = detail.MRP = item.MRP;
                        if (item.ReceivedBy != null)
                        {
                            detail.ReceivedQty = item.ReceivedQty;
                            detail.Quantity = item.Quantity;
                        }
                        else
                        {
                            detail.ReceivedQty = detail.Quantity = item.Quantity;
                        }
                        detail.TradePrice = item.TradePrice;
                        detail.NetAmount = item.NetAmount;
                        detail.TotalPrice = item.TotalPrice;
                        detail.Discount = item.Discount;
                        detail.ManufactureDate = item.ManufactureDate;
                        detail.IntimationDate = item.IntimationDate;
                        detail.ExpiryDate = item.ExpiryDate;
                        detail.InvoiceNo = item.InvoiceNo;
                        detail.InvoiceDate = item.InvoiceDate;
                        detail.IntimationDate = item.IntimationDate;
                        detail.ProductMaster = productMaster;
                        detail.OrderReturnNumber = list.Count == 0 ? 1 : list.Max(e => e.OrderReturnNumber) + 1;
                        detail.Remarks = item.Remarks;
                        detail.ProductId = item.ProductId;
                        detail.OrderReturnId = item.OrderReturnId;
                        detail.PlantLocationId = item.PlantLocationId;
                        detail.PlantLocation = item.PlantLocation;
                        detail.Company = item.IsFOCProduct ? allproductDetails.First(x => x.FOCProductCode == item.ProductMaster.SAPProductCode).Company : productDetail.Company;
                        detail.ReturnOrderNumber = item.ReturnOrderNumber;
                        detail.ReturnOrderStatus = item.ReturnOrderStatus;
                        detail.IsProductSelected = true;
                        detail.IsFOCProduct = item.IsFOCProduct;
                        list.Add(detail);
                        SessionHelper.AddReturnProduct = list;
                    }
                }
                model.OrderReturnDetail = SessionHelper.AddReturnProduct;
                model.ProductList = _ProductDetailBLL.DropDownProductReturnList();
                model.ReturnReasonList = _orderReturnReasonBLL.DropDownOrderReturnReasonList();
            }
            else
            {
                model.ProductList = _ProductDetailBLL.DropDownProductReturnList();
                model.ReturnReasonList = _orderReturnReasonBLL.DropDownOrderReturnReasonList();
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
            int.TryParse(EncryptDecrypt.Decrypt(DPID), out int id);
            var list = SessionHelper.AddReturnProduct;
            var item = list.FirstOrDefault(e => e.ProductId == id && e.BatchNo.Trim() == BatchNo.Trim());

            if (item != null)
            {
                ProductDetail productDetail = _ProductDetailBLL.Where(e => e.ProductMasterId == item.ProductId).FirstOrDefault();

                if (!string.IsNullOrEmpty(productDetail.FOCProductCode))
                {
                    var focProductMaster = _ProductMasterBLL.Where(e => e.SAPProductCode == productDetail.FOCProductCode).FirstOrDefault();
                    var focItem = list.FirstOrDefault(e => e.ProductId == focProductMaster.Id);
                    list.Remove(focItem);
                }
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
                if (!SessionHelper.AddReturnProduct.Any(e => e.ProductId == model.ProductId && e.ReceivedBatchNo.Trim() == model.BatchNo.Trim()))
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
            List<DistributorWiseProductDiscountAndPrices> distributorWiseProductDiscountAndPrices = new List<DistributorWiseProductDiscountAndPrices>();
            if (SessionHelper.DistributorWiseProductOrderReturn == null || SessionHelper.DistributorWiseProductOrderReturn.Count() == 0)
            {
                distributorWiseProductDiscountAndPrices = SessionHelper.DistributorWiseProductOrderReturn = _DistributorWiseProductDiscountAndPricesBLL.Where(e => (!SessionHelper.LoginUser.IsDistributor || e.DistributorId == SessionHelper.LoginUser.DistributorId) && e.ProductDetailId != null && (e.ProductDetail.ProductVisibilityId == ProductVisibility.Visible || e.ProductDetail.ProductVisibilityId == ProductVisibility.OrderReturn)).ToList();
            }
            else
            {
                distributorWiseProductDiscountAndPrices = SessionHelper.DistributorWiseProductOrderReturn;
            }
            ProductMaster productMaster = _ProductMasterBLL.FirstOrDefault(e => e.Id == model.ProductId);
            ProductDetail productDetail = _ProductDetailBLL.FirstOrDefault(e => e.ProductMasterId == model.ProductId);
            var list = SessionHelper.AddReturnProduct;
            detail.ReceivedBatchNo = detail.BatchNo = model.BatchNo;
            detail.ReceivedMRP = model.MRP;
            detail.ReceivedQty = model.Quantity;
            detail.TradePrice = model.MRP - (model.MRP / 100) * distributorWiseProductDiscountAndPrices.First(x => x.ProductDetailId == productDetail.Id).ReturnMRPDicount;
            detail.NetAmount = Convert.ToDouble((detail.TradePrice - (-1 * detail.TradePrice / 100) * distributorWiseProductDiscountAndPrices.First(x => x.ProductDetailId == productDetail.Id).Discount) * model.Quantity);
            detail.TotalPrice = distributorWiseProductDiscountAndPrices.First(x => x.ProductDetailId == productDetail.Id).ProductPrice;
            detail.Discount = distributorWiseProductDiscountAndPrices.First(x => x.ProductDetailId == productDetail.Id).Discount;
            detail.TotalPrice = model.TotalPrice;
            detail.ManufactureDate = model.ManufactureDate;
            detail.IntimationDate = model.IntimationDate;
            detail.ExpiryDate = model.ExpiryDate;
            detail.InvoiceNo = model.InvoiceNo;
            detail.InvoiceDate = model.InvoiceDate;
            detail.ProductMaster = productMaster;
            detail.OrderReturnNumber = list.Count == 0 ? 1 : list.Max(e => e.OrderReturnNumber) + 1;
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
            int.TryParse(EncryptDecrypt.Decrypt(DPID), out int id);
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
                    jsonResponse.Message = "Order return has been rejected";
                    jsonResponse.RedirectURL = Url.Action("Index", "OrderReturn");
                    jsonResponse.SignalRResponse = new SignalRResponse() { UserId = order.CreatedBy.ToString(), Number = "Order Return #: " + order.SNo, Message = "Order has been rejected by Admin", Status = Enum.GetName(typeof(OrderStatus), order.Status) };
                    notification.CompanyId = SessionHelper.LoginUser.CompanyId;
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
                bool SAPOrderStatus = false;
                bool POrderStatus = false;
                int.TryParse(EncryptDecrypt.Decrypt(DPID), out int id);
                var order = _OrderReturnBLL.GetById(id);
                var master = _OrderReturnBLL.FirstOrDefault(e => e.Id == id);
                _unitOfWork.Begin();
                var OrderReturnDetails = _OrderReturnDetailBLL.Where(e => e.OrderReturnId == id).ToList();
                foreach (var item in OrderReturnDetails)
                {
                    OrderReturnDetail orderReturnDetail = new OrderReturnDetail();
                    if (OrderReturnDetails.FirstOrDefault(e => e.ProductId == item.ProductId && e.BatchNo.Trim() == item.BatchNo.Trim()) == null)
                    {
                        var detail = SessionHelper.AddReturnProduct.First(e => e.ProductId == item.ProductId && e.BatchNo.Trim() == item.BatchNo.Trim());
                        detail.IsProductSelected = SessionHelper.AddReturnProduct.FirstOrDefault(x => x.ProductId == item.ProductId).IsProductSelected;
                        detail.OrderReturnId = id;
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
                        var product = OrderReturnDetails.First(e => e.ProductId == item.ProductId && e.BatchNo.Trim() == item.BatchNo.Trim());
                        product.ReceivedQty = item.ReceivedQty;
                        product.IsProductSelected = true;
                        product.ReceivedBy = SessionHelper.LoginUser.Id;
                        product.ReceivedDate = DateTime.Now;
                        _OrderReturnDetailBLL.Update(product);
                        _unitOfWork.Save();
                    }
                }
                List<ProductDetail> productDetails = _ProductDetailBLL.GetAllProductDetail();
                OrderReturnDetails.ForEach(x => x.ProductDetail = productDetails.First(y => y.ProductMasterId == x.ProductMaster.Id));
                var SAPOrderDetail = OrderReturnDetails.Where(e => e.ProductDetail.IsPlaceOrderInSAP && e.IsProductSelected && SessionHelper.LoginUser.PlantLocationId == e.PlantLocationId).ToList();
                var POrderDetail = OrderReturnDetails.Where(e => !e.ProductDetail.IsPlaceOrderInSAP && e.IsProductSelected && SessionHelper.LoginUser.PlantLocationId == e.PlantLocationId).ToList();

                if (SAPOrderDetail != null && SAPOrderDetail.Count() > 0)
                {
                    List<SAPOrderStatus> SAPProduct = _OrderReturnBLL.PostDistributorOrderReturn(SAPOrderDetail, _Configuration);
                    if (SAPProduct != null && SAPProduct.Count() > 0)
                    {
                        foreach (var item in SAPProduct)
                        {
                            var productList = OrderReturnDetails.Where(e => e.ProductMaster.SAPProductCode == item.ProductCode).ToList();
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
                        var SAPOrderDetailUpdate = OrderReturnDetails.Where(e => e.ReturnOrderNumber == null).ToList();
                        order.Status = SAPOrderDetailUpdate.Count > 0 ? OrderReturnStatus.PartiallyReceived : OrderReturnStatus.Received;
                        order.ReceivedBy = SessionHelper.LoginUser.Id;
                        order.ReceivedDate = DateTime.Now;
                        SAPOrderStatus = _OrderReturnBLL.Update(order);
                    }
                }
                else
                {
                    jsonResponse.Status = false;
                    jsonResponse.Message = "Unable to receive order";
                }
                if (POrderDetail != null && POrderDetail.Count() > 0)
                {
                    foreach (var item in POrderDetail)
                    {
                        item.ReturnOrderNumber = item.OrderReturnMaster.SNo.ToString();
                        item.ReturnOrderStatus = OrderStatus.CompletelyProcessed;
                        _OrderReturnDetailBLL.Update(item);
                    }
                    POrderDetail = OrderReturnDetails.Where(e => e.ReturnOrderNumber == null).ToList();
                    order.Status = POrderDetail.Count > 0 ? OrderReturnStatus.PartiallyReceived : OrderReturnStatus.Received;
                    order.ReceivedBy = SessionHelper.LoginUser.Id;
                    order.ReceivedDate = DateTime.Now;
                    POrderStatus = _OrderReturnBLL.Update(order);
                }
                var UpdatedOrderDetail = OrderReturnDetails.Where(e => e.ReturnOrderNumber == null).ToList();
                master.Status = UpdatedOrderDetail.Count > 0 ? OrderReturnStatus.PartiallyReceived : OrderReturnStatus.Received;
                master.ReceivedBy = SessionHelper.LoginUser.Id;
                master.ReceivedDate = DateTime.Now;
                var result = _OrderReturnBLL.Update(master);
                _unitOfWork.Commit();

                //Sending Email
                if (SAPOrderStatus || POrderStatus)
                {
                    foreach (var item in OrderReturnDetails.Select(x => x.ProductDetail.PlantLocationId).Distinct().ToArray())
                    {
                        string SAPOrder = string.Join("</br>" + Environment.NewLine, OrderReturnDetails.Where(x => x.ProductDetail.PlantLocationId == item && x.ProductDetail.IsPlaceOrderInSAP).Select(x => x.ReturnOrderNumber).Distinct().ToArray());
                        string DPOrder = string.Join("</br>" + Environment.NewLine, OrderReturnDetails.Where(x => x.ProductDetail.PlantLocationId == item && !x.ProductDetail.IsPlaceOrderInSAP).Select(x => x.ReturnOrderNumber).Distinct().ToArray());

                        string EmailTemplate = _env.WebRootPath + "\\Attachments\\EmailTemplates\\ReturnOrderFromCustomer.html";
                        ReturnOrderEmailUserModel EmailUserModel = new ReturnOrderEmailUserModel()
                        {
                            ToAcceptTemplate = System.IO.File.ReadAllText(EmailTemplate),
                            Date = Convert.ToDateTime(master.ReceivedDate).ToString("dd/MMM/yyyy"),
                            City = master.Distributor.City,
                            ShipToPartyName = master.Distributor.DistributorName,
                            SAPOrder = string.IsNullOrEmpty(SAPOrder) ? "" : "SAP Return Order No",
                            SAPOrderNumber = string.IsNullOrEmpty(SAPOrder) ? "" : SAPOrder,
                            DPOrder = string.IsNullOrEmpty(DPOrder) ? "" : "Distrbutor Portal Return Order No",
                            DPOrderNumber = string.IsNullOrEmpty(DPOrder) ? "" : OrderReturnDetails.First().OrderReturnMaster.SNo.ToString(),
                            Subject = "Return Delivery",
                            CreatedBy = SessionHelper.LoginUser.Id,
                            URL = _Configuration.URL
                        };
                        List<User> UserList = _UserBLL.GetAllActiveUser().Where(x => x.IsStoreKeeper && x.PlantLocationId != null && x.PlantLocationId == item
                        && Enum.GetValues(typeof(EmailIntimation)).Cast<int>().ToArray().Where(x => x.Equals(1) || x.Equals(2)).Contains(Convert.ToInt32(x.EmailIntimationId))).ToList();

                        if (UserList != null && UserList.Count() > 0 && (!string.IsNullOrEmpty(SAPOrder) || !string.IsNullOrEmpty(DPOrder)))
                        {
                            _EmailLogBLL.RetrunOrderEmail(UserList, EmailUserModel);
                        }
                    }
                }
                jsonResponse.Status = result;
                jsonResponse.Message = result ? NotificationMessage.ReceivedReturn : NotificationMessage.ErrorOccurred;
                jsonResponse.SignalRResponse = new SignalRResponse() { UserId = master.CreatedBy.ToString(), Number = "Order Return No: " + master.SNo, Message = jsonResponse.Message, Status = Enum.GetName(typeof(OrderReturnStatus), result) };
                jsonResponse.RedirectURL = Url.Action("Index", "OrderReturn");
                notification.CompanyId = SessionHelper.LoginUser.CompanyId;
                notification.DistributorId = master.DistributorId;
                notification.RequestId = master.SNo;
                notification.Status = master.Status.ToString();
                notification.Message = jsonResponse.SignalRResponse.Message;
                notification.URL = "/OrderReturn/View?DPID=" + EncryptDecrypt.Encrypt(master.Id.ToString());
                _NotificationBLL.Add(notification);
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
    }
}

