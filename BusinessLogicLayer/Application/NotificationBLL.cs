using BusinessLogicLayer.HelperClasses;
using DataAccessLayer.Repository;
using DataAccessLayer.WorkProcess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace BusinessLogicLayer.GeneralSetup
{
    public class NotificationBLL
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGenericRepository<Notification> _repository;
        public NotificationBLL(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _repository = _unitOfWork.GenericRepository<Notification>();
        }
        public bool Add(Notification module)
        {
            module.CreatedBy = SessionHelper.LoginUser.Id;
            module.CreatedDate = DateTime.Now;
            _repository.Insert(module);
            return _unitOfWork.Save() > 0;
        }
        public int AddRange(List<Notification> module)
        {
            _repository.AddRange(module);
            return _unitOfWork.Save();
        }
        public bool Update(Notification module)
        {
            var item = _repository.GetById(module.Id);
            _repository.Update(item);
            return _unitOfWork.Save() > 0;
        }
        public int UpdateRange(List<Notification> NotificationList)
        {
            _repository.UpdateRange(NotificationList);
            return _unitOfWork.Save();
        }
        public Notification GetNotificationById(int id)
        {            
            return _repository.GetById(id);
        }
        public List<Notification> GetAllNotification()
        {
            return _repository.GetAllList().ToList();
        }
        public List<Notification> Where(Expression<Func<Notification, bool>> predicate)
        {
            return _repository.Where(predicate);
        }
        public Notification FirstOrDefault(Expression<Func<Notification, bool>> predicate)
        {
            return _repository.FirstOrDefault(predicate);
        }
    }
}
