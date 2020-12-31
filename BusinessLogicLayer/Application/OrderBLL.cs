using BusinessLogicLayer.HelperClasses;
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
    public class OrderBLL
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGenericRepository<OrderMaster> _repository;
        public OrderBLL(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _repository = _unitOfWork.GenericRepository<OrderMaster>();
        }
        public int Add(OrderMaster module)
        {
            module.CreatedBy = SessionHelper.LoginUser.Id;
            module.IsDeleted = false;
            module.CreatedDate = DateTime.Now;
            _unitOfWork.GenericRepository<OrderMaster>().Insert(module);
            return _unitOfWork.Save();
        }
        public int Update(OrderMaster module)
        {
            var item = _unitOfWork.GenericRepository<OrderMaster>().GetById(module.Id);
            item.IsActive = module.IsActive;
            item.UpdatedBy = SessionHelper.LoginUser.Id;
            item.UpdatedDate = DateTime.Now;
            _unitOfWork.GenericRepository<OrderMaster>().Update(item);
            return _unitOfWork.Save();
        }
        public int Delete(int id)
        {
            var item = _unitOfWork.GenericRepository<OrderMaster>().GetById(id);
            item.IsDeleted = true;
            _unitOfWork.GenericRepository<OrderMaster>().Delete(item);
            return _unitOfWork.Save();
        }
        public OrderMaster GetOrderMasterById(int id)
        {
            return _unitOfWork.GenericRepository<OrderMaster>().GetById(id);
        }
        public List<OrderMaster> GetAllOrderMaster()
        {
            return _unitOfWork.GenericRepository<OrderMaster>().GetAllList().Where(x => x.IsDeleted == false).ToList();
        }
        public List<OrderMaster> Where(Expression<Func<OrderMaster, bool>> predicate)
        {
            return _repository.Where(predicate);
        }
        public OrderMaster FirstOrDefault(Expression<Func<OrderMaster, bool>> predicate)
        {
            return _repository.FirstOrDefault(predicate);
        }
    }
}
