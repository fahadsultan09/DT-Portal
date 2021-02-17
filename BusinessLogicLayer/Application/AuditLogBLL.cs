using BusinessLogicLayer.HelperClasses;
using DataAccessLayer.Repository;
using DataAccessLayer.WorkProcess;
using Models.Application;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BusinessLogicLayer.Application
{
    public class AuditLogBLL
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGenericRepository<AuditLog> repository;
        public AuditLogBLL(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            repository = _unitOfWork.GenericRepository<AuditLog>();
        }
        private int AddAuditLog(AuditLog module)
        {
            module.CreatedBy = SessionHelper.LoginUser is null ? null : SessionHelper.LoginUser.UserName;
            module.CreatedDate = DateTime.Now;
            repository.Insert(module);
            return _unitOfWork.Save();
        }

        public AuditLog GetAuditLogById(int id)
        {
            return repository.GetById(id);
        }

        public List<AuditLog> GetAllAuditLog()
        {
            return repository.GetAllList().ToList();
        }

        public void AddAuditLog(string PageName, string ActionName, string Description)
        {
            AuditLog module = new AuditLog
            {
                PageName = PageName,
                ActionName = ActionName,
                Description = Description
            };
            AddAuditLog(module);
        }
    }
}
