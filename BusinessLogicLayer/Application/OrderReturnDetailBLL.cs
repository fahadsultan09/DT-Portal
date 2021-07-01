using BusinessLogicLayer.HelperClasses;
using DataAccessLayer.Repository;
using DataAccessLayer.WorkProcess;
using Models.Application;
using Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Utility;

namespace BusinessLogicLayer.Application
{
    public class OrderReturnDetailBLL
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGenericRepository<OrderReturnDetail> _repository;
        public OrderReturnDetailBLL(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _repository = _unitOfWork.GenericRepository<OrderReturnDetail>();
        }
        public int Add(OrderReturnDetail module)
        {
            module.CreatedBy = SessionHelper.LoginUser.Id;
            module.CreatedDate = DateTime.Now;
            _repository.Insert(module);
            return _unitOfWork.Save();
        }
        public int AddRange(List<OrderReturnDetail> module)
        {
            _repository.AddRange(module);
            return _unitOfWork.Save();
        }
        public int DeleteRange(List<OrderReturnDetail> module)
        {
            _repository.DeleteRange(module);
            return _unitOfWork.Save();
        }
        public int Update(OrderReturnDetail module)
        {
            var item = _repository.GetById(module.Id);
            _repository.Update(item);
            return _unitOfWork.Save();
        }
        public int UpdateRange(List<OrderReturnDetail> OrderReturnDetailList)
        {
            _repository.UpdateRange(OrderReturnDetailList);
            return _unitOfWork.Save();
        }
        public int Delete(int id)
        {
            var item = _repository.GetById(id);
            _repository.Delete(item);
            return _unitOfWork.Save();
        }
        public OrderReturnDetail GetOrderReturnDetailById(int id)
        {
            return _repository.GetById(id);
        }
        public List<OrderReturnDetail> GetOrderDetailByIdByMasterId(int OrderReturnId)
        {
            return _repository.Where(x => x.OrderReturnId == OrderReturnId).ToList();
        }
        public List<OrderReturnDetail> GetAllOrderReturnDetail()
        {
            return _repository.GetAllList().ToList();
        }
        public List<OrderReturnDetail> Where(Expression<Func<OrderReturnDetail, bool>> predicate)
        {
            return _repository.Where(predicate);
        }
        public OrderReturnDetail FirstOrDefault(Expression<Func<OrderReturnDetail, bool>> predicate)
        {
            return _repository.FirstOrDefault(predicate);
        }
        public List<SAPOrderStatus> GetInProcessOrderReturnStatus()
        {
            List<SAPOrderStatus> list = _repository.GetAllList().Where(x => x.ReturnOrderNumber != null && x.ReturnOrderStatus != OrderStatus.CompletelyProcessed).Select(x => new SAPOrderStatus { SAPOrderNo = x.ReturnOrderNumber }).Distinct().ToList();
            return list;
        }
        public void UpdateProductOrderStatus(List<SAPOrderStatus> SAPOrderStatusList)
        {
            List<OrderReturnDetail> list = new List<OrderReturnDetail>();
            foreach (var item in SAPOrderStatusList)
            {
                List<OrderReturnDetail> OrderReturnDetailList = _repository.Where(x => x.ReturnOrderNumber == item.SAPOrderNo).ToList();
                OrderReturnDetailList.ForEach(x => x.ReturnOrderStatus = item.OrderStatus == "B" ? OrderStatus.PartiallyProcessed : (item.OrderStatus == "C" ? OrderStatus.CompletelyProcessed : OrderStatus.InProcess));
                list.AddRange(OrderReturnDetailList.ToList());
            }
            _repository.UpdateRange(list);
            _unitOfWork.Save();
        }
    }
}
