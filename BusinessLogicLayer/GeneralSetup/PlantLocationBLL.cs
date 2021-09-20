using BusinessLogicLayer.HelperClasses;
using DataAccessLayer.Repository;
using DataAccessLayer.WorkProcess;
using Microsoft.AspNetCore.Mvc.Rendering;
using Models.Application;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BusinessLogicLayer.GeneralSetup
{
    public class PlantLocationBLL
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGenericRepository<PlantLocation> repository;
        public PlantLocationBLL(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            repository = _unitOfWork.GenericRepository<PlantLocation>();
        }
        public bool AddPlantLocation(PlantLocation module)
        {
            module.PlantLocationName.Trim();
            module.CreatedBy = SessionHelper.LoginUser.Id;
            module.IsDeleted = false;
            module.CreatedDate = DateTime.Now;
            repository.Insert(module);
            return _unitOfWork.Save() > 0;
        }
        public bool UpdatePlantLocation(PlantLocation module)
        {
            var item = repository.GetById(module.Id);
            item.PlantLocationName = module.PlantLocationName.Trim();
            item.CCEmail = module.CCEmail;
            item.IsActive = module.IsActive;
            item.UpdatedBy = SessionHelper.LoginUser.Id;
            item.UpdatedDate = DateTime.Now;
            repository.Update(item);
            return _unitOfWork.Save() > 0;
        }
        public bool DeletePlantLocation(int id)
        {
            var item = repository.GetById(id);
            item.IsDeleted = true;
            repository.Delete(item);
            return _unitOfWork.Save() > 0;
        }
        public PlantLocation GetPlantLocationById(int id)
        {
            return repository.GetById(id);
        }
        public List<PlantLocation> GetAllPlantLocation()
        {
            return repository.GetAllList().Where(x => x.IsDeleted == false).ToList();
        }
        public bool CheckPlantLocationName(int Id, string ModuleName)
        {
            int? DosageFormId = Id == 0 ? null : (int?)Id;
            var model = _unitOfWork.GenericRepository<PlantLocation>().GetAllList().ToList().Where(x => x.IsDeleted == false && x.PlantLocationName == ModuleName && x.Id != DosageFormId || (DosageFormId == null && x.Id == null)).FirstOrDefault();
            if (model != null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        public SelectList DropDownPlantLocationList(int SelectedValue)
        {
            var selectList = GetAllPlantLocation().Where(x => x.IsActive == true).Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = x.PlantLocationName.ToString()
            });

            return new SelectList(selectList, "Value", "Text", SelectedValue);
        }
        public SelectList DropDownPlantLocationList()
        {
            var selectList = GetAllPlantLocation().Where(x => x.IsActive == true).Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = x.PlantLocationName.ToString()
            });

            return new SelectList(selectList, "Value", "Text");
        }
    }
}
