using BusinessLogicLayer.HelperClasses;
using DataAccessLayer.Repository;
using DataAccessLayer.WorkProcess;
using Models.Application;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Utility;

namespace BusinessLogicLayer.Application
{
    public class OrderValueBLL
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGenericRepository<OrderValue> _repository;
        private readonly AuditTrailBLL<OrderValue> _AuditTrail_OrderValue;
        public OrderValueBLL(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _repository = _unitOfWork.GenericRepository<OrderValue>();
            _AuditTrail_OrderValue = new AuditTrailBLL<OrderValue>(_unitOfWork);
        }
        public bool AddOrderValue(OrderValue module)
        {
            module.CreatedBy = SessionHelper.LoginUser.Id;
            module.CreatedDate = DateTime.Now;
            _repository.Insert(module);
            return _unitOfWork.Save() > 0;
        }
        public bool UpdateOrderValue(OrderValue module)
        {
            var item = _repository.GetById(module.Id);
            item.SuppliesZero = module.SuppliesZero;
            item.SuppliesOne = module.SuppliesOne;
            item.SuppliesFour = module.SuppliesFour;
            item.TotalOrderValues = module.TotalOrderValues;
            item.UnConfirmedPayment = module.UnConfirmedPayment;
            item.PendingOrderValues = module.PendingOrderValues;
            item.CurrentBalance = module.CurrentBalance;
            item.NetPayable = module.NetPayable;
            _repository.Update(item);
            return _unitOfWork.Save() > 0;
        }
        public bool UpdateRange(List<OrderValue> module)
        {
            _repository.UpdateRange(module);

            foreach (var item in module)
            {
                _AuditTrail_OrderValue.AddAuditTrail((int)ApplicationPages.Order, (int)ApplicationActions.Update, module, "Save Order Value", item.CreatedBy);
            }
            return _unitOfWork.Save() > 0;
        }
        public OrderValue GetOrderValueById(int id)
        {
            return _repository.GetById(id);
        }
        public List<OrderValue> GetOrderValueByCompanyId(int CompanyId)
        {
            return _repository.Where(e => e.CompanyId == CompanyId).ToList();
        }
        public List<OrderValue> GetOrderValueByOrderId(int OrderId)
        {
            return _repository.Where(e => e.OrderId == OrderId).ToList();
        }
        public List<OrderValue> Where(Expression<Func<OrderValue, bool>> expression)
        {
            return _repository.Where(expression);
        }
        public OrderValue FirstOrDefault(Expression<Func<OrderValue, bool>> predicate)
        {
            return _repository.FirstOrDefault(predicate);
        }

    }
}
