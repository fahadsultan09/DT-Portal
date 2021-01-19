

using BusinessLogicLayer.Application;
using BusinessLogicLayer.HelperClasses;
using DataAccessLayer.WorkProcess;
using DistributorPortal.BusinessLogicLayer.ApplicationSetup;
using Microsoft.AspNetCore.Mvc;
using Models.Application;
using Models.ViewModel;
using System.Collections.Generic;
using System.Linq;

namespace DistributorPortal.Controllers
{
    public class OrderReturnController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly OrderReturnBLL _OrderReturnBLL;
        private readonly OrderReturnDetailBLL _OrderReturnDetailBLL;
        public OrderReturnController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _OrderReturnBLL = new OrderReturnBLL(_unitOfWork);
            _OrderReturnDetailBLL = new OrderReturnDetailBLL(_unitOfWork);
        }
        public IActionResult Index()
        {
            return View();
        }
        public OrderReturnViewModel List(OrderReturnViewModel model)
        {
            if (model.DistributorId is null && model.Status is null && model.FromDate is null && model.ToDate is null)
            {
                model.OrderReturnMaster = GetOrderReturnList();
            }
            else
            {
                //model.OrderReturnMaster = _OrderReturnBLL.Search(model).Where(x => SessionHelper.LoginUser.IsDistributor == true ? x.DistributorId == SessionHelper.LoginUser.DistributorId : true).ToList();
            }
            return model;
        }
        [HttpGet]
        public IActionResult Add(int id)
        {
            new AuditTrailBLL(_unitOfWork).AddAuditTrail("OrderReturn", "Add", "Click on Add  Button of ");
            return View("AddDetail", BindOrderReturnMaster(id));
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
                //model = _OrderReturnBLL.GetById(Id);
            }
            else
            {

            }
            return model;
        }
    }
}
