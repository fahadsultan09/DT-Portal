using BusinessLogicLayer.HelperClasses;
using DataAccessLayer.WorkProcess;
using Microsoft.AspNetCore.Mvc.Rendering;
using Models.Application;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BusinessLogicLayer.GeneralSetup
{
    public class ComplaintCategoryBLL
    {
        private readonly IUnitOfWork _unitOfWork;
        public ComplaintCategoryBLL(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public int AddComplaintCategory(ComplaintCategory module)
        {
            module.CreatedBy = SessionHelper.LoginUser.Id;
            module.IsDeleted = false;
            module.CreatedDate = DateTime.Now;
            _unitOfWork.GenericRepository<ComplaintCategory>().Insert(module);
            return _unitOfWork.Save();
        }

        public int UpdateComplaintCategory(ComplaintCategory module)
        {
            var item = _unitOfWork.GenericRepository<ComplaintCategory>().GetById(module.Id);
            item.ComplaintCategoryName = module.ComplaintCategoryName;
            item.IsActive = module.IsActive;
            item.UpdatedBy = SessionHelper.LoginUser.Id;
            item.UpdatedDate = DateTime.Now;
            _unitOfWork.GenericRepository<ComplaintCategory>().Update(item);
            return _unitOfWork.Save();
        }

        public int DeleteComplaintCategory(int id)
        {
            var item = _unitOfWork.GenericRepository<ComplaintCategory>().GetById(id);
            item.IsDeleted = true;
            _unitOfWork.GenericRepository<ComplaintCategory>().Delete(item);
            return _unitOfWork.Save();
        }

        public ComplaintCategory GetComplaintCategoryById(int id)
        {
            return _unitOfWork.GenericRepository<ComplaintCategory>().GetById(id);
        }

        public List<ComplaintCategory> GetAllComplaintCategory()
        {
            return _unitOfWork.GenericRepository<ComplaintCategory>().GetAllList().Where(x => x.IsDeleted == false).ToList();
        }

        public bool CheckComplaintCategoryName(int Id, string ComplaintCategoryName)
        {
            int? ComplaintCategoryId = Id == 0 ? null : (int?)Id;
            var model = _unitOfWork.GenericRepository<ComplaintCategory>().GetAllList().ToList().Where(x => x.IsDeleted == false && x.ComplaintCategoryName == ComplaintCategoryName && x.Id != ComplaintCategoryId || (ComplaintCategoryId == null && x.Id == null)).FirstOrDefault();
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
    }
}
