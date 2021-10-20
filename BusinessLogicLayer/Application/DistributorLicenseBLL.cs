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
        private readonly AuditTrailBLL<DistributorLicense> _AuditTrailDistributorLicense;
        public DistributorLicenseBLL(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _repository = _unitOfWork.GenericRepository<DistributorLicense>();
            _AuditTrailDistributorLicense = new AuditTrailBLL<DistributorLicense>(_unitOfWork);
        }
        public bool Add(DistributorLicense module)
        {
            module.CreatedBy = SessionHelper.LoginUser.Id;
            module.File = null;
            module.LicenseControl = null;
            module.Status = LicenseStatus.Submitted;
            module.DistributorId = (int)SessionHelper.LoginUser.DistributorId;
            module.IsDeleted = false;
            module.IsActive = true;
            module.CreatedDate = DateTime.Now;
            module.UpdatedDate = DateTime.Now;
            _repository.Insert(module);
            _AuditTrailDistributorLicense.AddAuditTrail((int)ApplicationPages.DistributorLicense, (int)ApplicationActions.Insert, module, "Save Distributor License", module.CreatedBy);
            return _unitOfWork.Save() > 0;
        }
        public bool Update(DistributorLicense module)
        {
            var item = _repository.GetById(module.Id);
            item.Status = LicenseStatus.Submitted;
            item.LicenseTypeId = module.LicenseTypeId;
            item.LicenseId = module.LicenseId;
            item.Attachment = module.Attachment;
            item.LicenseNo = module.LicenseNo;
            item.IssuingAuthority = module.IssuingAuthority;
            item.FormNoId = module.FormNoId;
            item.IssueDate = module.IssueDate;
            item.Expiry = module.Expiry;
            item.DocumentType = module.DocumentType;
            item.RequestType = module.RequestType;
            item.IsActive = true;
            item.UpdatedBy = SessionHelper.LoginUser.Id;
            item.UpdatedDate = DateTime.Now;
            _repository.Update(item);
            module.File = null;
            _AuditTrailDistributorLicense.AddAuditTrail((int)ApplicationPages.DistributorLicense, (int)ApplicationActions.Update, module, "Update Distributor License", (int)item.UpdatedBy);
            return _unitOfWork.Save() > 0;
        }
        public bool UpdateApprove(DistributorLicense module)
        {
            var item = _repository.GetById(module.Id);
            item.Status = LicenseStatus.Verified;
            item.DocumentType = module.DocumentType;
            item.RequestType = module.RequestType;
            item.LicenseNo = module.LicenseNo;
            item.IssuingAuthority = module.IssuingAuthority;
            item.FormNoId = module.FormNoId;
            item.IssueDate = module.IssueDate;
            item.Expiry = module.Expiry;
            item.IsActive = true;
            item.UpdatedBy = SessionHelper.LoginUser.Id;
            item.UpdatedDate = DateTime.Now;
            _repository.Update(item);
            module.File = null;
            _AuditTrailDistributorLicense.AddAuditTrail((int)ApplicationPages.DistributorLicense, (int)ApplicationActions.Update, module, "Update Distributor License", (int)item.UpdatedBy);
            return _unitOfWork.Save() > 0;
        }
        public int UpdateActive(DistributorLicense module)
        {
            var item = _repository.GetById(module.Id);
            item.IsActive = module.IsActive;
            item.UpdatedBy = SessionHelper.LoginUser.Id;
            item.UpdatedDate = DateTime.Now;
            _repository.Update(item);
            module.File = null;
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
        public List<DistributorLicense> Search(DistributorLicenseViewModel model)
        {
            var LamdaId = (Expression<Func<DistributorLicense, bool>>)(x => x.IsDeleted == false);
            if (model.DistributorId != null)
            {
                LamdaId = LamdaId.And(e => e.DistributorId == model.DistributorId);
            }
            if (model.LicenseId != null)
            {
                LamdaId = LamdaId.And(e => e.LicenseId == model.LicenseId);
            }
            if (model.Status != null)
            {
                LamdaId = LamdaId.And(e => e.Status == model.Status);
            }
            if (model.FromDate != null)
            {
                LamdaId = LamdaId.And(e => e.CreatedDate.Date >= Convert.ToDateTime(model.FromDate).Date);
            }
            if (model.ToDate != null)
            {
                LamdaId = LamdaId.And(e => e.CreatedDate.Date <= Convert.ToDateTime(model.ToDate).Date);
            }
            if (model.FromDate != null && model.ToDate != null)
            {
                LamdaId = LamdaId.And(e => e.CreatedDate.Date >= Convert.ToDateTime(model.FromDate).Date || e.CreatedDate.Date <= Convert.ToDateTime(model.ToDate).Date);
            }
            var Filter = _repository.Where(LamdaId).ToList();
            var query = (from x in Filter
                         select new DistributorLicense
                         {
                             Id = x.Id,
                             Distributor = x.Distributor,
                             LicenseControl = x.LicenseControl,
                             DocumentType = x.DocumentType,
                             RequestType = x.RequestType,
                             LicenseType = x.LicenseType,
                             LicenseNo = x.LicenseNo,
                             IssuingAuthority = x.IssuingAuthority,
                             Attachment = x.Attachment,
                             IssueDate = x.IssueDate,
                             Expiry = x.Expiry,
                             Status = x.Status,
                             DistributorId = x.DistributorId,
                             CreatedDate = x.CreatedDate,
                         });

            return query.ToList();
        }
    }
}
