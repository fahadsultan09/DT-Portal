using BusinessLogicLayer.HelperClasses;
using DataAccessLayer.Repository;
using DataAccessLayer.WorkProcess;
using Models.UserRights;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace BusinessLogicLayer.GeneralSetup
{
    public class RolePermissionBLL
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGenericRepository<RolePermission> repository;
        public RolePermissionBLL(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            repository = unitOfWork.GenericRepository<RolePermission>();
        }

        public int AddRolePermission(RolePermission module)
        {
            module.CreatedBy = SessionHelper.LoginUser.Id;
            module.CreatedDate = DateTime.Now;
            repository.Insert(module);
            return _unitOfWork.Save();
        }

        public int UpdateRolePermission(int id, RolePermission module)
        {
            module.Id = id;
            repository.Update(module);
            return _unitOfWork.Save();
        }

        public int DeleteRolePermission(int id)
        {
            var item = _unitOfWork.GenericRepository<RolePermission>().GetById(id);
            repository.Delete(item);
            return _unitOfWork.Save();
        }

        public RolePermission GetRolePermissionById(int id)
        {
            return repository.GetById(id);
        }

        public List<RolePermission> GetAllRolePermission()
        {
            return repository.GetAllList().ToList();
        }

        public List<RolePermission> Where(Expression<Func<RolePermission, bool>> predicate)
        {
            return repository.Where(predicate);
        }

        public int DeleteRange(List<RolePermission> list)
        {
            repository.DeleteRange(list);
            return _unitOfWork.Save();
        }
        public int AddRange(List<RolePermission> list)
        {
            repository.AddRange(list);
            return _unitOfWork.Save();
        }
    }
}
