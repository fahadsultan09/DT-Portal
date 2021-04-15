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
    public class ComplaintBLL
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGenericRepository<Complaint> _repository;
        private readonly UserBLL _UserBLL;
        public ComplaintBLL(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _repository = _unitOfWork.GenericRepository<Complaint>();
            _UserBLL = new UserBLL(_unitOfWork);
        }
        public int Add(Complaint module)
        {
            module.CreatedBy = SessionHelper.LoginUser.Id;
            module.IsDeleted = false;
            module.IsActive = true;
            module.CreatedDate = DateTime.Now;
            _repository.Insert(module);
            return _unitOfWork.Save();
        }
        public int Update(Complaint module)
        {
            var item = _repository.GetById(module.Id);
            item.IsActive = module.IsActive;
            item.UpdatedBy = SessionHelper.LoginUser.Id;
            item.UpdatedDate = DateTime.Now;
            _repository.Update(item);
            return _unitOfWork.Save();
        }
        public int UpdateSNo(Complaint module)
        {
            var item = _repository.GetById(module.Id);
            if (_repository.GetAllList().Any())
            {
                item.SNo = _repository.GetAllList().Max(y => y.SNo) + 1;
            }
            else
            {
                item.SNo = 400000001;
            }
            _repository.Update(item);
            return _unitOfWork.Save();
        }
        public int Delete(int id)
        {
            var item = _repository.GetById(id);
            item.IsDeleted = true;
            return _unitOfWork.Save();
        }
        public void UpdateStatus(Complaint model, ComplaintStatus ComplaintStatus, string Remarks)
        {
            model.Status = ComplaintStatus;
            if (ComplaintStatus.Resolved == ComplaintStatus)
            {
                model.ResolvedRemarks = Remarks;
                model.ResolvedBy = SessionHelper.LoginUser.Id;
                model.ResolvedDate = DateTime.Now;
            }
            else if (ComplaintStatus.Approved == ComplaintStatus)

            {
                model.Remarks = Remarks;
                model.ApprovedBy = SessionHelper.LoginUser.Id;
                model.ApprovedDate = DateTime.Now;
            }
            else if (ComplaintStatus.Reject == ComplaintStatus)

            {
                model.Remarks = Remarks;
                model.RejectedBy = SessionHelper.LoginUser.Id;
                model.RejectedDate = DateTime.Now;
            }
            model.UpdatedBy = SessionHelper.LoginUser.Id;
            model.UpdatedDate = DateTime.Now;
            _repository.Update(model);
        }
        public Complaint GetById(int id)
        {
            return _repository.GetById(id);
        }
        public List<Complaint> GetAllComplaint()
        {
            return _repository.GetAllList().Where(x => x.IsDeleted == false).ToList();
        }
        public List<Complaint> Where(Expression<Func<Complaint, bool>> predicate)
        {
            return _repository.Where(predicate).ToList();
        }
        public Complaint FirstOrDefault(Expression<Func<Complaint, bool>> predicate)
        {
            return _repository.FirstOrDefault(predicate);
        }
        public List<Complaint> Search(ComplaintViewModel model)
        {
            var LamdaId = (Expression<Func<Complaint, bool>>)(x => x.IsDeleted == false);
            if (model.DistributorId != null)
            {
                LamdaId = LamdaId.And(e => e.DistributorId == model.DistributorId);
            }
            if (model.ComplaintNo != null)
            {
                LamdaId = LamdaId.And(e => e.SNo == model.ComplaintNo);
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
                         select new Complaint
                         {
                             Id = x.Id,
                             SNo = x.SNo,
                             Distributor = x.Distributor,
                             ComplaintCategory = x.ComplaintCategory,
                             ComplaintCategoryId = x.ComplaintCategoryId,
                             ComplaintSubCategory = x.ComplaintSubCategory,
                             ComplaintSubCategoryId = x.ComplaintSubCategoryId,
                             Status = x.Status,
                             DistributorId = x.DistributorId,
                             CreatedDate = x.CreatedDate,
                         }).ToList();

            return query.OrderByDescending(x => x.Id).ToList();
        }
        public List<Complaint> SearchReport(ComplaintSearch model)
        {
            var LamdaId = (Expression<Func<Complaint, bool>>)(x => x.IsDeleted == false);
            if (model.DistributorId != null)
            {
                LamdaId = LamdaId.And(e => e.DistributorId == model.DistributorId);
            }
            if (model.ComplaintNo != null)
            {
                LamdaId = LamdaId.And(e => e.SNo == model.ComplaintNo);
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
                         join ra in _UserBLL.GetAllUser().ToList()
                              on x.ResolvedBy equals ra.Id into resolvedGroup
                         from a1 in resolvedGroup.DefaultIfEmpty()
                         join ua in _UserBLL.GetAllUser().ToList()
                              on x.ApprovedBy equals ua.Id into approvedGroup
                         from a2 in approvedGroup.DefaultIfEmpty()
                         join ur in _UserBLL.GetAllUser().ToList()
                              on x.RejectedBy equals ur.Id into rejectedGroup
                         from a3 in rejectedGroup.DefaultIfEmpty()
                         select new Complaint
                         {
                             Id = x.Id,
                             SNo = x.SNo,
                             Distributor = x.Distributor,
                             ComplaintCategory = x.ComplaintCategory,
                             ComplaintSubCategory = x.ComplaintSubCategory,
                             Status = x.Status,
                             DistributorId = x.DistributorId,
                             CreatedBy = x.CreatedBy,
                             CreatedName = (u.FirstName + " " + u.LastName + " (" + u.UserName + ")"),
                             CreatedDate = x.CreatedDate,
                             ResolvedBy = x.ResolvedBy,
                             ResolverName = a1 == null ? string.Empty : (a1.FirstName + " " + a1.LastName + " (" + a1.UserName + ")"),
                             ResolvedDate = x.ResolvedDate,
                             ApprovedBy = x.ApprovedBy,
                             ApprovedName = a2 == null ? string.Empty : (a2.FirstName + " " + a2.LastName + " (" + a2.UserName + ")"),
                             ApprovedDate = x.ApprovedDate,
                             RejectedBy = x.RejectedBy,
                             RejectedName = a3 == null ? string.Empty : (a3.FirstName + " " + a3.LastName + " (" + a3.UserName + ")"),
                             RejectedDate = x.RejectedDate
                         }).ToList();

            return query.OrderByDescending(x => x.Id).ToList();
        }
        public List<Complaint> GetPendingComplaint()
        {
            return _repository.GetAllList().Where(x => x.Status == ComplaintStatus.Pending).ToList();
        }
    }
}
