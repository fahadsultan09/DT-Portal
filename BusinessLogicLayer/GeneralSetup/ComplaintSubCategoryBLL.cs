using BusinessLogicLayer.HelperClasses;
using DataAccessLayer.WorkProcess;
using Microsoft.AspNetCore.Mvc.Rendering;
using Models.Application;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BusinessLogicLayer.GeneralSetup
{
    public class ComplaintSubCategoryBLL
    {
        private readonly IUnitOfWork _unitOfWork;
        public ComplaintSubCategoryBLL(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public int AddComplaintSubCategory(ComplaintSubCategory module)
        {
            module.CreatedBy = SessionHelper.LoginUser.Id;
            module.IsDeleted = false;
            module.CreatedDate = DateTime.Now;
            _unitOfWork.GenericRepository<ComplaintSubCategory>().Insert(module);
            return _unitOfWork.Save();
        }

        public int UpdateComplaintSubCategory(ComplaintSubCategory module)
        {
            var item = _unitOfWork.GenericRepository<ComplaintSubCategory>().GetById(module.Id);
            item.ComplaintSubCategoryName = module.ComplaintSubCategoryName;
            item.IsActive = module.IsActive;
            item.UpdatedBy = SessionHelper.LoginUser.Id;
            item.UpdatedDate = DateTime.Now;
            _unitOfWork.GenericRepository<ComplaintSubCategory>().Update(item);
            return _unitOfWork.Save();
        }

        public int DeleteComplaintSubCategory(int id)
        {
            var item = _unitOfWork.GenericRepository<ComplaintSubCategory>().GetById(id);
            item.IsDeleted = true;
            _unitOfWork.GenericRepository<ComplaintSubCategory>().Delete(item);
            return _unitOfWork.Save();
        }

        public ComplaintSubCategory GetComplaintSubCategoryById(int id)
        {
            return _unitOfWork.GenericRepository<ComplaintSubCategory>().GetById(id);
        }

        public List<ComplaintSubCategory> GetAllComplaintSubCategory()
        {
            return _unitOfWork.GenericRepository<ComplaintSubCategory>().GetAllList().Where(x => x.IsDeleted == false).ToList();
        }

        public bool CheckComplaintSubCategoryName(int Id, string ComplaintSubCategoryName)
        {
            int? ComplaintSubCategoryId = Id == 0 ? null : (int?)Id;
            var model = _unitOfWork.GenericRepository<ComplaintSubCategory>().GetAllList().ToList().Where(x => x.IsDeleted == false && x.ComplaintSubCategoryName == ComplaintSubCategoryName && x.Id != ComplaintSubCategoryId || (ComplaintSubCategoryId == null && x.Id == null)).FirstOrDefault();
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
