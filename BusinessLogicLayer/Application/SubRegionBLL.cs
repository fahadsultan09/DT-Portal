using BusinessLogicLayer.HelperClasses;
using DataAccessLayer.Repository;
using DataAccessLayer.WorkProcess;
using Microsoft.AspNetCore.Mvc.Rendering;
using Models.Application;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BusinessLogicLayer.Application
{
    public class SubRegionBLL
    {
        private IUnitOfWork _unitOfWork;
        private IGenericRepository<SubRegion> repository;
        public SubRegionBLL(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            repository = _unitOfWork.GenericRepository<SubRegion>();
        }
        public bool AddSubRegion(SubRegion module)
        {
            module.SubRegionName.Trim();
            module.CreatedBy = SessionHelper.LoginUser.Id;
            module.IsDeleted = false;
            module.CreatedDate = DateTime.Now;
            repository.Insert(module);
            return _unitOfWork.Save() > 0;
        }
        public bool UpdateSubRegion(SubRegion module)
        {
            var item = repository.GetById(module.Id);
            item.SubRegionName = module.SubRegionName.Trim();
            item.RegionId = module.RegionId;
            item.IsActive = module.IsActive;
            repository.Update(item);
            return _unitOfWork.Save() > 0;
        }
        public bool DeleteSubRegion(int id)
        {
            var item = repository.GetById(id);
            item.IsDeleted = true;
            repository.Delete(item);
            return _unitOfWork.Save() > 0;
        }
        public SubRegion GetSubRegionById(int id)
        {
            return repository.GetById(id);
        }
        public List<SubRegion> GetAllSubRegion()
        {
            return repository.GetAllList().Where(x => x.IsDeleted == false).ToList();
        }
        public bool CheckSubRegionName(int Id, string SubRegionName)
        {
            int? DosageFormId = Id == 0 ? null : (int?)Id;
            var model = _unitOfWork.GenericRepository<SubRegion>().GetAllList().ToList().Where(x => x.IsDeleted == false && x.SubRegionName == SubRegionName && x.Id != DosageFormId || (DosageFormId == null && x.Id == null)).FirstOrDefault();
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
            var selectList = GetAllSubRegion().Where(x => x.IsActive == true).Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = x.SubRegionName.Trim()
            });

            return new MultiSelectList(selectList, "Value", "Text", selectedValues: SelectedValue);
        }
        public SelectList DropDownSubRegionList(int SelectedValue)
        {
            var selectList = GetAllSubRegion().Where(x => x.IsActive == true).Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = x.SubRegionName.ToString()
            });

            return new SelectList(selectList, "Value", "Text", SelectedValue);
        }
        public SelectList DropDownSubRegionList(int? RegionId, int? SelectedValue)
        {
            var selectList = GetAllSubRegion().Where(x => x.IsActive == true && x.RegionId == RegionId).Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = x.SubRegionName.ToString()
            });

            return new SelectList(selectList, "Value", "Text", SelectedValue);
        }
    }
}
