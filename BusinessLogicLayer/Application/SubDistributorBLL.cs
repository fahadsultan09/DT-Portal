using DataAccessLayer.Repository;
using DataAccessLayer.WorkProcess;
using Models.Application;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace BusinessLogicLayer.Application
{
    public class SubDistributorBLL
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGenericRepository<SubDistributor> _repository;
        public SubDistributorBLL(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _repository = _unitOfWork.GenericRepository<SubDistributor>();
        }
        public void AddRange(List<SubDistributor> subDistributors)
        {
            _repository.AddRange(subDistributors);
        }
        public void HardDeleteRange(List<SubDistributor> subDistributors)
        {
            _repository.HardDeleteRange(subDistributors);
        }
        public SubDistributor GetById(int id)
        {
            return _repository.GetById(id);
        }
        public List<SubDistributor> GetAllSubDistributor()
        {
            return _repository.GetAllList().Where(x => x.IsDeleted == false).ToList();
        }
        public List<SubDistributor> GetAllSubDistributors()
        {
            return _repository.GetAllList().ToList();
        }
        public bool CheckDistributor(int Id, int DistributorId)
        {
            int? id = Id == 0 ? null : (int?)Id;
            var model = _repository.GetAllList().ToList().Where(x => x.IsDeleted == false && x.SubDistributorId == DistributorId && x.Id != id || (id == null && x.Id == null)).FirstOrDefault();

            if (model != null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        public List<SubDistributor> Where(Expression<Func<SubDistributor, bool>> predicate)
        {
            return _repository.Where(predicate);
        }
    }
}
