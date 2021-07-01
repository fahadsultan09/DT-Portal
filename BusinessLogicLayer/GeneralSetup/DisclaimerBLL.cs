using BusinessLogicLayer.HelperClasses;
using DataAccessLayer.Repository;
using DataAccessLayer.WorkProcess;
using Models.Application;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BusinessLogicLayer.GeneralSetup
{
    public class DisclaimerBLL
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGenericRepository<Disclaimer> repository;
        public DisclaimerBLL(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            repository = _unitOfWork.GenericRepository<Disclaimer>();
        }
        public bool AddDisclaimer(Disclaimer module)
        {
            module.Name.Trim();
            module.Description.Trim();
            module.CreatedBy = SessionHelper.LoginUser.Id;
            module.IsDeleted = false;
            module.CreatedDate = DateTime.Now;
            repository.Insert(module);
            return _unitOfWork.Save() > 0;
        }
        public bool UpdateDisclaimer(Disclaimer module)
        {
            var item = repository.GetById(module.Id);
            item.Name = module.Name.Trim();
            item.Description = module.Description.Trim();
            item.IsActive = module.IsActive;
            item.UpdatedBy = SessionHelper.LoginUser.Id;
            item.UpdatedDate = DateTime.Now;
            repository.Update(item);
            return _unitOfWork.Save() > 0;
        }
        public bool DeleteDisclaimer(int id)
        {
            var item = repository.GetById(id);
            item.IsDeleted = true;
            repository.Delete(item);
            return _unitOfWork.Save() > 0;
        }
        public Disclaimer GetDisclaimerById(int id)
        {
            return repository.GetById(id);
        }
        public List<Disclaimer> GetAllDisclaimer()
        {
            return repository.GetAllList().Where(x => x.IsDeleted == false).ToList();
        }
        public bool CheckDisclaimerName(int Id, string ModuleName)
        {
            int? DosageFormId = Id == 0 ? null : (int?)Id;
            var model = _unitOfWork.GenericRepository<Disclaimer>().GetAllList().ToList().Where(x => x.IsDeleted == false && x.Name == ModuleName && x.Id != DosageFormId || (DosageFormId == null && x.Id == null)).FirstOrDefault();
            if (model != null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
