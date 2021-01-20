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
    public class LicenseControlBLL
    {
        private IUnitOfWork _unitOfWork;
        private IGenericRepository<LicenseControl> repository;
        public LicenseControlBLL(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            repository = _unitOfWork.GenericRepository<LicenseControl>();
        }
        public bool AddLicenseControl(LicenseControl module)
        {
            module.LicenseName.Trim();            
            module.CreatedBy = SessionHelper.LoginUser.Id;
            module.IsDeleted = false;
            module.CreatedDate = DateTime.Now;
            repository.Insert(module);
            return _unitOfWork.Save() > 0;
        }
        public bool UpdateLicenseControl(LicenseControl module)
        {
            var item = repository.GetById(module.Id);
            item.LicenseName = module.LicenseName.Trim();
            item.LicenseAcceptanceInDay = module.LicenseAcceptanceInDay;
            item.IsMandatory = module.IsMandatory;
            item.IsActive = module.IsActive;
            item.IsDeleted = module.IsDeleted;
            repository.Update(item);
            return _unitOfWork.Save() > 0;
        }
        public bool DeleteLicenseControl(int id)
        {
            var item = repository.GetById(id);
            item.IsDeleted = true;
            repository.Delete(item);
            return _unitOfWork.Save() > 0;
        }
        public LicenseControl GetLicenseControlById(int id)
        {
            return repository.GetById(id);
        }
        public List<LicenseControl> GetAllLicenseControl()
        {
            return repository.GetAllList().Where(x => x.IsDeleted == false).ToList();
        }

        public bool CheckLicenseControlName(int Id, string LicenseControlName)
        {
            int? DosageFormId = Id == 0 ? null : (int?)Id;
            var model = repository.GetAllList().ToList().Where(x => x.IsDeleted == false && x.LicenseName == LicenseControlName).FirstOrDefault();
            if (model != null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        public SelectList DropDownLicenseControlList(int SelectedValue)
        {
            var selectList = GetAllLicenseControl().Where(x => x.IsActive == true).Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = x.LicenseName.ToString()
            });

            return new SelectList(selectList, "Value", "Text", SelectedValue);
        }
    }
}
