using BusinessLogicLayer.HelperClasses;
using DataAccessLayer.Repository;
using DataAccessLayer.WorkProcess;
using Microsoft.AspNetCore.Mvc.Rendering;
using Models.Application;
using System;
using System.Collections.Generic;
using System.Linq;
using Utility.HelperClasses;

namespace BusinessLogicLayer.Application
{
    public class UserBLL
    {
        private IUnitOfWork _unitOfWork;
        private IGenericRepository<User> repository;
        public UserBLL(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            repository = _unitOfWork.GenericRepository<User>();
        }
        public bool AddUser(User module)
        {
            module.UserName.Trim();
            module.FirstName.Trim();
            module.LastName.Trim();
            module.Email.Trim();
            module.DistributorId = module.IsDistributor ? module.DistributorId : null;
            module.CreatedBy = SessionHelper.LoginUser.Id;
            module.IsDeleted = false;
            module.CreatedBy = SessionHelper.LoginUser.Id;
            module.CreatedDate = DateTime.Now;
            repository.Insert(module);
            return _unitOfWork.Save() > 0;
        }

        public bool UpdateUser(User module)
        {
            var item = repository.GetById(module.Id);
            item.UserName = module.UserName.Trim();
            item.FirstName = module.FirstName.Trim();
            item.LastName = module.LastName.Trim();
            item.Email = module.Email.Trim();
            item.DistributorId = item.IsDistributor ? module.DistributorId : null;
            item.IsDistributor = item.IsDistributor;
            item.Password = EncryptDecrypt.Encrypt(module.Password);
            item.RoleId = module.RoleId;
            item.IsActive = module.IsActive;
            item.UpdatedBy = SessionHelper.LoginUser.Id;
            item.UpdatedDate = DateTime.Now;
            repository.Update(item);
            return _unitOfWork.Save() > 0;
        }

        public bool DeleteUser(int id)
        {
            var item = repository.GetById(id);
            item.IsDeleted = true;
            item.DeletedBy = SessionHelper.LoginUser.Id;
            item.DeletedDate = DateTime.Now;
            repository.Delete(item);
            return _unitOfWork.Save() > 0;
        }

        public User GetUserById(int id)
        {
            return repository.GetById(id);
        }

        public List<User> GetAllUser()
        {
            return repository.GetAllList().Where(x => x.IsDeleted == false).ToList();
        }

        public bool CheckActionName(string ActionName)
        {

            var model = repository.GetAllList().ToList().Where(x => x.IsDeleted == false && x.UserName == ActionName.Trim()).FirstOrDefault();
            if (model != null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public MultiSelectList DropDownActionNameList(int[] SelectedValue)
        {
            var selectList = GetAllUser().Where(x => x.IsActive == true).Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = x.UserName.Trim()
            });

            return new MultiSelectList(selectList, "Value", "Text", selectedValues: SelectedValue);
        }
    }
}
