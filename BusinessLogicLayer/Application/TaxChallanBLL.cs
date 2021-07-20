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
    public class TaxChallanBLL
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGenericRepository<TaxChallan> _repository;
        private readonly UserBLL _UserBLL;
        public TaxChallanBLL(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _repository = _unitOfWork.GenericRepository<TaxChallan>();
            _UserBLL = new UserBLL(_unitOfWork);
        }
        public int Add(TaxChallan module)
        {
            module.Status = TaxChallanStatus.Unverified;
            module.DistributorId = (int)SessionHelper.LoginUser.DistributorId;
            module.CreatedBy = SessionHelper.LoginUser.Id;
            module.IsDeleted = false;
            module.IsActive = true;
            module.CreatedDate = DateTime.Now;
            _unitOfWork.GenericRepository<TaxChallan>().Insert(module);
            return _unitOfWork.Save();
        }
        public bool Update(TaxChallan module)
        {
            var item = _unitOfWork.GenericRepository<TaxChallan>().GetById(module.Id);
            item.UpdatedBy = SessionHelper.LoginUser.Id;
            item.UpdatedDate = DateTime.Now;
            _unitOfWork.GenericRepository<TaxChallan>().Update(item);
            return _unitOfWork.Save() > 0;
        }
        public int UpdateSNo(TaxChallan module)
        {
            var item = _repository.GetById(module.Id);
            if (_repository.GetAllList().Count() > 1)
            {
                item.SNo = _repository.GetAllList().Max(y => y.SNo) + 1;
            }
            else
            {
                item.SNo = 500000001;
            }
            _repository.Update(item);
            return _unitOfWork.Save();
        }
        public int Delete(int id)
        {
            var item = _unitOfWork.GenericRepository<TaxChallan>().GetById(id);
            item.IsDeleted = true;
            return _unitOfWork.Save();
        }
        public bool UpdateStatus(TaxChallan model, TaxChallanStatus TaxChallanStatus, string Remarks)
        {
            if (TaxChallanStatus.Verified == TaxChallanStatus)

            {
                model.ApprovedBy = SessionHelper.LoginUser.Id;
                model.ApprovedDate = DateTime.Now;
            }
            else if (TaxChallanStatus.Rejected == TaxChallanStatus)

            {
                model.RejectedRemarks = Remarks;
                model.RejectedBy = SessionHelper.LoginUser.Id;
                model.RejectedDate = DateTime.Now;
            }
            model.Status = TaxChallanStatus;
            model.UpdatedBy = SessionHelper.LoginUser.Id;
            model.UpdatedDate = DateTime.Now;
            _repository.Update(model);
            return _unitOfWork.Save() > 0;
        }
        public TaxChallan GetById(int id)
        {
            return _unitOfWork.GenericRepository<TaxChallan>().GetById(id);
        }
        public List<TaxChallan> GetAllTaxChallanMaster()
        {
            return _unitOfWork.GenericRepository<TaxChallan>().GetAllList().Where(x => x.IsDeleted == false).ToList();
        }
        public List<TaxChallan> Where(Expression<Func<TaxChallan, bool>> predicate)
        {
            return _repository.Where(predicate);
        }
        public TaxChallan FirstOrDefault(Expression<Func<TaxChallan, bool>> predicate)
        {
            return _repository.FirstOrDefault(predicate);
        }
        public List<TaxChallan> Search(TaxChallanViewModel model)
        {
            var LamdaId = (Expression<Func<TaxChallan, bool>>)(x => x.IsDeleted == false);
            if (model.DistributorId != null)
            {
                LamdaId = LamdaId.And(e => e.DistributorId == model.DistributorId);
            }
            if (model.TaxChallanNo != null)
            {
                LamdaId = LamdaId.And(e => e.SNo == model.TaxChallanNo);
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
                         join u in _UserBLL.GetUsers().ToList()
                              on x.CreatedBy equals u.Id
                         join ua in _UserBLL.GetUsers().ToList()
                              on x.ApprovedBy equals ua.Id into approvedGroup
                         from a1 in approvedGroup.DefaultIfEmpty()
                         join ur in _UserBLL.GetUsers().ToList()
                              on x.RejectedBy equals ur.Id into rejectedGroup
                         from a2 in rejectedGroup.DefaultIfEmpty()
                         select new TaxChallan
                         {
                             Id = x.Id,
                             SNo = x.SNo,
                             Distributor = x.Distributor,
                             Status = x.Status,
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
                         });

            return query.ToList();
        }
    }
}
