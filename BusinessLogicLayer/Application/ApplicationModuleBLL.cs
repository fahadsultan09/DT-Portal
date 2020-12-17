using BusinessLogicLayer.HelperClasses;
using DataAccessLayer.WorkProcess;
using Microsoft.AspNetCore.Mvc.Rendering;
using Models.UserRights;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BusinessLogicLayer.ApplicationSetup
{
    public class ApplicationModuleBLL
    {
        private readonly IUnitOfWork _unitOfWork;
        public ApplicationModuleBLL(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public int AddApplicationModule(ApplicationModule module)
        {
            module.CreatedBy = SessionHelper.LoginUser.Id;
            module.IsDeleted = false;
            module.CreatedDate = DateTime.Now;
            _unitOfWork.GenericRepository<ApplicationModule>().Insert(module);
            return _unitOfWork.Save();
        }

        public int UpdateApplicationModule(ApplicationModule module)
        {
            var item = _unitOfWork.GenericRepository<ApplicationModule>().GetById(module.Id);
            item.ModuleName = module.ModuleName;
            item.ModuleIcon = module.ModuleIcon;
            item.IsActive = module.IsActive;
            item.UpdatedBy = SessionHelper.LoginUser.Id;
            item.UpdatedDate = DateTime.Now;
            _unitOfWork.GenericRepository<ApplicationModule>().Update(item);
            return _unitOfWork.Save();
        }

        public int DeleteApplicationModule(int id)
        {
            var item = _unitOfWork.GenericRepository<ApplicationModule>().GetById(id);
            item.IsDeleted = true;
            _unitOfWork.GenericRepository<ApplicationModule>().Delete(item);
            return _unitOfWork.Save();
        }

        public ApplicationModule GetApplicationModuleById(int id)
        {
            return _unitOfWork.GenericRepository<ApplicationModule>().GetById(id);
        }

        public List<ApplicationModule> GetAllApplicationModule()
        {
            return _unitOfWork.GenericRepository<ApplicationModule>().GetAllList().Where(x => x.IsDeleted == false).ToList();
        }

        public bool CheckApplicationModuleName(int Id, string ApplicationModuleName)
        {
            int? ApplicationModuleId = Id == 0 ? null : (int?)Id;
            var model = _unitOfWork.GenericRepository<ApplicationModule>().GetAllList().ToList().Where(x => x.IsDeleted == false && x.ModuleName == ApplicationModuleName && x.Id != ApplicationModuleId || (ApplicationModuleId == null && x.Id == null)).FirstOrDefault();
            if (model != null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public SelectList DropDownApplicationModuleByUserId(int[] ApplicationModule)
        {
            var selectList = GetAllApplicationModule().Where(x => x.IsActive == true && ApplicationModule.Contains(x.Id)).Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = x.ModuleName.ToString()
            });

            return new SelectList(selectList, "Value", "Text");
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

        public MultiSelectList DropDownApplicationModuleMultiList(int[] SelectedValue)
        {
            var selectList = GetAllApplicationModule().Where(x => x.IsActive == true).Select(x => new
            {
                Value = x.Id.ToString(),
                Text = x.ModuleName.ToString()
            });

            return new MultiSelectList(selectList, "Value", "Text", selectedValues: SelectedValue);
        }
    }
}
