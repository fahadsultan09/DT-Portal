using BusinessLogicLayer.HelperClasses;
using DataAccessLayer.Repository;
using DataAccessLayer.WorkProcess;
using Models.Application;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DistributorPortal.BusinessLogicLayer.ApplicationSetup
{
    public class AuditTrailBLL
    {
        private readonly IUnitOfWork _unitOfWork;
        private IGenericRepository<AuditTrail> repository;
        public AuditTrailBLL(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            repository = _unitOfWork.GenericRepository<AuditTrail>();
        }
        private int AddAuditTrail(AuditTrail module)
        {
            module.CreatedBy = SessionHelper.LoginUser.UserName;
            module.CreatedDate = DateTime.Now;
            repository.Insert(module);
            return _unitOfWork.Save();
        }

        public AuditTrail GetAuditTrailById(int id)
        {
            return repository.GetById(id);
        }

        public List<AuditTrail> GetAllAuditTrail()
        {
            return repository.GetAllList().ToList();
        }

        public void AddAuditTrail(string PageName, string ActionName, string Description)
        {
            AuditTrail module = new AuditTrail();
            module.PageName = PageName;
            module.ActionName = ActionName;
            module.Description = Description;
            AddAuditTrail(module);
        }
    }
}
