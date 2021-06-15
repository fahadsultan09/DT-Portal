using BusinessLogicLayer.HelperClasses;
using DataAccessLayer.Repository;
using DataAccessLayer.WorkProcess;
using Models.Application;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace BusinessLogicLayer.GeneralSetup
{
    public class PolicyBLL
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGenericRepository<Policy> _repository;
        public PolicyBLL(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _repository = _unitOfWork.GenericRepository<Policy>(); ;
        }
        public int AddPolicy(Policy module)
        {
            module.CreatedBy = SessionHelper.LoginUser.Id;
            module.IsDeleted = false;
            module.CreatedDate = DateTime.Now;
            _repository.Insert(module);
            return _unitOfWork.Save();
        }
        public int UpdatePolicy(Policy module)
        {
            var item = _repository.GetById(module.Id);
            item.Title = module.Title;
            item.Icon = module.Icon;
            item.Style = module.Style;
            item.Message = module.Message;
            item.Sort = module.Sort;
            item.IsActive = module.IsActive;
            item.UpdatedBy = SessionHelper.LoginUser.Id;
            item.UpdatedDate = DateTime.Now;
            _repository.Update(item);
            return _unitOfWork.Save();
        }
        public int DeletePolicy(int id)
        {
            var item = _repository.GetById(id);
            item.IsDeleted = false;
            item.DeletedBy = SessionHelper.LoginUser.Id;
            item.DeletedDate = DateTime.Now;
            _repository.Delete(item);
            return _unitOfWork.Save();
        }
        public Policy GetPolicyById(int id)
        {
            return _repository.GetById(id);
        }
        public List<Policy> GetAllPolicy()
        {
            return _repository.GetAllList().Where(x => x.IsDeleted == false).ToList();
        }
        public bool CheckPolicyName(int Id, string PolicyName)
        {
            int? PolicyId = Id == 0 ? null : (int?)Id;
            var model = _repository.GetAllList().ToList().Where(x => x.IsDeleted == false && x.Title == PolicyName && x.Id != PolicyId || (PolicyId == null && x.Id == null)).FirstOrDefault();
            if (model != null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        public List<Policy> Where(Expression<Func<Policy, bool>> predicate)
        {
            return _repository.Where(predicate);
        }
    }
}
