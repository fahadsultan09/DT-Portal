using BusinessLogicLayer.HelperClasses;
using DataAccessLayer.Repository;
using DataAccessLayer.WorkProcess;
using Models.Application;
using Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Utility;

namespace BusinessLogicLayer.Application
{
    public class OrderReturnBLL
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGenericRepository<OrderReturnMaster> _repository;
        private readonly UserBLL _UserBLL;
        public OrderReturnBLL(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _repository = _unitOfWork.GenericRepository<OrderReturnMaster>();
            _UserBLL = new UserBLL(_unitOfWork);
        }
        public int Add(OrderReturnMaster module)
        {
            module.CreatedBy = SessionHelper.LoginUser.Id;
            module.IsDeleted = false;
            module.IsActive = true;
            module.CreatedDate = DateTime.Now;
            _unitOfWork.GenericRepository<OrderReturnMaster>().Insert(module);
            return _unitOfWork.Save();
        }
        public bool Update(OrderReturnMaster module)
        {
            var item = _unitOfWork.GenericRepository<OrderReturnMaster>().GetById(module.Id);
            item.UpdatedBy = SessionHelper.LoginUser.Id;
            item.UpdatedDate = DateTime.Now;
            _unitOfWork.GenericRepository<OrderReturnMaster>().Update(item);
            return _unitOfWork.Save() > 0;
        }
        public int Delete(int id)
        {
            var item = _unitOfWork.GenericRepository<OrderReturnMaster>().GetById(id);
            item.IsDeleted = true;
            return _unitOfWork.Save();
        }
        public void UpdateStatus(OrderReturnMaster model, OrderReturnStatus OrderReturnStatus, string Remarks)
        {
            //model.Remarks = Remarks;
            //model.Status = OrderReturnStatus;
            model.UpdatedBy = SessionHelper.LoginUser.Id;
            model.UpdatedDate = DateTime.Now;
            _repository.Update(model);
        }
        public OrderReturnMaster GetById(int id)
        {
            return _unitOfWork.GenericRepository<OrderReturnMaster>().GetById(id);
        }
        public List<OrderReturnMaster> GetAllOrderReturn()
        {
            return _unitOfWork.GenericRepository<OrderReturnMaster>().GetAllList().Where(x => x.IsDeleted == false).ToList();
        }
        public List<OrderReturnMaster> Where(Expression<Func<OrderReturnMaster, bool>> predicate)
        {
            return _repository.Where(predicate);
        }
        public OrderReturnMaster FirstOrDefault(Expression<Func<OrderReturnMaster, bool>> predicate)
        {
            return _repository.FirstOrDefault(predicate);
        }
        public List<OrderReturnMaster> Search(OrderReturnViewModel model)
        {
            var LamdaId = (Expression<Func<OrderReturnMaster, bool>>)(x => x.IsDeleted == false);
            if (model.DistributorId != null)
            {
                LamdaId = LamdaId.And(e => e.DistributorId == model.DistributorId);
            }
            if (model.Status != null)
            {
                //LamdaId = LamdaId.And(e => e.Status == model.Status);
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
                         select new OrderReturnMaster
                         {
                             Id = x.Id,
                             Distributor = x.Distributor,
                             DistributorId = x.DistributorId,
                             CreatedDate = x.CreatedDate,
                         }).ToList();

            return query.OrderByDescending(x => x.Id).ToList();
        }
        public List<OrderReturnMaster> SearchReport(OrderReturnSearch model)
        {
            var LamdaId = (Expression<Func<OrderReturnMaster, bool>>)(x => x.IsDeleted == false);
            if (model.DistributorId != null)
            {
                LamdaId = LamdaId.And(e => e.DistributorId == model.DistributorId);
            }
            if (model.OrderReturnNo != null)
            {
                LamdaId = LamdaId.And(e => e.Id == model.OrderReturnNo);
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
                         select new OrderReturnMaster
                         {
                             Id = x.Id,
                             Distributor = x.Distributor,
                             Status = x.Status,
                             DistributorId = x.DistributorId,
                             CreatedBy = x.CreatedBy,
                             CreatedName = (u.FirstName + " " + u.LastName + " (" + u.UserName + ")"),
                             CreatedDate = x.CreatedDate,
                         }).ToList();

            return query.OrderByDescending(x => x.Id).ToList();
        }
    }
}
