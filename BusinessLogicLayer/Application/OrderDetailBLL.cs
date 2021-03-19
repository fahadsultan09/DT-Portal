﻿using BusinessLogicLayer.HelperClasses;
using DataAccessLayer.Repository;
using DataAccessLayer.WorkProcess;
using Models.Application;
using Models.ViewModel;
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
            _unitOfWork.GenericRepository<OrderDetail>().Insert(module);
            return _unitOfWork.Save();
        }

        public int AddRange(List<OrderDetail> module)
        {
            _unitOfWork.GenericRepository<OrderDetail>().AddRange(module);
            return _unitOfWork.Save();
        }

        public int DeleteRange(List<OrderDetail> module)
        {
            _unitOfWork.GenericRepository<OrderDetail>().DeleteRange(module);
            return _unitOfWork.Save();
        }
        public int Update(OrderDetail module)
        {
            var item = _unitOfWork.GenericRepository<OrderDetail>().GetById(module.Id);
            _unitOfWork.GenericRepository<OrderDetail>().Update(item);
            return _unitOfWork.Save();
        }
        public int Delete(int id)
        {
            var item = _unitOfWork.GenericRepository<OrderDetail>().GetById(id);
            _unitOfWork.GenericRepository<OrderDetail>().Delete(item);
            return _unitOfWork.Save();
        }
        public OrderDetail GetOrderDetailById(int id)
        {
            return _unitOfWork.GenericRepository<OrderDetail>().GetById(id);
        }
        public List<OrderDetail> GetOrderDetailByIdByMasterId(int OrderId)
        {
            return _repository.Where(x => x.OrderId == OrderId).ToList();
        }
        public List<OrderDetail> GetAllOrderDetail()
        {
            return _unitOfWork.GenericRepository<OrderDetail>().GetAllList().ToList();
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
            List<SAPOrderStatus> list = _repository.Where(x => x.SAPOrderNumber != null && x.OrderProductStatus != OrderStatus.CompletelyProcessed).Select(x => new SAPOrderStatus { SAPOrderNo = x.SAPOrderNumber }).ToList();
            return list;
        }
        public void UpdateProductOrderStatus(List<SAPOrderStatus> SAPOrderStatusList)
        {
            foreach (var item in SAPOrderStatusList)
            {
                var model = _repository.Where(x=>x.SAPOrderNumber == item.SAPOrderNo).First();
                model.OrderProductStatus = item.OrderStatus == "B" ? OrderStatus.PartiallyProcessed : (item.OrderStatus == "C" ? OrderStatus.CompletelyProcessed : OrderStatus.InProcess);
                _repository.Update(model);
                _unitOfWork.Save();
            }
        }
    }
}
