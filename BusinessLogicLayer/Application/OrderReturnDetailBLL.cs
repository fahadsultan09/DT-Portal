﻿using BusinessLogicLayer.HelperClasses;
using DataAccessLayer.Repository;
using DataAccessLayer.WorkProcess;
using Models.Application;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

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
            _unitOfWork.GenericRepository<OrderReturnDetail>().Insert(module);
            return _unitOfWork.Save();
        }
        public int AddRange(List<OrderReturnDetail> module)
        {
            _unitOfWork.GenericRepository<OrderReturnDetail>().AddRange(module);
            return _unitOfWork.Save();
        }
        public int DeleteRange(List<OrderReturnDetail> module)
        {
            _unitOfWork.GenericRepository<OrderReturnDetail>().DeleteRange(module);
            return _unitOfWork.Save();
        }
        public int Update(OrderReturnDetail module)
        {
            var item = _unitOfWork.GenericRepository<OrderReturnDetail>().GetById(module.Id);
            _unitOfWork.GenericRepository<OrderReturnDetail>().Update(item);
            return _unitOfWork.Save();
        }
        public int Delete(int id)
        {
            var item = _unitOfWork.GenericRepository<OrderReturnDetail>().GetById(id);
            _unitOfWork.GenericRepository<OrderReturnDetail>().Delete(item);
            return _unitOfWork.Save();
        }
        public OrderReturnDetail GetOrderReturnDetailById(int id)
        {
            return _unitOfWork.GenericRepository<OrderReturnDetail>().GetById(id);
        }
        public List<OrderReturnDetail> GetOrderDetailByIdByMasterId(int OrderReturnId)
        {
            return _repository.Where(x => x.OrderReturnId == OrderReturnId).ToList();
        }
        public List<OrderReturnDetail> GetAllOrderReturnDetail()
        {
            return _unitOfWork.GenericRepository<OrderReturnDetail>().GetAllList().ToList();
        }
        public List<OrderReturnDetail> Where(Expression<Func<OrderReturnDetail, bool>> predicate)
        {
            return _repository.Where(predicate);
        }
        public OrderReturnDetail FirstOrDefault(Expression<Func<OrderReturnDetail, bool>> predicate)
        {
            return _repository.FirstOrDefault(predicate);
        }
    }
}
