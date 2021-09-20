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
        public OrderDetailBLL(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _repository = _unitOfWork.GenericRepository<OrderDetail>();
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
            List<SAPOrderStatus> list = _repository.Where(x => x.SAPOrderNumber != null && x.OrderProductStatus != OrderStatus.CompletelyProcessed).Select(x => new SAPOrderStatus { SAPOrderNo = x.SAPOrderNumber }).DistinctBy(x => x.SAPOrderNo).ToList();
            return list;
        }
        public void UpdateProductOrderStatus(List<SAPOrderStatus> SAPOrderStatusList)
        {
            List<OrderDetail> list = new List<OrderDetail>();
            foreach (var item in SAPOrderStatusList)
            {
                List<OrderDetail> OrderDetailList = _repository.Where(x => x.SAPOrderNumber == item.SAPOrderNo).ToList();
                OrderDetailList.ForEach(x => x.OrderProductStatus = item.OrderStatus == "B" ? OrderStatus.PartiallyProcessed : (item.OrderStatus == "C" ? OrderStatus.CompletelyProcessed : OrderStatus.InProcess));
                list.AddRange(OrderDetailList.ToList());
            }
            _repository.UpdateRange(list);
            _unitOfWork.Save();
        }
    }
}
