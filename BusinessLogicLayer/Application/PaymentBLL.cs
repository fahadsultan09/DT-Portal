using BusinessLogicLayer.HelperClasses;
using DataAccessLayer.WorkProcess;
using Models.Application;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BusinessLogicLayer.Application
{
    public class PaymentBLL
    {
        private readonly IUnitOfWork _unitOfWork;
        public PaymentBLL(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public int Add(PaymentMaster module)
        {
            module.CreatedBy = SessionHelper.LoginUser.Id;
            module.IsDeleted = false;
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
            _unitOfWork.GenericRepository<PaymentMaster>().Delete(item);
            return _unitOfWork.Save();
        }

        public PaymentMaster GetPaymentMasterById(int id)
        {
            return _unitOfWork.GenericRepository<PaymentMaster>().GetById(id);
        }

        public List<PaymentMaster> GetAllPaymentMaster()
        {
            return _unitOfWork.GenericRepository<PaymentMaster>().GetAllList().Where(x => x.IsDeleted == false).ToList();
        }
    }
}
