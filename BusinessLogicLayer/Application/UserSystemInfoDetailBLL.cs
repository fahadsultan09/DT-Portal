using BusinessLogicLayer.HelperClasses;
using DataAccessLayer.Repository;
using DataAccessLayer.WorkProcess;
using Models.Application;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace BusinessLogicLayer.Application
{
    public class UserSystemInfoDetailBLL
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGenericRepository<UserSystemInfoDetail> repository;
        public UserSystemInfoDetailBLL(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            repository = _unitOfWork.GenericRepository<UserSystemInfoDetail>();
        }
        public void Add(UserSystemInfoDetail module)
        {
            module.CreatedBy = SessionHelper.LoginUser == null ? 1 : SessionHelper.LoginUser.Id;
            module.CreatedDate = DateTime.Now;
            repository.Insert(module);
            _unitOfWork.Save();
        }
        public UserSystemInfoDetail GetById(int id)
        {
            return repository.GetById(id);
        }
        public List<UserSystemInfoDetail> GetAll()
        {
            return repository.GetAllList().ToList();
        }
        public bool Check(int Id, string MACAddress)
        {
            int? UserSystemInfoDetailId = Id == 0 ? null : (int?)Id;
            var model = _unitOfWork.GenericRepository<UserSystemInfoDetail>().GetAllList().ToList().Where(x => x.MACAddress == MACAddress && x.Id != UserSystemInfoDetailId || (UserSystemInfoDetailId == null && x.Id == null)).FirstOrDefault();
            if (model != null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        public UserSystemInfoDetail FirstOrDefault(Expression<Func<UserSystemInfoDetail, bool>> predicate)
        {
            return repository.FirstOrDefault(predicate);
        }
        public List<UserSystemInfoDetail> Where(Expression<Func<UserSystemInfoDetail, bool>> predicate)
        {
            return repository.Where(predicate);
        }
    }
}
