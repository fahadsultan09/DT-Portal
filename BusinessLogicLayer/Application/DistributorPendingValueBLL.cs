using DataAccessLayer.Repository;
using DataAccessLayer.WorkProcess;
using Models.Application;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace BusinessLogicLayer.Application
{
    public class DistributorPendingValueBLL
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGenericRepository<DistributorPendingValue> _repository;
        public DistributorPendingValueBLL(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _repository = _unitOfWork.GenericRepository<DistributorPendingValue>();
        }
        public void AddRange(List<DistributorPendingValue> DistributorPendingValueList, int DistributorId)
        {
            List<DistributorPendingValue> distributorPendingValues = new List<DistributorPendingValue>();
            if (DistributorId > 0)
            {
                distributorPendingValues = _repository.Where(x => x.DistributorId == DistributorId).ToList();
            }
            else
            {
                distributorPendingValues = GetAllList();
            }
            if (distributorPendingValues.Count > 0 && DistributorPendingValueList.Count() > 0)
            {
                DeleteRange(distributorPendingValues);
            }
            _repository.AddRange(DistributorPendingValueList.Distinct().ToList());
            _unitOfWork.Save();
        }
        public int UpdateRange(List<DistributorPendingValue> distributorPendingValues)
        {
            _repository.UpdateRange(distributorPendingValues);
            return _unitOfWork.Save();
        }
        public void DeleteRange(List<DistributorPendingValue> distributorPendingValues)
        {
            _repository.DeleteRange(distributorPendingValues);
        }
        public List<DistributorPendingValue> GetAllList()
        {
            return _repository.GetAllList().ToList();
        }
        public List<DistributorPendingValue> Where(Expression<Func<DistributorPendingValue, bool>> model)
        {
            return _repository.Where(model).ToList();
        }

    }
}
