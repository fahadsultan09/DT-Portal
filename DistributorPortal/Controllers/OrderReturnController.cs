
using BusinessLogicLayer.Application;
using BusinessLogicLayer.ApplicationSetup;
using BusinessLogicLayer.ErrorLog;
using BusinessLogicLayer.GeneralSetup;
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
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Utility;
using static Utility.Constant.Common;

namespace DistributorPortal.Controllers
{
    public class OrderReturnController : BaseController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly OrderReturnBLL _OrderReturnBLL;
        private readonly OrderReturnDetailBLL _OrderReturnDetailBLL;
        private readonly ProductMasterBLL _ProductMasterBLL;
        private ICompositeViewEngine _viewEngine;
        public OrderReturnController(IUnitOfWork unitOfWork, ICompositeViewEngine viewEngine)
        {
            _unitOfWork = unitOfWork;
            _OrderReturnBLL = new OrderReturnBLL(_unitOfWork);
            _OrderReturnDetailBLL = new OrderReturnDetailBLL(_unitOfWork);
            _ProductMasterBLL = new ProductMasterBLL(_unitOfWork);
            _viewEngine = viewEngine;
        }
        public IActionResult Index()
        {
            new AuditTrailBLL(_unitOfWork).AddAuditTrail("OrderReturn", "Index", " Form");
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
        public IActionResult Add(int id)
        {            
            SessionHelper.AddReturnProduct = new List<OrderReturnDetail>();
            return View("Add", BindOrderReturnMaster(id));
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
        public JsonResult AddProduct(OrderReturnDetail model)
        {
            JsonResponse jsonResponse = new JsonResponse();
            try
            {
                if (!SessionHelper.AddReturnProduct.Any(e => e.ProductId == model.ProductId))
                {
                    ProductMaster productMaster = _ProductMasterBLL.GetProductMasterById(model.ProductId);
                    if (productMaster != null)
                    {
                        var list = SessionHelper.AddReturnProduct;
                        model.ProductMaster = productMaster;
                        model.PlantLocation = new PlantLocationBLL(_unitOfWork).GetAllPlantLocation().Where(x => x.Id == model.PlantLocationId).FirstOrDefault();
                        model.OrderReturnNumber = list.Count == 0 ? 1 : list.Max(e => e.OrderReturnNumber) + 1;
                        model.NetAmount = model.Quantity * model.MRP;
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
        [HttpPost]
        public IActionResult Search(OrderReturnViewModel model, string Search)
        {
            new AuditTrailBLL(_unitOfWork).AddAuditTrail("OrderReturn", "Search", "Start Click on Search Button of ");
            if (!string.IsNullOrEmpty(Search))
            {
                model = List(model);
            }
            else
            {
                model.OrderReturnMaster = GetOrderReturnList();
            }
            new AuditTrailBLL(_unitOfWork).AddAuditTrail("OrderReturn", "Search", "End Click on Search Button of ");
            return PartialView("List", model.OrderReturnMaster);
        }
        public List<OrderReturnMaster> GetOrderReturnList()
        {
            var list = _OrderReturnBLL.GetAllOrderReturn().Where(x => SessionHelper.LoginUser.IsDistributor == true ? x.DistributorId == SessionHelper.LoginUser.DistributorId : true).OrderByDescending(x => x.Id).ToList();
            return list;
        }
        private OrderReturnMaster BindOrderReturnMaster(int Id)
        {
            OrderReturnMaster model = new OrderReturnMaster();
            if (Id > 0)
            {
                model = _OrderReturnBLL.GetById(Id);
                model.Distributor = new DistributorBLL(_unitOfWork).Where(x => x.Id == model.DistributorId).FirstOrDefault();
                model.OrderReturnDetail = _OrderReturnDetailBLL.Where(e => e.OrderReturnId == Id).ToList();
                OrderReturnDetail detail = new OrderReturnDetail();
                foreach (var item in model.OrderReturnDetail)
                {
                    ProductMaster productMaster = _ProductMasterBLL.GetProductMasterById(item.ProductId);
                    if (productMaster != null)
                    {
                        var list = SessionHelper.AddReturnProduct;
                        detail.ProductMaster = productMaster;
                        detail.PlantLocation = new PlantLocationBLL(_unitOfWork).GetAllPlantLocation().Where(x => x.Id == item.PlantLocationId).FirstOrDefault();
                        detail.OrderReturnNumber = list.Count == 0 ? 1 : list.Max(e => e.OrderReturnNumber) + 1;
                        detail.NetAmount = detail.Quantity * detail.MRP;
                        list.Add(detail);
                        SessionHelper.AddReturnProduct = list;                       
                    }
                }               
            }
            else
            {
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
        public IActionResult Delete(int Id)
        {
            var list = SessionHelper.AddReturnProduct;
            var item = list.FirstOrDefault(e => e.ProductId == Id);
            if (item != null)
            {
                list.Remove(item);
            }
            SessionHelper.AddReturnProduct = list;
            new AuditTrailBLL(_unitOfWork).AddAuditTrail("OrderMaster", "Delete", "End Click on Delete Button of ");
            return PartialView("AddToGrid", SessionHelper.AddReturnProduct.OrderByDescending(e => e.OrderReturnNumber));
        }
    }
}
