using BusinessLogicLayer.GeneralSetup;
using BusinessLogicLayer.HelperClasses;
using DataAccessLayer.Repository;
using DataAccessLayer.WorkProcess;
using Models.Application;
using Models.ViewModel;
using MoreLinq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Utility;

namespace BusinessLogicLayer.Application
{
    public class OrderDetailBLL
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGenericRepository<OrderDetail> _repository;
        private readonly IGenericRepository<OrderMaster> _repository_OrderMaster;
        private readonly AuditTrailBLL<OrderDetail> _AuditTrail_OrderDetail;
        public OrderDetailBLL(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _repository = _unitOfWork.GenericRepository<OrderDetail>();
            _repository_OrderMaster = _unitOfWork.GenericRepository<OrderMaster>();
            _AuditTrail_OrderDetail = new AuditTrailBLL<OrderDetail>(_unitOfWork);
        }
        public int Add(OrderDetail module)
        {
            module.CreatedBy = SessionHelper.LoginUser.Id;
            module.CreatedDate = DateTime.Now;
            _repository.Insert(module);
            return _unitOfWork.Save();
        }
        public int AddRange(List<OrderDetail> module)
        {
            _repository.AddRange(module);
            return _unitOfWork.Save();
        }
        public int UpdateRange(List<OrderDetail> module)
        {
            _repository.UpdateRange(module);
            //foreach (var item in module)
            //{
            //    _AuditTrail_OrderDetail.AddAuditTrail((int)ApplicationPages.Order, (int)ApplicationActions.Update, module, "Save Order Detail", item.CreatedBy);
            //}
            return _unitOfWork.Save();
        }
        public int DeleteRange(List<OrderDetail> module)
        {
            _repository.DeleteRange(module);
            return _unitOfWork.Save();
        }
        public int Update(OrderDetail module)
        {
            var item = _repository.GetById(module.Id);
            item.IsProductSelected = module.IsProductSelected;
            item.SAPOrderNumber = module.SAPOrderNumber;
            item.OrderProductStatus = module.OrderProductStatus;
            _repository.Update(item);
            return _unitOfWork.Save();
        }
        public int Delete(int id)
        {
            var item = _repository.GetById(id);
            _repository.Delete(item);
            return _unitOfWork.Save();
        }
        public OrderDetail GetOrderDetailById(int id)
        {
            return _repository.GetById(id);
        }
        public List<OrderDetail> GetOrderDetailByIdByMasterId(int OrderId)
        {
            return _repository.Where(x => x.OrderId == OrderId).ToList();
        }
        public List<OrderDetail> GetAllOrderDetail()
        {
            return _repository.GetAllList().ToList();
        }
        public List<OrderDetail> Where(Expression<Func<OrderDetail, bool>> predicate)
        {
            return _repository.Where(predicate);
        }
        public OrderDetail FirstOrDefault(Expression<Func<OrderDetail, bool>> predicate)
        {
            return _repository.FirstOrDefault(predicate);
        }
        public List<SAPOrderStatus> GetInProcessOrderStatus()
        {
            List<SAPOrderStatus> list = _repository.Where(x => x.SAPOrderNumber != null && x.OrderProductStatus != OrderStatus.CompletelyProcessed && x.OrderProductStatus != OrderStatus.ApprovedCanceled).Select(x => new SAPOrderStatus { SAPOrderNo = x.SAPOrderNumber }).DistinctBy(x => x.SAPOrderNo).ToList();
            return list;
        }
        public void UpdateProductOrderStatus(List<SAPOrderStatus> SAPOrderStatusList)
        {
            try
            {
                _unitOfWork.Begin();
                List<OrderMaster> orderMasters = _repository_OrderMaster.Where(x => x.IsActive && !x.IsDeleted).ToList();
                List<OrderDetail> orderDetails = _repository.Where(x => SAPOrderStatusList.Select(x => x.SAPOrderNo).Distinct().Contains(x.SAPOrderNumber)).ToList();
                foreach (var item in orderDetails)
                {
                    SAPOrderStatus sAPOrderStatus = SAPOrderStatusList.FirstOrDefault(x => x.SAPOrderNo == item.SAPOrderNumber);
                    if (sAPOrderStatus != null)
                    {
                        item.OrderProductStatus = sAPOrderStatus.OrderStatus.Trim() == "B" ? OrderStatus.PartiallyProcessed : (sAPOrderStatus.OrderStatus.Trim() == "C" ? OrderStatus.CompletelyProcessed : (sAPOrderStatus.OrderStatus.Trim() == "D" ? OrderStatus.ApprovedCanceled : OrderStatus.InProcess));
                    }
                }
                UpdateRange(orderDetails);
                if (orderDetails != null && orderDetails.Count() > 0)
                {
                    orderDetails = orderDetails.Where(x => x.OrderProductStatus == OrderStatus.ApprovedCanceled).ToList();

                    List<RecentOrder> recentOrders = orderDetails.GroupBy(x => x.OrderId).Select(y => new RecentOrder
                    {
                        OrderNo = orderMasters.FirstOrDefault(x => x.Id == y.Key).Id,
                        Amount = y.Sum(x => x.Amount)
                    }).ToList();

                    if (orderDetails != null && orderDetails.Count() > 0)
                    {
                        foreach (var recentOrder in recentOrders)
                        {
                            double result = orderMasters.First(x => x.Id == recentOrder.OrderNo).TotalValue - recentOrder.Amount;
                            orderMasters.First(x => x.Id == recentOrder.OrderNo).TotalValue = result;
                        }
                        new OrderBLL(_unitOfWork).UpdateRange(orderMasters);
                        UpdateOrderValueModel(orderDetails);
                    }
                }
                _unitOfWork.Commit();
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                throw ex;
            }
        }
        public void UpdateOrderValueModel(List<OrderDetail> orderDetails)
        {
            var SAMI = Convert.ToInt32(CompanyEnum.SAMI);
            var HealthTek = Convert.ToInt32(CompanyEnum.Healthtek);
            var Phytek = Convert.ToInt32(CompanyEnum.Phytek);
            List<OrderValue> orderValueList = new List<OrderValue>();
            OrderValueViewModel viewModel = new OrderValueViewModel();
            List<Company> companies = new CompanyBLL(_unitOfWork).GetAllCompany();
            List<ProductDetail> productDetails = new ProductDetailBLL(_unitOfWork).GetAllProductDetail().ToList();
            orderDetails.ForEach(x => x.ProductDetail = productDetails.FirstOrDefault(y => y.ProductMasterId == x.ProductId));
            int orderId = orderDetails.First().OrderId;
            OrderValue SAMIorderValues = new OrderValueBLL(_unitOfWork).FirstOrDefault(x => x.OrderId == orderId && x.CompanyId == Convert.ToInt32(CompanyEnum.SAMI));
            OrderValue HealthtekorderValues = new OrderValueBLL(_unitOfWork).FirstOrDefault(x => x.OrderId == orderId && x.CompanyId == Convert.ToInt32(CompanyEnum.Healthtek));
            OrderValue PhytekorderValues = new OrderValueBLL(_unitOfWork).FirstOrDefault(x => x.OrderId == orderId && x.CompanyId == Convert.ToInt32(CompanyEnum.Phytek));

            var SAMIOrderDetails = orderDetails.Where(e => e.ProductDetail.CompanyId == SAMI).ToList();
            SAMIorderValues.SuppliesZero = SAMIorderValues.SuppliesZero - SAMIOrderDetails.Where(e => e.ProductDetail.WTaxRate == "0").Sum(e => e.Amount) <= 0 ? 0 : SAMIorderValues.SuppliesZero - SAMIOrderDetails.Where(e => e.ProductDetail.WTaxRate == "0").Sum(e => e.Amount);
            SAMIorderValues.SuppliesOne = SAMIorderValues.SuppliesOne - SAMIOrderDetails.Where(e => e.ProductDetail.WTaxRate == "1").Sum(e => e.Amount) <= 0 ? 0 : SAMIorderValues.SuppliesOne - SAMIOrderDetails.Where(e => e.ProductDetail.WTaxRate == "1").Sum(e => e.Amount);
            SAMIorderValues.SuppliesFour = SAMIorderValues.SuppliesFour - SAMIOrderDetails.Where(e => e.ProductDetail.WTaxRate == "4").Sum(e => e.Amount) <= 0 ? 0 : SAMIorderValues.SuppliesFour - SAMIOrderDetails.Where(e => e.ProductDetail.WTaxRate == "4").Sum(e => e.Amount);
            SAMIorderValues.TotalOrderValues = SAMIorderValues.TotalOrderValues - SAMIOrderDetails.Sum(e => e.Amount) <= 0 ? 0 : SAMIorderValues.TotalOrderValues - SAMIOrderDetails.Sum(e => e.Amount);
            SAMIorderValues.NetPayable = SAMIorderValues.NetPayable - SAMIOrderDetails.Sum(e => e.Amount) <= 0 ? 0 : SAMIorderValues.NetPayable - SAMIOrderDetails.Sum(e => e.Amount);
            orderValueList.Add(SAMIorderValues);

            var HealthtekOrderDetails = orderDetails.Where(e => e.ProductDetail.CompanyId == HealthTek).ToList();
            HealthtekorderValues.SuppliesZero = HealthtekorderValues.SuppliesZero - HealthtekOrderDetails.Where(e => e.ProductDetail.WTaxRate == "0").Sum(e => e.Amount) <= 0 ? 0 : HealthtekorderValues.SuppliesZero - HealthtekOrderDetails.Where(e => e.ProductDetail.WTaxRate == "0").Sum(e => e.Amount);
            HealthtekorderValues.SuppliesOne = HealthtekorderValues.SuppliesOne - HealthtekOrderDetails.Where(e => e.ProductDetail.WTaxRate == "1").Sum(e => e.Amount) <= 0 ? 0 : HealthtekorderValues.SuppliesOne - HealthtekOrderDetails.Where(e => e.ProductDetail.WTaxRate == "1").Sum(e => e.Amount);
            HealthtekorderValues.SuppliesFour = HealthtekorderValues.SuppliesFour - HealthtekOrderDetails.Where(e => e.ProductDetail.WTaxRate == "4").Sum(e => e.Amount) <= 0 ? 0 : HealthtekorderValues.SuppliesZero - HealthtekOrderDetails.Where(e => e.ProductDetail.WTaxRate == "4").Sum(e => e.Amount);
            HealthtekorderValues.TotalOrderValues = HealthtekorderValues.TotalOrderValues - HealthtekOrderDetails.Sum(e => e.Amount) <= 0 ? 0 : HealthtekorderValues.TotalOrderValues - HealthtekOrderDetails.Sum(e => e.Amount);
            HealthtekorderValues.NetPayable = HealthtekorderValues.NetPayable - HealthtekOrderDetails.Sum(e => e.Amount) <= 0 ? 0 : HealthtekorderValues.NetPayable - HealthtekOrderDetails.Sum(e => e.Amount);
            orderValueList.Add(HealthtekorderValues);

            var PhytekOrderDetails = orderDetails.Where(e => e.ProductDetail.CompanyId == Phytek).ToList();
            PhytekorderValues.SuppliesZero = PhytekorderValues.SuppliesZero - PhytekOrderDetails.Where(e => e.ProductDetail.WTaxRate == "0").Sum(e => e.Amount) <= 0 ? 0 : PhytekorderValues.SuppliesZero - PhytekOrderDetails.Where(e => e.ProductDetail.WTaxRate == "0").Sum(e => e.Amount);
            PhytekorderValues.SuppliesOne = PhytekorderValues.SuppliesOne - PhytekOrderDetails.Where(e => e.ProductDetail.WTaxRate == "1").Sum(e => e.Amount) <= 0 ? 0 : PhytekorderValues.SuppliesOne - PhytekOrderDetails.Where(e => e.ProductDetail.WTaxRate == "1").Sum(e => e.Amount);
            PhytekorderValues.SuppliesFour = PhytekorderValues.SuppliesFour - PhytekOrderDetails.Where(e => e.ProductDetail.WTaxRate == "4").Sum(e => e.Amount) <= 0 ? 0 : PhytekorderValues.SuppliesFour - PhytekOrderDetails.Where(e => e.ProductDetail.WTaxRate == "4").Sum(e => e.Amount);
            PhytekorderValues.TotalOrderValues = PhytekorderValues.TotalOrderValues - PhytekOrderDetails.Sum(e => e.Amount) <= 0 ? 0 : PhytekorderValues.TotalOrderValues - PhytekOrderDetails.Sum(e => e.Amount);
            PhytekorderValues.NetPayable = PhytekorderValues.NetPayable - PhytekOrderDetails.Sum(e => e.Amount) <= 0 ? 0 : PhytekorderValues.NetPayable - PhytekOrderDetails.Sum(e => e.Amount);
            orderValueList.Add(PhytekorderValues);

            new OrderValueBLL(_unitOfWork).UpdateRange(orderValueList);
        }
    }
}
