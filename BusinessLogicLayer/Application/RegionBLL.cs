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
        private IUnitOfWork _unitOfWork;
        private IGenericRepository<Region> repository;
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

        public bool CheckActionName(string ActionName)
        {

            var model = repository.GetAllList().ToList().Where(x => x.IsDeleted == false && x.RegionName == ActionName.Trim()).FirstOrDefault();
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
            var selectList = GetAllRegion().Where(x => x.IsActive == true).Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = x.RegionName.Trim()
            });

            return new MultiSelectList(selectList, "Value", "Text", selectedValues: SelectedValue);
        }
    }
}
