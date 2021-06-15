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
    public class ComplaintCategoryBLL
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGenericRepository<ComplaintCategory> _repository;
        public ComplaintCategoryBLL(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _repository = _unitOfWork.GenericRepository<ComplaintCategory>();
        }
        public int AddComplaintCategory(ComplaintCategory module)
        {
            module.CreatedBy = SessionHelper.LoginUser.Id;
            module.IsDeleted = false;
            module.CreatedDate = DateTime.Now;
            _repository.Insert(module);
            return _unitOfWork.Save();
        }
        public int UpdateComplaintCategory(ComplaintCategory module)
        {
            var item = _repository.GetById(module.Id);
            item.ComplaintCategoryName = module.ComplaintCategoryName;
            item.IsActive = module.IsActive;
            item.UpdatedBy = SessionHelper.LoginUser.Id;
            item.UpdatedDate = DateTime.Now;
            _repository.Update(item);
            return _unitOfWork.Save();
        }
        public int DeleteComplaintCategory(int id)
        {
            var item = _repository.GetById(id);
            item.IsDeleted = false;
            _repository.Delete(item);
            return _unitOfWork.Save();
        }
        public ComplaintCategory GetComplaintCategoryById(int id)
        {
            return _repository.GetById(id);
        }
        public List<ComplaintCategory> GetAllComplaintCategory()
        {
            return _repository.GetAllList().Where(x => x.IsDeleted == false).ToList();
        }
        public bool CheckComplaintCategoryName(int Id, string ComplaintCategoryName)
        {
            int? ComplaintCategoryId = Id == 0 ? null : (int?)Id;
            var model = _repository.GetAllList().ToList().Where(x => x.IsDeleted == false && x.ComplaintCategoryName == ComplaintCategoryName && x.Id != ComplaintCategoryId || (ComplaintCategoryId == null && x.Id == null)).FirstOrDefault();
            if (model != null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        public SelectList DropDownComplaintCategoryByUserId(int[] ComplaintCategory)
        {
            var selectList = GetAllComplaintCategory().Where(x => x.IsActive == true && ComplaintCategory.Contains(x.Id)).Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = x.ToString()
            });

            return new SelectList(selectList, "Value", "Text");
        }
        public SelectList DropDownComplaintCategoryList(int SelectedValue)
        {
            var selectList = GetAllComplaintCategory().Where(x => x.IsActive == true).Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = x.ComplaintCategoryName.ToString()
            });

            return new SelectList(selectList, "Value", "Text", SelectedValue);
        }
        public MultiSelectList DropDownComplaintCategoryMultiList(int[] SelectedValue)
        {
            var selectList = GetAllComplaintCategory().Where(x => x.IsActive == true).Select(x => new
            {
                Value = x.Id.ToString(),
                Text = x.ComplaintCategoryName.ToString()
            });

            return new MultiSelectList(selectList, "Value", "Text", selectedValues: SelectedValue);
        }
        public List<ComplaintCategory> Where(Expression<Func<ComplaintCategory, bool>> predicate)
        {
            return _repository.Where(predicate);
        }
        public ComplaintCategory FirstOrDefault(Expression<Func<ComplaintCategory, bool>> predicate)
        {
            return _repository.FirstOrDefault(predicate);
        }
    }
}
