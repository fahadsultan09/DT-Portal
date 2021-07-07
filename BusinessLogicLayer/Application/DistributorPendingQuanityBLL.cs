using DataAccessLayer.Repository;
using DataAccessLayer.WorkProcess;
using Models.Application;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace BusinessLogicLayer.Application
{
    public class DistributorPendingQuanityBLL
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGenericRepository<DistributorPendingQuantity> _repository;
        public DistributorPendingQuanityBLL(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _repository = _unitOfWork.GenericRepository<DistributorPendingQuantity>();
        }
        public void AddRange(List<DistributorPendingQuantity> DistributorPendingQuanityList, int DistributorId)
        {
            List<DistributorPendingQuantity> distributorPendingQuanities = new List<DistributorPendingQuantity>();
            if (DistributorId > 0)
            {
                distributorPendingQuanities = _repository.Where(x => x.DistributorId == DistributorId).ToList();
            }
            else
            {
                distributorPendingQuanities = GetAllList();
            }
            if (distributorPendingQuanities.Count > 0 && DistributorPendingQuanityList.Count() > 0)
            {
                DeleteRange(distributorPendingQuanities);
            }
            _repository.AddRange(DistributorPendingQuanityList.Distinct().ToList());
            _unitOfWork.Save();
        }
        public int UpdateRange(List<DistributorPendingQuantity> DistributorPendingQuantitys)
        {
            _repository.UpdateRange(DistributorPendingQuantitys);
            return _unitOfWork.Save();
        }
        public void DeleteRange(List<DistributorPendingQuantity> DistributorPendingQuantitys)
        {
            _repository.DeleteRange(DistributorPendingQuantitys);
        }
        public List<DistributorPendingQuantity> GetAllList()
        {
            return _repository.GetAllList().ToList();
        }
        public List<DistributorPendingQuantity> Where(Expression<Func<DistributorPendingQuantity, bool>> model)
        {
            return _repository.Where(model).ToList();
        }

    }
}
