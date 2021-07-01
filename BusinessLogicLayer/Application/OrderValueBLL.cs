using BusinessLogicLayer.HelperClasses;
using DataAccessLayer.Repository;
using DataAccessLayer.WorkProcess;
using Microsoft.AspNetCore.Mvc.Rendering;
using Models.Application;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BusinessLogicLayer.Application
{
    public class OrderValueBLL
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGenericRepository<OrderValue> repository;
        public OrderValueBLL(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            repository = _unitOfWork.GenericRepository<OrderValue>();
        }
        public bool AddOrderValue(OrderValue module)
        {
            module.CreatedBy = SessionHelper.LoginUser.Id;
            module.CreatedDate = DateTime.Now;
            repository.Insert(module);
            return _unitOfWork.Save() > 0;
        }
        public bool UpdateOrderValue(OrderValue module)
        {
            var item = repository.GetById(module.Id);
            item.SuppliesZero = module.SuppliesZero;
            item.SuppliesOne = module.SuppliesOne;
            item.SuppliesFour = module.SuppliesFour;
            item.TotalOrderValues = module.TotalOrderValues;
            item.UnConfirmedPayment = module.UnConfirmedPayment;
            item.PendingOrderValues = module.PendingOrderValues;
            item.CurrentBalance = module.CurrentBalance;
            item.NetPayable = module.NetPayable;
            repository.Update(item);
            return _unitOfWork.Save() > 0;
        }
        public OrderValue GetOrderValueById(int id)
        {
            return repository.GetById(id);
        }
        public List<OrderValue> GetOrderValueByCompanyId(int CompanyId)
        {
            return repository.Where(e => e.CompanyId == CompanyId).ToList();
        }
        public List<OrderValue> GetOrderValueByOrderId(int OrderId)
        {
            return repository.Where(e => e.OrderId == OrderId).ToList();
        }  
    }
}
