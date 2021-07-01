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
    public class ApplicationPageBLL
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGenericRepository<ApplicationPage> repository;
        public ApplicationPageBLL(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            repository = unitOfWork.GenericRepository<ApplicationPage>();
        }

        public bool AddApplicationPage(ApplicationPage module)
        {
            module.ControllerName.Trim();
            module.PageTitle.Trim();
            module.PageURL.Trim();
            module.PageIcon.Trim();
            module.CreatedBy = SessionHelper.LoginUser.Id;
            module.IsDeleted = false;
            module.CreatedDate = DateTime.Now;
            repository.Insert(module);
            return _unitOfWork.Save() > 0;
        }

        public bool UpdateApplicationPage(ApplicationPage module)
        {
            var item = repository.GetById(module.Id);
            item.ApplicationModuleId = module.ApplicationModuleId;
            item.ApplicationActionsId = module.ApplicationActionsId;
            item.ControllerName = module.ControllerName.Trim();
            item.PageTitle = module.PageTitle.Trim();
            item.PageURL = module.PageURL.Trim();
            item.PageIcon = module.PageIcon.Trim();
            item.Sort = module.Sort;
            item.IsShowOnNavigation = module.IsShowOnNavigation;
            item.IsActive = module.IsActive;
            item.UpdatedBy = SessionHelper.LoginUser.Id;
            item.UpdatedDate = DateTime.Now;
            repository.Update(item);
            return _unitOfWork.Save() > 0;
        }

        public bool DeleteApplicationPage(int id)
        {
            var item = repository.GetById(id);            
            item.IsDeleted = true;
            item.IsActive = false;
            item.DeletedBy = SessionHelper.LoginUser.Id;
            item.DeletedDate = DateTime.Now;
            repository.Delete(item);
            return _unitOfWork.Save() > 0;
        }

        public ApplicationPage GetApplicationPageById(int id)
        {
            return repository.GetById(id);
        }

        public List<ApplicationPage> GetAllApplicationPage()
        {
            return repository.GetAllList().Where(x => x.IsDeleted == false).ToList();
        }

        public bool CheckApplicationPageName(long Id, string PageTitle)
        {
            long? ApplicationPageId = Id == 0 ? null : (long?)Id;
            var model = repository.GetAllList().ToList().Where(x => x.IsDeleted == false && x.PageTitle == PageTitle.Trim() && x.Id != ApplicationPageId || (ApplicationPageId == null && x.Id == null)).FirstOrDefault();
            if (model != null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public SelectList DropDownPageTitleList(long ApplicationModuleId, long SelectedValue = 0)
        {
            var selectList = GetAllApplicationPage().Where(x => x.IsActive == true && x.ApplicationModuleId == ApplicationModuleId).Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = x.PageTitle.Trim().ToString()
            });

            return new SelectList(selectList, "Value", "Text", SelectedValue);
        }
    }
}
