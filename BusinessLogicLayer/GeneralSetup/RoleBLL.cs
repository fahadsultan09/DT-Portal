using BusinessLogicLayer.HelperClasses;
using DataAccessLayer.Repository;
using DataAccessLayer.WorkProcess;
using Microsoft.AspNetCore.Mvc.Rendering;
using Models.UserRights;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BusinessLogicLayer.GeneralSetup
{
    public class RoleBLL
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGenericRepository<Role> repository;
        public RoleBLL(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            repository = unitOfWork.GenericRepository<Role>();
        }

        public bool AddRole(Role module)
        {
            module.RoleName.Trim();
            module.Description.Trim();
            module.CreatedBy = SessionHelper.LoginUser.Id;
            module.IsDeleted = false;
            module.CreatedDate = DateTime.Now;
            repository.Insert(module);
            return _unitOfWork.Save() > 0;
        }

        public bool UpdateRole(Role module)
        {
            var item = repository.GetById(module.Id);
            item.RoleName = module.RoleName.Trim();
            item.Description = module.Description.Trim();
            item.IsActive = module.IsActive;
            item.UpdatedBy = SessionHelper.LoginUser.Id;
            item.UpdatedDate = DateTime.Now;
            repository.Update(item);
            return _unitOfWork.Save() > 0;
        }

        public bool DeleteRole(int id)
        {
            var item = repository.GetById(id);
            item.IsDeleted = true;
            item.DeletedBy = SessionHelper.LoginUser.Id;
            item.DeletedDate = DateTime.Now;
            repository.Delete(item);
            return _unitOfWork.Save() > 0;
        }

        public Role GetRoleById(int id)
        {
            return repository.GetById(id);
        }

        public List<Role> GetAllRole()
        {
            return repository.GetAllList().Where(x=> x.IsDeleted == false).ToList();
        }

        public bool CheckRoleName(int Id, string RoleName)
        {
            long? RoleId = Id == 0 ? null : (int?)Id;
            var model = repository.GetAllList().ToList().Where(x => x.IsDeleted == false && x.RoleName == RoleName.Trim() && x.Id != RoleId || (RoleId == null && x.Id == null)).FirstOrDefault();
            if (model != null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public SelectList DropDownRoleList(long SelectedValue)
        {
            var selectList = GetAllRole().Where(x => x.IsActive == true).Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = x.RoleName.ToString()
            });

            return new SelectList(selectList, "Value", "Text", SelectedValue);
        }
    }
}
