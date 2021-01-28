using BusinessLogicLayer.GeneralSetup;
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
    public class PaymentBLL
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGenericRepository<PaymentMaster> _repository;
        private readonly CompanyBLL _CompanyBLL;
        private readonly UserBLL _UserBLL;
        public PaymentBLL(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _repository = _unitOfWork.GenericRepository<PaymentMaster>();
            _CompanyBLL = new CompanyBLL(_unitOfWork);
            _UserBLL = new UserBLL(_unitOfWork);
        }
        public int Add(PaymentMaster module)
        {
            module.CreatedBy = SessionHelper.LoginUser.Id;
            module.IsDeleted = false;
            module.IsActive = true;
            module.CreatedDate = DateTime.Now;
            _unitOfWork.GenericRepository<PaymentMaster>().Insert(module);
            return _unitOfWork.Save();
        }
        public bool Update(PaymentMaster module)
        {
            var item = _unitOfWork.GenericRepository<PaymentMaster>().GetById(module.Id);
            item.SAPCompanyCode = module.SAPCompanyCode;
            item.SAPDocumentNumber = module.SAPDocumentNumber;
            item.SAPFiscalYear = module.SAPFiscalYear;
            item.UpdatedBy = SessionHelper.LoginUser.Id;
            item.UpdatedDate = DateTime.Now;
            _unitOfWork.GenericRepository<PaymentMaster>().Update(item);
            return _unitOfWork.Save() > 0;
        }
        public int Delete(int id)
        {
            var item = _unitOfWork.GenericRepository<PaymentMaster>().GetById(id);
            item.IsDeleted = true;
            return _unitOfWork.Save();
        }
        public void UpdateStatus(PaymentMaster model, PaymentStatus paymentStatus, string Remarks)
        {
            model.Remarks = Remarks;
            model.Status = paymentStatus;
            model.UpdatedBy = SessionHelper.LoginUser.Id;
            model.UpdatedDate = DateTime.Now;
            _repository.Update(model);
        }
        public PaymentMaster GetById(int id)
        {
            return _unitOfWork.GenericRepository<PaymentMaster>().GetById(id);
        }
        public List<PaymentMaster> GetAllPaymentMaster()
        {
            return _unitOfWork.GenericRepository<PaymentMaster>().GetAllList().Where(x => x.IsDeleted == false).ToList();
        }
        public List<PaymentMaster> Where(Expression<Func<PaymentMaster, bool>> predicate)
        {
            return _repository.Where(predicate);
        }
        public PaymentMaster FirstOrDefault(Expression<Func<PaymentMaster, bool>> predicate)
        {
            return _repository.FirstOrDefault(predicate);
        }
        public List<PaymentMaster> Search(PaymentViewModel model)
        {
            var LamdaId = (Expression<Func<PaymentMaster, bool>>)(x => x.IsDeleted == false);
            if (model.DistributorId != null)
            {
                LamdaId = LamdaId.And(e => e.DistributorId == model.DistributorId);
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
                         select new PaymentMaster
                         {
                             Id = x.Id,
                             Distributor = x.Distributor,
                             PaymentMode = x.PaymentMode,
                             Amount = x.Amount,
                             Status = x.Status,
                             DistributorId = x.DistributorId,
                             CreatedDate = x.CreatedDate,
                         }).ToList();

            return query.OrderByDescending(x => x.Id).ToList();
        }
        public List<PaymentMaster> SearchReport(PaymentSearch model)
        {
            var LamdaId = (Expression<Func<PaymentMaster, bool>>)(x => x.IsDeleted == false);
            if (model.DistributorId != null)
            {
                LamdaId = LamdaId.And(e => e.DistributorId == model.DistributorId);
            }
            if (model.PaymentNo != null)
            {
                LamdaId = LamdaId.And(e => e.Id == model.PaymentNo);
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
            var Filter = _repository.Where(LamdaId).ToList();
            var query = (from x in Filter
                         join u in _UserBLL.GetAllUser().ToList()
                              on x.CreatedBy equals u.Id
                         join ua in _UserBLL.GetAllUser().ToList()
                              on x.ApprovedBy equals ua.Id into approvedGroup
                         from a1 in approvedGroup.DefaultIfEmpty()
                         join ur in _UserBLL.GetAllUser().ToList()
                              on x.RejectedBy equals ur.Id into rejectedGroup
                         from a2 in rejectedGroup.DefaultIfEmpty()
                         select new PaymentMaster
                         {
                             Id = x.Id,
                             Company = x.Company,
                             PaymentMode = x.PaymentMode,
                             Distributor = x.Distributor,
                             Status = x.Status,
                             Amount = x.Amount,
                             SAPDocumentNumber = x.SAPDocumentNumber,
                             SAPFiscalYear = x.SAPFiscalYear,
                             DistributorId = x.DistributorId,
                             CreatedBy = x.CreatedBy,
                             CreatedName = (u.FirstName + " " + u.LastName + " (" + u.UserName + ")"),
                             CreatedDate = x.CreatedDate,
                             ApprovedBy = x.ApprovedBy,
                             ApprovedName = a1 == null ? string.Empty : (a1.FirstName + " " + a1.LastName + " (" + a1.UserName + ")"),
                             ApprovedDate = x.ApprovedDate,
                             RejectedBy = x.RejectedBy,
                             RejectedName = a2 == null ? string.Empty : (a2.FirstName + " " + a2.LastName + " (" + a2.UserName + ")"),
                             RejectedDate = x.RejectedDate
                         }).ToList();

            return query.OrderByDescending(x => x.Id).ToList();
        }
        public SAPPaymentViewModel AddPaymentToSAP(int PaymentId)
        {
            var payment = GetAllPaymentMaster().Where(e => e.Id == PaymentId).FirstOrDefault();

            SAPPaymentViewModel model = new SAPPaymentViewModel() 
            {
                PAY_ID = payment.Id.ToString(),
                REF = payment.PaymentModeNo.ToString(),
                COMPANY = _CompanyBLL.GetAllCompany().FirstOrDefault(x=>x.Id == payment.CompanyId).SAPCompanyCode,
                AMOUNT = payment.Amount.ToString(),
                DISTRIBUTOR = payment.Distributor.DistributorSAPCode,
                B_CODE = new BankBLL(_unitOfWork).GetAllBank().FirstOrDefault(x => x.BranchCode == payment.CompanyBankCode && x.CompanyId == GetById(PaymentId).CompanyId).GLAccount,

            };
            return model;
        }
    }
}
