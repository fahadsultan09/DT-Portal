using BusinessLogicLayer.Application;
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
        private readonly ProductDetailBLL _productDetailBLL;
        private readonly IConfiguration _IConfiguration;
        private ICompositeViewEngine _viewEngine;
        public OrderController(IUnitOfWork unitOfWork, IConfiguration configuration, ICompositeViewEngine viewEngine)
        {
            _unitOfWork = unitOfWork;
            _OrderBLL = new OrderBLL(_unitOfWork);
            _productDetailBLL = new ProductDetailBLL(_unitOfWork);
            _orderDetailBLL = new OrderDetailBLL(_unitOfWork);
            _IConfiguration = configuration;
            _viewEngine = viewEngine;
        }
        // GET: Order
        public ActionResult Index()
        {
            return View(_OrderBLL.GetAllOrderMaster());
        }
        public IActionResult List()
        {
            return PartialView("List", _OrderBLL.GetAllOrderMaster());
        }
        [HttpGet]
        public IActionResult Add(int id)
        {
            SessionHelper.AddProduct = new List<ProductDetail>();
            return PartialView("Add", BindOrderMaster(id));
        }
        [HttpGet]
        public IActionResult PaymentApproval(int id)
        {
            SessionHelper.AddProduct = new List<ProductDetail>();
            return PartialView("PaymentApproval", BindOrderMaster(id));
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
                    list.Add(master);
                    SessionHelper.AddProduct = list;
                    jsonResponse.Status = true;
                    jsonResponse.Message = "Product Added Successfully";
                    jsonResponse.RedirectURL = string.Empty;
                    jsonResponse.HtmlString = RenderRazorViewToString("AddToGrid", SessionHelper.AddProduct);
                }                                
            }
            else
            {
                jsonResponse.Status = false;
                jsonResponse.Message = "Product Already Exists";
                jsonResponse.RedirectURL = string.Empty;
            }
            return Json(new { data = jsonResponse });
        }
        public IActionResult Delete(int Id)
        {            
            var list = SessionHelper.AddProduct;
            var item = list.First(e => e.ProductMasterId == Id);
            list.Remove(item);
            SessionHelper.AddProduct = list;
            return PartialView("AddToGrid", SessionHelper.AddProduct);
        }
        [HttpPost]
        public JsonResult SaveEdit(OrderMaster model, SubmitStatus btnSubmit)
        {
            JsonResponse jsonResponse = new JsonResponse();
            string FolderPath = _IConfiguration.GetSection("Settings").GetSection("FolderPath").Value;
            try
            {
                ModelState.Remove("Id");
                ModelState.Remove("Attachment");
                if (!ModelState.IsValid)
                {
                    jsonResponse.Status = false;
                    jsonResponse.Message = NotificationMessage.RequiredFieldsValidation;
                }
                else
                {
                    if (btnSubmit == SubmitStatus.Draft)
                    {
                        model.Status = OrderStatus.Draft;
                    }
                    else
                    {
                        model.Status = OrderStatus.Submit;
                    }
                    string[] permittedExtensions = Common.permittedExtensions;
                    if (model.AttachmentFormFile != null)
                    {
                        var ext = Path.GetExtension(model.AttachmentFormFile.FileName).ToLowerInvariant();
                        if (permittedExtensions.Contains(ext) && model.AttachmentFormFile.Length < Convert.ToInt64(5242880))
                        {
                            Tuple<bool, string> tuple = FileUtility.UploadFile(model.AttachmentFormFile, FolderName.Order, FolderPath);
                            if (tuple.Item1)
                            {
                                model.Attachment = tuple.Item2;
                            }
                        }
                    }
                    model.TotalValue = SessionHelper.AddProduct.Select(e => e.TotalPrice).Sum();
                    model.DistributorId = SessionHelper.LoginUser.DistributorId ?? 1;
                    _OrderBLL.Add(model);
                    List<OrderDetail> details = new List<OrderDetail>();
                    foreach (var item in SessionHelper.AddProduct)
                    {
                        details.Add(new OrderDetail()
                        {
                            Amount = item.TotalPrice,
                            OrderId = model.Id,
                            ProductId = item.ProductMasterId,
                            Quantity = item.ProductMaster.Quantity,
                            CreatedBy = SessionHelper.LoginUser.Id,
                            CreatedDate = DateTime.Now
                        });
                    }
                    _orderDetailBLL.AddRange(details);
                    jsonResponse.Status = true;
                    jsonResponse.Message = NotificationMessage.OrderSaved;
                    jsonResponse.RedirectURL = Url.Action("Index", "Order");
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
        private OrderMaster BindOrderMaster(int Id)
        {
            OrderMaster model = new OrderMaster();
            if (Id > 0)
            {
                model = _OrderBLL.GetOrderMasterById(Id);
            }
            else
            {
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
    }
}
