using BusinessLogicLayer.HelperClasses;
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
        public void Add(SubDistributor module)
        {
            List<SubDistributor> SubDistributorList = new List<SubDistributor>();
            if (CheckDistributor(module.Id, module.DistributorId))
            {
                foreach (var item in module.SubDistributorIds.Where(x => x != module.DistributorId).ToArray())
                {
                    SubDistributor subDistributor = new SubDistributor();
                    subDistributor.DistributorId = module.DistributorId;
                    subDistributor.SubDistributorId = item;
                    subDistributor.IsParent = false;
                    subDistributor.IsActive = module.IsActive;
                    subDistributor.IsDeleted = false;
                    subDistributor.CreatedBy = SessionHelper.LoginUser.Id;
                    subDistributor.CreatedDate = DateTime.Now;
                    SubDistributorList.Add(subDistributor);
                }
                SubDistributor parentDistributor = new SubDistributor();
                parentDistributor.DistributorId = module.DistributorId;
                parentDistributor.SubDistributorId = module.DistributorId;
                parentDistributor.IsParent = true;
                parentDistributor.IsActive = module.IsActive;
                parentDistributor.IsDeleted = false;
                parentDistributor.CreatedBy = SessionHelper.LoginUser.Id;
                parentDistributor.CreatedDate = DateTime.Now;
                SubDistributorList.Add(parentDistributor);
                _repository.AddRange(SubDistributorList);
                _unitOfWork.Save();
            }
        }
        public void Update(SubDistributor module)
        {
            //Get all SubDistributorIds against DistributorId
            List<SubDistributor> AllSubDistributors = GetAllSubDistributors().Where(x => x.DistributorId == module.DistributorId && !x.IsParent).ToList();

            //Unchecked SubDistributorIds that exist in DB update 
            var SoftDeleteSubDistributorIds = AllSubDistributors.Select(x => x.SubDistributorId).Where(x => !module.SubDistributorIds.Contains(x)).ToList();

            //Checked SubDistributorIds that exist in DB update
            var updateSubDistributorIds = AllSubDistributors.Select(x => x.SubDistributorId).Where(x => module.SubDistributorIds.Contains(x)).ToList();

            //Add new SubDistributorIds in DB
            var AddSubDistributorIds = module.SubDistributorIds.Where(x => !updateSubDistributorIds.Select(y => y).Contains(x)).ToList();

            //if SubDistributorIds that already exist in DB but not yet delete it
            if (SoftDeleteSubDistributorIds != null && SoftDeleteSubDistributorIds.Count > 0)
            {
                foreach (var itemSoftDelete in SoftDeleteSubDistributorIds.ToList())
                {
                    SubDistributor subDistributor = AllSubDistributors.Where(x => x.SubDistributorId == itemSoftDelete && x.IsDeleted != true).FirstOrDefault();
                    if (subDistributor != null)
                    {
                        subDistributor.IsActive = true;
                        subDistributor.IsDeleted = true;
                        subDistributor.DeletedBy = SessionHelper.LoginUser.Id;
                        subDistributor.DeletedDate = DateTime.Now;
                        _repository.Update(subDistributor);
                    }
                }
            }
            if (updateSubDistributorIds != null && updateSubDistributorIds.Count > 0)
            {
                foreach (var itemupdate in updateSubDistributorIds)
                {
                    SubDistributor subDistributor = AllSubDistributors.Where(x => x.SubDistributorId == itemupdate).FirstOrDefault();
                    if (subDistributor != null)
                    {
                        subDistributor.IsActive = module.IsActive;
                        subDistributor.IsDeleted = false;
                        subDistributor.UpdatedBy = SessionHelper.LoginUser.Id;
                        subDistributor.UpdatedDate = DateTime.Now;
                        subDistributor.DeletedBy = null;
                        subDistributor.DeletedDate = null;
                        _repository.Update(subDistributor);
                    }
                }
            }
            if (AddSubDistributorIds != null && AddSubDistributorIds.Count > 0)
            {
                foreach (var itemAddLabelIDs in AddSubDistributorIds.Where(x => x != module.DistributorId))
                {
                    SubDistributor subDistributor = new SubDistributor();
                    subDistributor.DistributorId = module.DistributorId;
                    subDistributor.SubDistributorId = itemAddLabelIDs;
                    subDistributor.IsParent = false;
                    subDistributor.IsActive = module.IsActive;
                    subDistributor.IsDeleted = false;
                    subDistributor.CreatedBy = SessionHelper.LoginUser.Id;
                    subDistributor.CreatedDate = DateTime.Now;
                    _repository.Insert(subDistributor);
                }
            }
            _unitOfWork.Save();
        }
        public void Delete(int id)
        {
            var item = _repository.GetById(id);
            item.IsDeleted = true;
            item.DeletedBy = SessionHelper.LoginUser.Id;
            item.DeletedDate = DateTime.Now;
            _repository.HardDelete(item);
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
