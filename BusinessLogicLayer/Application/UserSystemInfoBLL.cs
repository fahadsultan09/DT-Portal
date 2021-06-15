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
    public class UserSystemInfoBLL
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGenericRepository<UserSystemInfo> _repository;
        public UserSystemInfoBLL(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _repository = _unitOfWork.GenericRepository<UserSystemInfo>();
        }
        public void Add(UserSystemInfo module)
        {
            if (CheckMACAddress(module.Id, module.MACAddress))
            {
                module.Id = 0;
                module.MACAddress = module.MACAddress;
                module.IsDeleted = false;
                module.IsActive = module.IsActive;
                module.CreatedBy = SessionHelper.LoginUser == null ? 1 : SessionHelper.LoginUser.Id;
                module.CreatedDate = DateTime.Now;
                _repository.Insert(module);
            }
            else
            {
                module.IsDeleted = false;
                _repository.Update(module);
            }
            _unitOfWork.Save();
        }
        public void Update(UserSystemInfo module)
        {
            var item = _repository.GetById(module.Id);
            item.ProcessorId = module.ProcessorId;
            item.HostName = module.HostName;
            item.IsActive = module.IsActive;
            item.UpdatedBy = SessionHelper.LoginUser.Id;
            item.UpdatedDate = DateTime.Now;
            _repository.Update(item);
            _unitOfWork.Save();
        }
        public void Delete(int id)
        {
            var item = _repository.GetById(id);
            item.IsDeleted = true;
            item.DeletedBy = SessionHelper.LoginUser.Id;
            item.DeletedDate = DateTime.Now;
            _repository.Delete(item);
        }
        public void DeleteRange(List<UserSystemInfo> list)
        {
            _repository.DeleteRange(list);
        }
        public UserSystemInfo GetById(int id)
        {
            return _repository.GetById(id);
        }
        public List<UserSystemInfo> GetAllUserSystemInfo()
        {
            return _repository.GetAllList().Where(x => x.IsDeleted == false).ToList();
        }
        public bool Check(int Id, string MACAddress)
        {
            int? UserSystemInfoId = Id == 0 ? null : (int?)Id;
            var model = _repository.GetAllList().ToList().Where(x => x.IsDeleted == false && x.Id != UserSystemInfoId || (UserSystemInfoId == null && x.Id == null)).FirstOrDefault();
            if (model != null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        public bool CheckMACAddress(int Id, string MACAddress)
        {
            int? id = Id == 0 ? null : (int?)Id;
            var model = _repository.GetAllList().ToList().Where(x => x.IsDeleted == false && x.MACAddress == MACAddress.Trim() && x.Id != id || (id == null && x.Id == null)).FirstOrDefault();

            if (model != null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        public UserSystemInfo FirstOrDefault(Expression<Func<UserSystemInfo, bool>> predicate)
        {
            return _repository.FirstOrDefault(predicate);
        }
        public List<UserSystemInfo> Where(Expression<Func<UserSystemInfo, bool>> predicate)
        {
            return _repository.Where(predicate);
        }
    }
}
