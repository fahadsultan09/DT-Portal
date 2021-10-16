using BusinessLogicLayer.HelperClasses;
using DataAccessLayer.Repository;
using DataAccessLayer.WorkProcess;
using Microsoft.AspNetCore.Mvc.Rendering;
using Models.Application;
using System;
using System.Collections.Generic;
using System.Linq;
using Utility.HelperClasses;

namespace BusinessLogicLayer.GeneralSetup
{
    public class CompanyBLL
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGenericRepository<Company> repository;
        public CompanyBLL(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            repository = _unitOfWork.GenericRepository<Company>();
        }
        public bool AddCompany(Company module)
        {
            module.CompanyName.Trim();
            module.CreatedBy = SessionHelper.LoginUser.Id;
            module.IsDeleted = false;
            module.CreatedDate = DateTime.Now;
            repository.Insert(module);
            return _unitOfWork.Save() > 0;
        }
        public bool UpdateCompany(Company module)
        {
            var item = repository.GetById(module.Id);
            item.CompanyName = module.CompanyName.Trim();
            item.SAPCompanyCode = module.SAPCompanyCode;
            item.IsPaymentAllowed = module.IsPaymentAllowed;
            item.IsReturnOrderAllowed = module.IsReturnOrderAllowed;
            item.IsActive = module.IsActive;
            item.UpdatedBy = SessionHelper.LoginUser.Id;
            item.UpdatedDate = DateTime.Now;
            repository.Update(item);
            return _unitOfWork.Save() > 0;
        }
        public bool DeleteCompany(int id)
        {
            var item = repository.GetById(id);
            item.IsDeleted = true;
            item.DeletedBy = SessionHelper.LoginUser.Id;
            item.DeletedDate = DateTime.Now;
            repository.Delete(item);
            return _unitOfWork.Save() > 0;
        }
        public Company GetCompanyById(int id)
        {
            return repository.GetById(id);
        }
        public List<Company> GetAllCompany()
        {
            return repository.GetAllList().Where(x => x.IsDeleted == false).ToList();
        }
        public bool CheckCompanyName(int Id, string ModuleName)
        {
            int? DosageFormId = Id == 0 ? null : (int?)Id;
            var model = _unitOfWork.GenericRepository<Company>().GetAllList().ToList().Where(x => x.IsDeleted == false && x.CompanyName == ModuleName && x.Id != DosageFormId || (DosageFormId == null && x.Id == null)).FirstOrDefault();
            if (model != null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        public SelectList DropDownCompanyList(int SelectedValue, bool IsPaymentAllowed)
        {
            var selectList = GetAllCompany().Where(x => x.IsDeleted == false && x.IsActive == true && (IsPaymentAllowed == true ? x.IsPaymentAllowed == IsPaymentAllowed : true)).Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = x.CompanyName.ToString()
            });

            return new SelectList(selectList, "Value", "Text", SelectedValue);
        }
        public MultiSelectList DropDownCompanyList(int[] SelectedValue, bool IsPaymentAllowed)
        {
            var selectList = GetAllCompany().Where(x => x.IsDeleted == false && x.IsActive == true && (IsPaymentAllowed == true ? x.IsPaymentAllowed == IsPaymentAllowed : true)).Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = x.CompanyName.ToString()
            });

            return new MultiSelectList(selectList, "Value", "Text", selectedValues: SelectedValue);
        }
        public SelectList DropDownCompanyList(int? SelectedValue)
        {
            var selectList = GetAllCompany().Where(x => x.IsActive == true && !x.IsDeleted).Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = x.CompanyName.ToString()
            });

            return new SelectList(selectList, "Value", "Text", SelectedValue);
        }
        public SelectList DropDownCompanyList()
        {
            var selectList = GetAllCompany().Where(x => x.IsActive == true && !x.IsDeleted && x.IsPaymentAllowed).Select(x => new SelectListItem
            {
                Value = x.SAPCompanyCode.ToString(),
                Text = x.CompanyName.ToString()
            });

            return new SelectList(selectList, "Value", "Text");
        }
        public SelectList DropDownCompanyListlocal()
        {
            var selectList = GetAllCompany().Where(x => x.IsActive == true && !x.IsDeleted && x.IsPaymentAllowed).Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = x.CompanyName.ToString()
            });

            return new SelectList(selectList, "Value", "Text");
        }
        public SelectList DropDownCompanyList(bool IsPaymentAllowed)
        {
            var selectList = GetAllCompany().Where(x => x.IsDeleted == false && x.IsActive == true && x.IsPaymentAllowed == IsPaymentAllowed).Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = x.CompanyName.ToString()
            });

            return new SelectList(selectList, "Value", "Text");
        }
    }
}
