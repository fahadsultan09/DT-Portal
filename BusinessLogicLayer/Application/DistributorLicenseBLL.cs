using BusinessLogicLayer.HelperClasses;
using DataAccessLayer.Repository;
using DataAccessLayer.WorkProcess;
using Models.Application;
using Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Utility;

namespace BusinessLogicLayer.Application
{
    public class DistributorLicenseBLL
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGenericRepository<DistributorLicense> _repository;
        public DistributorLicenseBLL(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _repository = _unitOfWork.GenericRepository<DistributorLicense>();
        }
        public int Add(DistributorLicense module)
        {
            module.CreatedBy = SessionHelper.LoginUser.Id;
            module.IsDeleted = false;
            module.IsActive = true;
            module.CreatedDate = DateTime.Now;
            module.Status = LicenseStatus.Submitted;
            _repository.Insert(module);
            return _unitOfWork.Save();
        }
        public int Update(DistributorLicense module)
        {
            var item = _repository.GetById(module.Id);
            item.IssueDate = module.IssueDate;
            item.LicenseId = module.LicenseId;
            item.Attachment = module.Attachment;
            item.DistributorId = module.DistributorId;
            item.Expiry = module.Expiry;
            item.IsActive = module.IsActive;
            item.Status = module.Status;
            item.UpdatedBy = SessionHelper.LoginUser.Id;
            item.UpdatedDate = DateTime.Now;
            _repository.Update(item);
            return _unitOfWork.Save();
        }
        public int Delete(int id)
        {
            var item = _repository.GetById(id);
            item.IsDeleted = true;
            return _unitOfWork.Save();
        }
        public void UpdateStatus(DistributorLicense model, LicenseStatus licenseStatus, string Remarks)
        {
            model.Remarks = Remarks;
            model.Status = licenseStatus;
            model.UpdatedBy = SessionHelper.LoginUser.Id;
            model.UpdatedDate = DateTime.Now;
            _repository.Update(model);
        }
        public DistributorLicense GetById(int id)
        {
            return _repository.GetById(id);
        }
        public List<DistributorLicense> GetAllDistributorLicense()
        {
            return _repository.Where(x => x.IsDeleted == false).ToList();
        }
        public List<DistributorLicense> Where(Expression<Func<DistributorLicense, bool>> predicate)
        {
            return _repository.Where(predicate);
        }
        public DistributorLicense FirstOrDefault(Expression<Func<DistributorLicense, bool>> predicate)
        {
            return _repository.FirstOrDefault(predicate);
        }
    }
}
