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
    public class CityBLL
    {
        private IUnitOfWork _unitOfWork;
        private IGenericRepository<City> repository;
        public CityBLL(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            repository = _unitOfWork.GenericRepository<City>();
        }
        public bool AddCity(City module)
        {
            module.CityName.Trim();
            module.CreatedBy = SessionHelper.LoginUser.Id;
            module.IsDeleted = false;
            module.CreatedDate = DateTime.Now;
            repository.Insert(module);
            return _unitOfWork.Save() > 0;
        }

        public bool UpdateCity(City module)
        {
            var item = repository.GetById(module.Id);
            item.CityName = module.CityName.Trim();
            item.SubRegionId = module.SubRegionId;
            item.IsActive = module.IsActive;
            repository.Update(item);
            return _unitOfWork.Save() > 0;
        }

        public bool DeleteCity(int id)
        {
            var item = repository.GetById(id);
            item.IsDeleted = true;
            repository.Delete(item);
            return _unitOfWork.Save() > 0;
        }

        public City GetCityById(int id)
        {
            return repository.GetById(id);
        }

        public List<City> GetAllCity()
        {
            return repository.GetAllList().Where(x => x.IsDeleted == false).ToList();
        }

        public bool CheckActionName(string ActionName)
        {

            var model = repository.GetAllList().ToList().Where(x => x.IsDeleted == false && x.CityName == ActionName.Trim()).FirstOrDefault();
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
            var selectList = GetAllCity().Where(x => x.IsActive == true).Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = x.CityName.Trim()
            });

            return new MultiSelectList(selectList, "Value", "Text", selectedValues: SelectedValue);
        }
    }
}
