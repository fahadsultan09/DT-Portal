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
    public class RegionBLL
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGenericRepository<Region> repository;
        public RegionBLL(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            repository = _unitOfWork.GenericRepository<Region>();
        }
        public bool AddRegion(Region module)
        {
            module.RegionName.Trim();
            module.CreatedBy = SessionHelper.LoginUser.Id;
            module.IsDeleted = false;
            module.CreatedDate = DateTime.Now;
            repository.Insert(module);
            return _unitOfWork.Save() > 0;
        }
        public bool UpdateRegion(Region module)
        {
            var item = repository.GetById(module.Id);
            item.RegionName = module.RegionName.Trim();
            item.IsActive = module.IsActive;
            repository.Update(item);
            return _unitOfWork.Save() > 0;
        }
        public bool DeleteRegion(int id)
        {
            var item = repository.GetById(id);
            item.IsDeleted = true;
            repository.Delete(item);
            return _unitOfWork.Save() > 0;
        }
        public Region GetRegionById(int id)
        {
            return repository.GetById(id);
        }
        public List<Region> GetAllRegion()
        {
            return repository.GetAllList().Where(x => x.IsDeleted == false).ToList();
        }

        public bool CheckRegionName(int Id, string RegionName)
        {
            int? DosageFormId = Id == 0 ? null : (int?)Id;
            var model = _unitOfWork.GenericRepository<Region>().GetAllList().ToList().Where(x => x.IsDeleted == false && x.RegionName == RegionName && x.Id != DosageFormId || (DosageFormId == null && x.Id == null)).FirstOrDefault();
            if (model != null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        public SelectList DropDownRegionList(int SelectedValue)
        {
            var selectList = GetAllRegion().Where(x => x.IsActive == true).Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = x.RegionName.ToString()
            });

            return new SelectList(selectList, "Value", "Text", SelectedValue);
        }
    }
}
