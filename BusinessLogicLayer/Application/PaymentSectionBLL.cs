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
    public class PaymentSectionBLL
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGenericRepository<PaymentSection> _repository;
        public PaymentSectionBLL(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _repository = _unitOfWork.GenericRepository<PaymentSection>();
        }
        public int AddPaymentSection(PaymentSection module)
        {
            module.CreatedBy = SessionHelper.LoginUser.Id;
            module.IsDeleted = false;
            module.CreatedDate = DateTime.Now;
            _repository.Insert(module);
            return _unitOfWork.Save();
        }
        public int UpdatePaymentSection(PaymentSection module)
        {
            var item = _repository.GetById(module.Id);
            item.CompanyId = module.CompanyId;
            item.FormNo = module.FormNo;
            item.GLAccount = module.GLAccount;
            item.TaxRate = module.TaxRate;
            item.IsActive = module.IsActive;
            item.UpdatedBy = SessionHelper.LoginUser.Id;
            item.UpdatedDate = DateTime.Now;
            _repository.Update(item);
            return _unitOfWork.Save();
        }
        public int DeletePaymentSection(int id)
        {
            var item = _repository.GetById(id);
            item.IsDeleted = true;
            item.DeletedBy = SessionHelper.LoginUser.Id;
            item.DeletedDate = DateTime.Now;
            _repository.Delete(item);
            return _unitOfWork.Save();
        }
        public PaymentSection GetPaymentSectionById(int id)
        {
            return _repository.GetById(id);
        }
        public List<PaymentSection> GetAllPaymentSection()
        {
            return _repository.GetAllList().Where(x => x.IsDeleted == false).ToList();
        }
        public bool CheckPaymentSectionName(int Id, string PaymentSectionName, int CompanyId)
        {
            int? PaymentSectionId = Id == 0 ? null : (int?)Id;
            var model = _repository.GetAllList().ToList().Where(x => x.IsDeleted == false && x.FormNo == PaymentSectionName && x.CompanyId == CompanyId && x.Id != PaymentSectionId || (PaymentSectionId == null && x.Id == null)).FirstOrDefault();
            if (model != null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        public SelectList DropDownPaymentSectionByUserId(int[] PaymentSection)
        {
            var selectList = GetAllPaymentSection().Where(x => x.IsActive == true && PaymentSection.Contains(x.Id)).Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = x.FormNo.ToString()
            });
            return new SelectList(selectList, "Value", "Text");
        }
        public SelectList DropDownPaymentSectionTaxRateList(int SelectedValue)
        {
            var selectList = GetAllPaymentSection().Where(x => x.IsActive == true).Select(x => new SelectListItem
            {
                Value = x.TaxRate.ToString(),
                Text = x.FormNo.ToString()
            });
            return new SelectList(selectList, "Value", "Text", SelectedValue);
        }
        public MultiSelectList DropDownPaymentSectionMultiList(int[] SelectedValue)
        {
            var selectList = GetAllPaymentSection().Where(x => x.IsActive == true).Select(x => new
            {
                Value = x.Id.ToString(),
                Text = x.FormNo.ToString()
            });
            return new MultiSelectList(selectList, "Value", "Text", selectedValues: SelectedValue);
        }
        public SelectList DropDownPaymentSectionList()
        {
            var selectList = GetAllPaymentSection().Where(x => x.IsActive == true).Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = x.FormNo.Trim()
            });

            return new SelectList(selectList, "Value", "Text");
        }
        public SelectList DropDownPaymentSectionList(int? CompanyId, int? SelectedValue)
        {
            var selectList = GetAllPaymentSection().Where(x => x.IsActive == true && x.CompanyId == CompanyId).Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = x.FormNo.ToString()
            });
            return new SelectList(selectList, "Value", "Text", SelectedValue);
        }
    }
}
