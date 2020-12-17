using BusinessLogicLayer.HelperClasses;
using DataAccessLayer.Repository;
using DataAccessLayer.WorkProcess;
using Microsoft.AspNetCore.Mvc.Rendering;
using Models.UserRights;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BusinessLogicLayer.GeneralSetup
{
    public class ApplicationModuleBLL
    {
        private IUnitOfWork _unitOfWork;
        private IGenericRepository<ApplicationModule> repository;
        public ApplicationModuleBLL(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            repository = _unitOfWork.GenericRepository<ApplicationModule>();
        }
        public bool AddApplicationModule(ApplicationModule module)
        {
            module.ModuleIcon.Trim();
            module.ModuleName.Trim();
            module.CreatedBy = SessionHelper.LoginUser.Id;
            module.IsDeleted = false;
            module.CreatedDate = DateTime.Now;
            repository.Insert(module);
            return _unitOfWork.Save() > 0;
        }

        public bool UpdateApplicationModule(ApplicationModule module)
        {
            var item = repository.GetById(module.Id);
            item.ModuleName = module.ModuleName.Trim();
            item.ModuleIcon = module.ModuleIcon.Trim();
            item.IsActive = module.IsActive;
            item.UpdatedBy = SessionHelper.LoginUser.Id;
            item.UpdatedDate = DateTime.Now;
            repository.Update(item);
            return _unitOfWork.Save() > 0;
        }

        public bool DeleteApplicationModule(int id)
        {
            var item = repository.GetById(id);
            item.IsDeleted = true;
            repository.Delete(item);
            return _unitOfWork.Save() > 0;
        }

        public ApplicationModule GetApplicationModuleById(int id)
        {            
            return repository.GetById(id);
        }

        public List<ApplicationModule> GetAllApplicationModule()
        {
            return repository.GetAllList().Where(x => x.IsDeleted == false).ToList();
        }

        public bool CheckApplicationModuleName(int Id, string ModuleName)
        {
            int? DosageFormId = Id == 0 ? null : (int?)Id;
            var model = _unitOfWork.GenericRepository<ApplicationModule>().GetAllList().ToList().Where(x => x.IsDeleted == false && x.ModuleName == ModuleName && x.Id != DosageFormId || (DosageFormId == null && x.Id == null)).FirstOrDefault();
            if (model != null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public SelectList DropDownApplicationModuleList(int SelectedValue)
        {
            var selectList = GetAllApplicationModule().Where(x => x.IsActive == true).Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = x.ModuleName.ToString()
            });

            return new SelectList(selectList, "Value", "Text", SelectedValue);
        }
    }
}
