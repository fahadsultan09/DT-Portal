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
    public class ApplicationActionBLL
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGenericRepository<ApplicationAction> repository;
        public ApplicationActionBLL(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            repository = _unitOfWork.GenericRepository<ApplicationAction>();
        }
        public bool AddApplicationAction(ApplicationAction module)
        {
            module.ActionName.Trim();
            module.CreatedBy = SessionHelper.LoginUser.Id;
            module.IsDeleted = false;
            module.CreatedDate = DateTime.Now;
            repository.Insert(module);
            return _unitOfWork.Save() > 0;
        }
        public bool UpdateApplicationAction(ApplicationAction module)
        {
            var item = repository.GetById(module.Id);
            item.ActionName = module.ActionName.Trim();
            item.IsActive = module.IsActive;
            item.UpdatedBy = SessionHelper.LoginUser.Id;
            item.UpdatedDate = DateTime.Now;
            repository.Update(item);
            return _unitOfWork.Save() > 0;
        }
        public bool DeleteApplicationAction(int id)
        {
            var item = repository.GetById(id);
            item.IsDeleted = false;
            item.CreatedBy = SessionHelper.LoginUser.Id;
            item.CreatedDate = DateTime.Now; 
            repository.Delete(item);
            return _unitOfWork.Save() > 0;
        }
        public ApplicationAction GetApplicationActionById(int id)
        {
            return repository.GetById(id);
        }

        public List<ApplicationAction> GetAllApplicationAction()
        {
            return repository.GetAllList().Where(x => x.IsDeleted == false).ToList();
        }
        public bool CheckApplicationActionName(int Id, string ModuleName)
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
        public MultiSelectList DropDownActionNameList(int[] SelectedValue)
        {
            var selectList = GetAllApplicationAction().Where(x => x.IsActive == true).Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = x.ActionName.Trim()
            });

            return new MultiSelectList(selectList, "Value", "Text", selectedValues: SelectedValue);
        }
    }
}
