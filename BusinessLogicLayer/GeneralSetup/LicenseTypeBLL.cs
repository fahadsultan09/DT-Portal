using BusinessLogicLayer.HelperClasses;
using DataAccessLayer.Repository;
using DataAccessLayer.WorkProcess;
using Microsoft.AspNetCore.Mvc.Rendering;
using Models.Application;
using System;
using System.Collections.Generic;
using System.Linq;


namespace BusinessLogicLayer.GeneralSetup
{
    public class LicenseTypeBLL
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGenericRepository<LicenseType> repository;
        public LicenseTypeBLL(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            repository = _unitOfWork.GenericRepository<LicenseType>();
        }
        public List<LicenseType> GetAllLicenseType()
        {
            return repository.GetAllList().Where(x => !x.IsDeleted && x.IsActive).ToList();
        }
    }
}
