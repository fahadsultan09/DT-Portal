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
    public class PaymentBLL
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGenericRepository<PaymentMaster> _repository;
        public PaymentBLL(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _repository = _unitOfWork.GenericRepository<PaymentMaster>();
        }
        public int Add(PaymentMaster module)
        {
            module.CreatedBy = SessionHelper.LoginUser.Id;
            module.IsDeleted = false;
            module.IsActive = true;
            module.CreatedDate = DateTime.Now;
            _unitOfWork.GenericRepository<PaymentMaster>().Insert(module);
            return _unitOfWork.Save();
        }
        public int Update(PaymentMaster module)
        {
            var item = _unitOfWork.GenericRepository<PaymentMaster>().GetById(module.Id);
            item.IsActive = module.IsActive;
            item.UpdatedBy = SessionHelper.LoginUser.Id;
            item.UpdatedDate = DateTime.Now;
            _unitOfWork.GenericRepository<PaymentMaster>().Update(item);
            return _unitOfWork.Save();
        }
        public int Delete(int id)
        {
            var item = _unitOfWork.GenericRepository<PaymentMaster>().GetById(id);
            item.IsDeleted = true;
            return _unitOfWork.Save();
        }

        public void UpdateStatus(PaymentMaster model, PaymentStatus paymentStatus)
        {
            model.Status = paymentStatus;
            model.UpdatedBy = SessionHelper.LoginUser.Id;
            model.UpdatedDate = DateTime.Now;
            _repository.Update(model);
        }
        public PaymentMaster GetById(int id)
        {
            return _unitOfWork.GenericRepository<PaymentMaster>().GetById(id);
        }
        public List<PaymentMaster> GetAllPaymentMaster()
        {
            return _unitOfWork.GenericRepository<PaymentMaster>().GetAllList().Where(x => x.IsDeleted == false).OrderByDescending(x => x.Id).ToList();
        }
        public List<PaymentMaster> Where(Expression<Func<PaymentMaster, bool>> predicate)
        {
            return _repository.Where(predicate);
        }
        public PaymentMaster FirstOrDefault(Expression<Func<PaymentMaster, bool>> predicate)
        {
            return _repository.FirstOrDefault(predicate);
        }
    }
}
