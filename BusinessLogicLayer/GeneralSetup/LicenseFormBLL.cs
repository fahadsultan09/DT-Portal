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
    public class LicenseFormBLL
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGenericRepository<LicenseForm> repository;
        public LicenseFormBLL(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            repository = _unitOfWork.GenericRepository<LicenseForm>();
        }
        public bool AddLicenseForm(LicenseForm module)
        {
            module.LicenseFormNo.Trim();
            module.CreatedBy = SessionHelper.LoginUser.Id;
            module.IsDeleted = false;
            module.CreatedDate = DateTime.Now;
            repository.Insert(module);
            return _unitOfWork.Save() > 0;
        }
        public bool UpdateLicenseForm(LicenseForm module)
        {
            var item = repository.GetById(module.Id);
            item.LicenseFormNo = module.LicenseFormNo.Trim();
            item.IsActive = module.IsActive;
            item.UpdatedBy = SessionHelper.LoginUser.Id;
            item.UpdatedDate = DateTime.Now;
            repository.Update(item);
            return _unitOfWork.Save() > 0;
        }
        public bool DeleteLicenseForm(int id)
        {
            var item = repository.GetById(id);
            item.IsDeleted = false;
            repository.Delete(item);
            return _unitOfWork.Save() > 0;
        }
        public LicenseForm GetLicenseFormById(int id)
        {
            return repository.GetById(id);
        }
        public List<LicenseForm> GetAllLicenseForm()
        {
            return repository.GetAllList().Where(x => !x.IsDeleted && x.IsActive).ToList();
        }
        public bool CheckLicenseFormName(int Id, string ModuleName)
        {
            int? DosageFormId = Id == 0 ? null : (int?)Id;
            var model = _unitOfWork.GenericRepository<LicenseForm>().GetAllList().ToList().Where(x => x.IsDeleted == false && x.LicenseFormNo == ModuleName && x.Id != DosageFormId || (DosageFormId == null && x.Id == null)).FirstOrDefault();
            if (model != null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        public SelectList DropDownLicenseFormList(int SelectedValue)
        {
            var selectList = GetAllLicenseForm().Where(x => x.IsActive == true).Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = x.LicenseFormNo.ToString()
            });

            return new SelectList(selectList, "Value", "Text", SelectedValue);
        }
        public SelectList DropDownLicenseFormList()
        {
            var selectList = GetAllLicenseForm().Where(x => x.IsActive == true).Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = x.LicenseFormNo.ToString()
            });

            return new SelectList(selectList, "Value", "Text");
        }
    }
}
