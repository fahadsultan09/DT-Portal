using BusinessLogicLayer.HelperClasses;
using DataAccessLayer.Repository;
using DataAccessLayer.WorkProcess;
using Microsoft.AspNetCore.Mvc.Rendering;
using Models.Application;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace BusinessLogicLayer.Application
{
    public class UserBLL
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGenericRepository<User> repository;
        private readonly SubDistributorBLL _SubDistributorBLL;

        public UserBLL(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            repository = _unitOfWork.GenericRepository<User>();
            _SubDistributorBLL = new SubDistributorBLL(_unitOfWork);
        }
        public bool AddUser(User module)
        {
            List<SubDistributor> SubDistributorList = new List<SubDistributor>();
            module.MobileNumber = module.MobileNumber.Replace("-", "");
            module.UserName.Trim();
            module.FirstName.Trim();
            module.LastName.Trim();
            module.DistributorId = module.IsDistributor ? module.DistributorId : null;
            module.IsActive = module.IsActive;
            module.IsDeleted = false;
            module.CreatedBy = SessionHelper.LoginUser.Id;
            module.CreatedDate = DateTime.Now;
            repository.Insert(module);

            if (_SubDistributorBLL.Where(x => x.UserId == module.Id).Count() > 0)
            {
                _SubDistributorBLL.HardDeleteRange(_SubDistributorBLL.Where(x => x.UserId == module.Id).ToList());
            }
            if (module.SubDistributorIds != null && module.SubDistributorIds.Count() > 0)
            {
                foreach (var item1 in module.SubDistributorIds.ToArray())
                {
                    SubDistributor subDistributor = new SubDistributor();
                    subDistributor.UserId = module.Id;
                    subDistributor.SubDistributorId = item1;
                    subDistributor.IsActive = module.IsActive;
                    subDistributor.IsDeleted = false;
                    subDistributor.CreatedBy = SessionHelper.LoginUser.Id;
                    subDistributor.CreatedDate = DateTime.Now;
                    SubDistributorList.Add(subDistributor);
                }
                _SubDistributorBLL.AddRange(SubDistributorList);
            }
            return _unitOfWork.Save() > 0;
        }
        public bool UpdateUser(User module)
        {
            List<SubDistributor> SubDistributorList = new List<SubDistributor>();
            var item = repository.GetById(module.Id);
            item.AccessToken = module.AccessToken;
            item.UserName = module.UserName.Trim();
            item.FirstName = module.FirstName.Trim();
            item.LastName = module.LastName.Trim();
            item.Email = module.Email.Trim();
            item.DistributorId = module.IsDistributor ? module.DistributorId : null;
            item.IsDistributor = module.IsDistributor;
            item.RoleId = module.RoleId;
            item.PlantLocationId = module.PlantLocationId;
            item.CompanyId = module.CompanyId;
            item.RegisteredAddress = module.RegisteredAddress;
            item.IsStoreKeeper = module.IsStoreKeeper;
            item.EmailIntimationId = module.EmailIntimationId;
            item.DesignationId = module.DesignationId;
            item.CityId = module.CityId;
            item.AccessToken = module.AccessToken;
            item.IsActive = module.IsActive;
            item.UpdatedBy = SessionHelper.LoginUser == null ? 1 : SessionHelper.LoginUser.Id;
            item.UpdatedDate = DateTime.Now;
            repository.Update(item);

            if (_SubDistributorBLL.Where(x => x.UserId == module.Id).Count() > 0)
            {
                _SubDistributorBLL.HardDeleteRange(_SubDistributorBLL.Where(x => x.UserId == module.Id).ToList());
            }
            if (module.SubDistributorIds != null && module.SubDistributorIds.Count() > 0)
            {
                foreach (var item1 in module.SubDistributorIds.ToArray())
                {
                    SubDistributor subDistributor = new SubDistributor();
                    subDistributor.UserId = module.Id;
                    subDistributor.SubDistributorId = item1;
                    subDistributor.IsActive = module.IsActive;
                    subDistributor.IsDeleted = false;
                    subDistributor.CreatedBy = SessionHelper.LoginUser.Id;
                    subDistributor.CreatedDate = DateTime.Now;
                    SubDistributorList.Add(subDistributor);
                }
                _SubDistributorBLL.AddRange(SubDistributorList);
            }
            return _unitOfWork.Save() > 0;
        }
        public bool UpdateAccessToken(User module)
        {
            var item = repository.GetById(module.Id);
            item.AccessToken = module.AccessToken;
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
        public bool ResetPassword(int id, string password)
        {
            var item = repository.GetById(id);
            item.Password = password;
            item.UpdatedBy = id;
            item.UpdatedDate = DateTime.Now;
            repository.Update(item);
            return _unitOfWork.Save() > 0;
        }
        public User GetUserById(int id)
        {
            return repository.GetById(id);
        }
        public List<User> GetAllUser()
        {
            return repository.GetAllList().Where(x => !x.IsDeleted).ToList();
        }
        public List<User> GetAllActiveUser()
        {
            return repository.GetAllList().Where(x => x.IsActive && !x.IsDeleted).ToList();
        }
        public List<User> GetUsers()
        {
            return repository.GetAllList().ToList();
        }
        public bool CheckUserName(int Id, string UserName)
        {
            int? DistributorId = Id == 0 ? null : (int?)Id;
            var model = _unitOfWork.GenericRepository<User>().GetAllList().ToList().Where(x => x.IsDeleted == false && x.UserName == UserName && x.Id != DistributorId || (DistributorId == null && x.Id == null)).FirstOrDefault();
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
        public SelectList DropDownUserList(int? SelectedValue)
        {
            var selectList = GetAllUser().Where(x => !x.IsDeleted && x.IsActive && !x.IsDistributor).Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = x.Email.Trim()
            });

            return new SelectList(selectList, "Value", "Text", SelectedValue);
        }
        public MultiSelectList DropDownUserList(int[] SelectedValue)
        {
            var selectList = GetAllUser().Where(x => x.IsActive == true).Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = x.Email.Trim()
            });

            return new MultiSelectList(selectList, "Value", "Text", selectedValues: SelectedValue);
        }
        public User FirstOrDefault(Expression<Func<User, bool>> predicate)
        {
            return repository.FirstOrDefault(predicate);
        }
        public List<User> Where(Expression<Func<User, bool>> predicate)
        {
            return repository.Where(predicate);
        }
    }
}
