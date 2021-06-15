using BusinessLogicLayer.HelperClasses;
using DataAccessLayer.Repository;
using DataAccessLayer.WorkProcess;
using Microsoft.AspNetCore.Mvc.Rendering;
using Models.Application;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace BusinessLogicLayer.GeneralSetup
{
    public class ComplaintSubCategoryBLL
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGenericRepository<ComplaintSubCategory> _repository;
        public ComplaintSubCategoryBLL(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _repository = _unitOfWork.GenericRepository<ComplaintSubCategory>();
        }
        public int AddComplaintSubCategory(ComplaintSubCategory module)
        {
            module.IsActive = true;
            module.IsDeleted = false;
            module.CreatedBy = SessionHelper.LoginUser.Id;
            module.CreatedDate = DateTime.Now;
            _repository.Insert(module);
            return _unitOfWork.Save();
        }
        public int UpdateComplaintSubCategory(ComplaintSubCategory module)
        {
            var item = _repository.GetById(module.Id);
            item.ComplaintSubCategoryName = module.ComplaintSubCategoryName;
            item.UserEmailTo = module.UserEmailTo;
            item.KPIDay = module.KPIDay;
            item.IsActive = module.IsActive;
            item.UpdatedBy = SessionHelper.LoginUser.Id;
            item.UpdatedDate = DateTime.Now;
            _repository.Update(item);
            return _unitOfWork.Save();
        }
        public int DeleteComplaintSubCategory(int id)
        {
            var item = _repository.GetById(id);
            item.IsDeleted = false;
            _repository.Delete(item);
            return _unitOfWork.Save();
        }
        public ComplaintSubCategory GetComplaintSubCategoryById(int id)
        {
            return _repository.GetById(id);
        }
        public List<ComplaintSubCategory> GetAllComplaintSubCategory()
        {
            return _repository.GetAllList().Where(x => x.IsDeleted == false).ToList();
        }
        public List<ComplaintSubCategory> Where(Expression<Func<ComplaintSubCategory, bool>> predicate)
        {
            return _repository.Where(predicate);
        }
        public ComplaintSubCategory FirstOrDefault(Expression<Func<ComplaintSubCategory, bool>> predicate)
        {
            return _repository.FirstOrDefault(predicate);
        }
        public bool CheckComplaintSubCategoryName(int Id, string ComplaintSubCategoryName)
        {
            int? ComplaintSubCategoryId = Id == 0 ? null : (int?)Id;
            var model = _repository.GetAllList().ToList().Where(x => x.IsDeleted == false && x.ComplaintSubCategoryName == ComplaintSubCategoryName && x.Id != ComplaintSubCategoryId || (ComplaintSubCategoryId == null && x.Id == null)).FirstOrDefault();
            if (model != null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        public SelectList DropDownComplaintSubCategoryByUserId(int[] ComplaintSubCategory)
        {
            var selectList = GetAllComplaintSubCategory().Where(x => x.IsActive == true && ComplaintSubCategory.Contains(x.Id)).Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = x.ToString()
            });

            return new SelectList(selectList, "Value", "Text");
        }
        public SelectList DropDownComplaintSubCategoryList(int SelectedValue)
        {
            var selectList = GetAllComplaintSubCategory().Where(x => x.IsActive == true).Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = x.ComplaintSubCategoryName.ToString()
            });

            return new SelectList(selectList, "Value", "Text", SelectedValue);
        }
        public SelectList DropDownComplaintSubCategoryList(int? ComplaintCategoryId, int? SelectedValue)
        {
            var selectList = GetAllComplaintSubCategory().Where(x => x.IsActive == true && x.ComplaintCategoryId == ComplaintCategoryId).Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = x.ComplaintSubCategoryName.ToString()
            });

            return new SelectList(selectList, "Value", "Text", SelectedValue);
        }
        public MultiSelectList DropDownComplaintSubCategoryMultiList(int[] SelectedValue)
        {
            var selectList = GetAllComplaintSubCategory().Where(x => x.IsActive == true).Select(x => new
            {
                Value = x.Id.ToString(),
                Text = x.ComplaintSubCategoryName.ToString()
            });

            return new MultiSelectList(selectList, "Value", "Text", selectedValues: SelectedValue);
        }
    }
}
