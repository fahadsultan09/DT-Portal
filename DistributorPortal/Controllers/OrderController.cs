using BusinessLogicLayer.Application;
using BusinessLogicLayer.ErrorLog;
using DataAccessLayer.WorkProcess;
using DistributorPortal.Resource;
using Microsoft.AspNetCore.Mvc;
using Models.Application;
using Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DistributorPortal.Controllers
{
    public class OrderController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly OrderBLL _OrderBLL;
        public OrderController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _OrderBLL = new OrderBLL(_unitOfWork);
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
            return PartialView("Add", BindOrderMaster(id));
        }
        [HttpPost]
        public JsonResult SaveEdit(Distributor model)
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
            model.ProductList = new ProductMasterBLL(_unitOfWork).DropDownProductList();
            return model;
        }
    }
}
