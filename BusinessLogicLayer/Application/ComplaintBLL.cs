using BusinessLogicLayer.HelperClasses;
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
    public class ComplaintBLL
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGenericRepository<Complaint> _repository;
        public ComplaintBLL(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _repository = _unitOfWork.GenericRepository<Complaint>();
        }
        public int Add(Complaint module)
        {
            module.CreatedBy = SessionHelper.LoginUser.Id;
            module.IsDeleted = false;
            module.IsActive = true;
            module.CreatedDate = DateTime.Now;
            _unitOfWork.GenericRepository<Complaint>().Insert(module);
            return _unitOfWork.Save();
        }
        public int Update(Complaint module)
        {
            var item = _unitOfWork.GenericRepository<Complaint>().GetById(module.Id);
            item.IsActive = module.IsActive;
            item.UpdatedBy = SessionHelper.LoginUser.Id;
            item.UpdatedDate = DateTime.Now;
            _unitOfWork.GenericRepository<Complaint>().Update(item);
            return _unitOfWork.Save();
        }
        public int Delete(int id)
        {
            var item = _unitOfWork.GenericRepository<Complaint>().GetById(id);
            item.IsDeleted = true;
            return _unitOfWork.Save();
        }

        public void UpdateStatus(Complaint model, ComplaintStatus ComplaintStatus)
        {
            model.Status = ComplaintStatus;
            model.UpdatedBy = SessionHelper.LoginUser.Id;
            model.UpdatedDate = DateTime.Now;
            _repository.Update(model);
        }
        public Complaint GetById(int id)
        {
            return _unitOfWork.GenericRepository<Complaint>().GetById(id);
        }
        public List<Complaint> GetAllComplaint()
        {
            return _unitOfWork.GenericRepository<Complaint>().GetAllList().Where(x => x.IsDeleted == false).ToList();
        }
        public List<Complaint> Where(Expression<Func<Complaint, bool>> predicate)
        {
            return _repository.Where(predicate);
        }
        public Complaint FirstOrDefault(Expression<Func<Complaint, bool>> predicate)
        {
            return _repository.FirstOrDefault(predicate);
        }
    }
}
