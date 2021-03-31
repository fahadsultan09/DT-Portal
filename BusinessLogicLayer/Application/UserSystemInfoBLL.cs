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
        private readonly IGenericRepository<UserSystemInfo> repository;
        public UserSystemInfoBLL(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            repository = _unitOfWork.GenericRepository<UserSystemInfo>();
        }
        public bool Add(UserSystemInfo module)
        {
            //module.IsActive = true;
            var item = repository.FirstOrDefault(x => x.MACAddress == module.MACAddress);
            if (item is null)
            {
                module.IsDeleted = false;
                module.CreatedBy = SessionHelper.LoginUser == null ? 1 : SessionHelper.LoginUser.Id;
                module.CreatedDate = DateTime.Now;
                repository.Insert(module);
                return _unitOfWork.Save() > 0;
            }
            else
            {
                return false;
            }
        }
        public bool Update(UserSystemInfo module)
        {
            var item = repository.GetById(module.Id);
            item.ProcessorId = module.ProcessorId;
            item.HostName = module.HostName;
            item.MACAddress = module.MACAddress;
            item.OtherMACAddress = module.OtherMACAddress;
            item.UpdatedBy = SessionHelper.LoginUser.Id;
            item.UpdatedDate = DateTime.Now;
            repository.Update(item);
            return _unitOfWork.Save() > 0;
        }
        public bool Delete(int id)
        {
            var item = repository.GetById(id);
            item.IsDeleted = true;
            item.DeletedBy = SessionHelper.LoginUser.Id;
            item.DeletedDate = DateTime.Now;
            repository.Delete(item);
            return _unitOfWork.Save() > 0;
        }
        public UserSystemInfo GetById(int id)
        {
            return repository.GetById(id);
        }
        public List<UserSystemInfo> GetAllUserSystemInfo()
        {
            return repository.GetAllList().Where(x => x.IsDeleted == false).ToList();
        }
        public bool Check(int Id, string MACAddress)
        {
            int? UserSystemInfoId = Id == 0 ? null : (int?)Id;
            var model = _unitOfWork.GenericRepository<UserSystemInfo>().GetAllList().ToList().Where(x => x.IsDeleted == false && x.MACAddress == MACAddress && x.Id != UserSystemInfoId || (UserSystemInfoId == null && x.Id == null)).FirstOrDefault();
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
            return repository.FirstOrDefault(predicate);
        }
        public List<UserSystemInfo> Where(Expression<Func<UserSystemInfo, bool>> predicate)
        {
            return repository.Where(predicate);
        }
    }
}
