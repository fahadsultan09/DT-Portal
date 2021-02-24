using BusinessLogicLayer.HelperClasses;
using DataAccessLayer.WorkProcess;
using Microsoft.AspNetCore.Mvc.Rendering;
using Models.Application;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utility.HelperClasses;

namespace BusinessLogicLayer.GeneralSetup
{
    public class BankBLL
    {
        private readonly IUnitOfWork _unitOfWork;
        public BankBLL(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public int AddBank(Bank module)
        {
            module.CreatedBy = SessionHelper.LoginUser.Id;
            module.IsDeleted = false;
            module.CreatedDate = DateTime.Now;
            _unitOfWork.GenericRepository<Bank>().Insert(module);
            return _unitOfWork.Save();
        }
        public int UpdateBank(Bank module)
        {
            var item = _unitOfWork.GenericRepository<Bank>().GetById(module.Id);
            item.CompanyId = module.CompanyId;
            item.BankName = module.BankName;
            item.Branch = module.Branch;
            item.BranchCode = module.BranchCode;
            item.AccountNo = module.AccountNo;
            item.IBANNo = module.IBANNo;
            item.IsActive = module.IsActive;
            item.UpdatedBy = SessionHelper.LoginUser.Id;
            item.UpdatedDate = DateTime.Now;
            _unitOfWork.GenericRepository<Bank>().Update(item);
            return _unitOfWork.Save();
        }
        public int DeleteBank(int id)
        {
            var item = _unitOfWork.GenericRepository<Bank>().GetById(id);
            item.IsDeleted = true;
            _unitOfWork.GenericRepository<Bank>().Delete(item);
            return _unitOfWork.Save();
        }
        public Bank GetBankById(int id)
        {
            return _unitOfWork.GenericRepository<Bank>().GetById(id);
        }
        public List<Bank> GetAllBank()
        {
            return _unitOfWork.GenericRepository<Bank>().GetAllList().Where(x => x.IsDeleted == false).ToList();
        }
        public bool CheckBankName(int Id, string BankName, int CompanyId)
        {
            int? BankId = Id == 0 ? null : (int?)Id;
            var model = _unitOfWork.GenericRepository<Bank>().GetAllList().ToList().Where(x => x.IsDeleted == false && x.BankName == BankName && x.CompanyId == CompanyId && x.Id != BankId || (BankId == null && x.Id == null)).FirstOrDefault();
            if (model != null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        public SelectList DropDownBankByUserId(int[] Bank)
        {
            var selectList = GetAllBank().Where(x => x.IsActive == true && Bank.Contains(x.Id)).Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = x.BankName.ToString()
            });
            return new SelectList(selectList, "Value", "Text");
        }
        public SelectList DropDownBankList(int SelectedValue)
        {
            var selectList = GetAllBank().Where(x => x.IsActive == true).Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = x.BankName.ToString()
            });
            return new SelectList(selectList, "Value", "Text", SelectedValue);
        }
        public MultiSelectList DropDownBankMultiList(int[] SelectedValue)
        {
            var selectList = GetAllBank().Where(x => x.IsActive == true).Select(x => new
            {
                Value = x.Id.ToString(),
                Text = x.BankName.ToString()
            });
            return new MultiSelectList(selectList, "Value", "Text", selectedValues: SelectedValue);
        }
        public SelectList DropDownBankList()
        {
            var selectList = GetAllBank().Where(x => x.IsActive == true).Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = x.BankName.Trim()
            });

            return new SelectList(selectList, "Value", "Text");
        }
        public SelectList DropDownBankList(int? CompanyId, int? SelectedValue)
        {
            var selectList = GetAllBank().Where(x => x.IsActive == true && x.CompanyId == CompanyId).Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = x.BankName.ToString()
            });
            return new SelectList(selectList, "Value", "Text", SelectedValue);
        }
    }
}
