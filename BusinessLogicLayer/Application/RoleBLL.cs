using BusinessLogicLayer.HelperClasses;
using DataAccessLayer.WorkProcess;
using Microsoft.AspNetCore.Mvc.Rendering;
using Models.UserRights;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BusinessLogicLayer.ApplicationSetup
{
    public class RoleBLL
    {
        private readonly IUnitOfWork _unitOfWork;
        public RoleBLL(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public int AddRole(Role module)
        {
            module.CreatedBy = SessionHelper.LoginUser.Id;
            module.IsDeleted = false;
            module.CreatedDate = DateTime.Now;
            _unitOfWork.GenericRepository<Role>().Insert(module);
            return _unitOfWork.Save();
        }

        public int UpdateRole(Role module)
        {
            var item = _unitOfWork.GenericRepository<Role>().GetById(module.Id);
            item.RoleName = module.RoleName;
            item.IsActive = module.IsActive;
            item.UpdatedBy = SessionHelper.LoginUser.Id;
            item.UpdatedDate = DateTime.Now;
            _unitOfWork.GenericRepository<Role>().Update(item);
            return _unitOfWork.Save();
        }

        public int DeleteRole(int id)
        {
            var item = _unitOfWork.GenericRepository<Role>().GetById(id);
            item.IsDeleted = true;
            _unitOfWork.GenericRepository<Role>().Delete(item);
            return _unitOfWork.Save();
        }

        public Role GetRoleById(int id)
        {
            return _unitOfWork.GenericRepository<Role>().GetById(id);
        }

        public List<Role> GetAllRole()
        {
            return _unitOfWork.GenericRepository<Role>().GetAllList().Where(x => x.IsDeleted == false).ToList();
        }

        public bool CheckRoleName(int Id, string RoleName)
        {
            int? RoleId = Id == 0 ? null : (int?)Id;
            var model = _unitOfWork.GenericRepository<Role>().GetAllList().ToList().Where(x => x.IsDeleted == false && x.RoleName == RoleName && x.Id != RoleId || (RoleId == null && x.Id == null)).FirstOrDefault();
            if (model != null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public SelectList DropDownRoleByUserId(int[] Role)
        {
            var selectList = GetAllRole().Where(x => x.IsActive == true && Role.Contains(x.Id)).Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = x.RoleName.ToString()
            });

            return new SelectList(selectList, "Value", "Text");
        }


        public SelectList DropDownRoleList(int SelectedValue)
        {
            var selectList = GetAllRole().Where(x => x.IsActive == true).Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = x.RoleName.ToString()
            });

            return new SelectList(selectList, "Value", "Text", SelectedValue);
        }

        public MultiSelectList DropDownRoleMultiList(int[] SelectedValue)
        {
            var selectList = GetAllRole().Where(x => x.IsActive == true).Select(x => new
            {
                Value = x.Id.ToString(),
                Text = x.RoleName.ToString()
            });

            return new MultiSelectList(selectList, "Value", "Text", selectedValues: SelectedValue);
        }
    }
}
