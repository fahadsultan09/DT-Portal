using BusinessLogicLayer.HelperClasses;
using DataAccessLayer.Repository;
using DataAccessLayer.WorkProcess;
using Models.UserRights;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace BusinessLogicLayer.GeneralSetup
{
    public class ApplicationPageActionBLL
    {
        private IUnitOfWork _unitOfWork;
        private IGenericRepository<ApplicationPageAction> repository;
        public ApplicationPageActionBLL(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            repository = _unitOfWork.GenericRepository<ApplicationPageAction>();
        }
        public bool AddApplicationPageAction(ApplicationPageAction module)
        {
            module.CreatedBy = SessionHelper.LoginUser.Id;
            module.CreatedDate = DateTime.Now;
            repository.Insert(module);
            return _unitOfWork.Save() > 0;
        }

        public bool UpdateApplicationPageAction(int id, ApplicationPageAction module)
        {
            module.Id = id;
            repository.Update(module);
            return _unitOfWork.Save() > 0;
        }

        private bool DeleteApplicationPageAction(int id)
        {
            var item = repository.GetById(id);
            repository.Delete(item);
            return _unitOfWork.Save() > 0;
        }

        public ApplicationPageAction GetApplicationPageActionById(int id)
        {
            return repository.GetById(id);
        }

        public List<ApplicationPageAction> GetAllApplicationPageAction()
        {
            return repository.GetAllList().ToList();
        }

        public int[] GetAllApplicationPageActionByApplicationPageId(long ApplicationPageId)
        {
            return repository.GetAllList().Where(x => x.ApplicationPageId == ApplicationPageId).Select(x => Convert.ToInt32(x.ApplicationActionId)).ToArray();
        }
        public List<ApplicationPageAction> Where(Expression<Func<ApplicationPageAction, bool>> predicate)
        {
            return repository.Where(predicate);
        }
        public bool DeleteRange(List<ApplicationPageAction> teams)
        {
            repository.DeleteRange(teams);
            return _unitOfWork.Save() > 0;
        }

        public bool AddRange(List<ApplicationPageAction> list)
        {
            repository.AddRange(list);
            return _unitOfWork.Save() > 0;
        }
    }
}
