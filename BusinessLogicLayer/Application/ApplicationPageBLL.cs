using BusinessLogicLayer.HelperClasses;
using DataAccessLayer.WorkProcess;
using Microsoft.AspNetCore.Mvc.Rendering;
using Models.UserRights;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BusinessLogicLayer.ApplicationSetup
{
    public class ApplicationPageBLL
    {
        private readonly IUnitOfWork _unitOfWork;
        public ApplicationPageBLL(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public int AddApplicationPage(ApplicationPage module)
        {
            module.CreatedBy = SessionHelper.LoginUser.Id;
            module.IsDeleted = false;
            module.CreatedDate = DateTime.Now;
            _unitOfWork.GenericRepository<ApplicationPage>().Insert(module);
            return _unitOfWork.Save();
        }

        public int UpdateApplicationPage(ApplicationPage module)
        {
            var item = _unitOfWork.GenericRepository<ApplicationPage>().GetById(module.Id);
            item.PageTitle = module.PageTitle;
            item.PageIcon = module.PageIcon;
            item.PageTitle = module.PageTitle;
            item.Sort = module.Sort;
            item.IsActive = module.IsActive;
            item.UpdatedBy = SessionHelper.LoginUser.Id;
            item.UpdatedDate = DateTime.Now;
            _unitOfWork.GenericRepository<ApplicationPage>().Update(item);
            return _unitOfWork.Save();
        }

        public int DeleteApplicationPage(int id)
        {
            var item = _unitOfWork.GenericRepository<ApplicationPage>().GetById(id);
            item.IsDeleted = true;
            _unitOfWork.GenericRepository<ApplicationPage>().Delete(item);
            return _unitOfWork.Save();
        }

        public ApplicationPage GetApplicationPageById(int id)
        {
            return _unitOfWork.GenericRepository<ApplicationPage>().GetById(id);
        }

        public List<ApplicationPage> GetAllApplicationPage()
        {
            return _unitOfWork.GenericRepository<ApplicationPage>().GetAllList().Where(x => x.IsDeleted == false).ToList();
        }

        public bool CheckApplicationPageName(int Id, string ApplicationPageName)
        {
            int? ApplicationPageId = Id == 0 ? null : (int?)Id;
            var model = _unitOfWork.GenericRepository<ApplicationPage>().GetAllList().ToList().Where(x => x.IsDeleted == false && x.PageTitle == ApplicationPageName && x.Id != ApplicationPageId || (ApplicationPageId == null && x.Id == null)).FirstOrDefault();
            if (model != null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public SelectList DropDownApplicationPageByUserId(int[] ApplicationPage)
        {
            var selectList = GetAllApplicationPage().Where(x => x.IsActive == true && ApplicationPage.Contains(x.Id)).Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = x.PageTitle.ToString()
            });

            return new SelectList(selectList, "Value", "Text");
        }


        public SelectList DropDownApplicationPageList(int SelectedValue)
        {
            var selectList = GetAllApplicationPage().Where(x => x.IsActive == true).Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = x.PageTitle.ToString()
            });

            return new SelectList(selectList, "Value", "Text", SelectedValue);
        }

        public MultiSelectList DropDownApplicationPageMultiList(int[] SelectedValue)
        {
            var selectList = GetAllApplicationPage().Where(x => x.IsActive == true).Select(x => new
            {
                Value = x.Id.ToString(),
                Text = x.PageTitle.ToString()
            });

            return new MultiSelectList(selectList, "Value", "Text", selectedValues: SelectedValue);
        }
    }
}
