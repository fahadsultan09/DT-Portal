using BusinessLogicLayer.HelperClasses;
using DataAccessLayer.Repository;
using DataAccessLayer.WorkProcess;
using Models.Application;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Utility;

namespace BusinessLogicLayer.GeneralSetup
{
    public class ComplaintUserEmailBLL
    {
        private readonly IUnitOfWork _unitOfWork;
        private IGenericRepository<ComplaintUserEmail> _repository;
        public ComplaintUserEmailBLL(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _repository = _unitOfWork.GenericRepository<ComplaintUserEmail>();
        }
        public bool Add(ComplaintUserEmail module)
        {
            module.CreatedBy = SessionHelper.LoginUser.Id;
            module.CreatedDate = DateTime.Now;
            _repository.Insert(module);
            return _unitOfWork.Save() > 0;
        }
        public ComplaintUserEmail GetById(int id)
        {
            return _repository.GetById(id);
        }
        public List<ComplaintUserEmail> GetAll()
        {
            return _repository.GetAllList().ToList();
        }
        public string[] GetAllAComplaintUserEmailByComplaintSubCategoryId(int ComplaintSubCategoryId, EmailType EmailType)
        {
            return _repository.GetAllList().Where(x => x.ComplaintSubCategoryId == ComplaintSubCategoryId && x.EmailType == EmailType).Select(x => x.UserEmailId).ToArray();
        }

        public int[] GetAllApplicationPageActionByApplicationPageId(int ComplaintSubCategoryId)
        {
            return _repository.GetAllList().Where(x => x.ComplaintSubCategoryId == ComplaintSubCategoryId).Select(x => Convert.ToInt32(x.ComplaintSubCategoryId)).ToArray();
        }
        public List<ComplaintUserEmail> Where(Expression<Func<ComplaintUserEmail, bool>> predicate)
        {
            return _repository.Where(predicate);
        }
        public bool DeleteRange(List<ComplaintUserEmail> teams)
        {
            _repository.DeleteRange(teams);
            return _unitOfWork.Save() > 0;
        }
        public bool AddRange(List<ComplaintUserEmail> list)
        {
            _repository.AddRange(list);
            return _unitOfWork.Save() > 0;
        }
        public List<ComplaintUserEmail> GetAllComplaintUserEmailByComplaintSubCategoryId(int[] ComplaintSubCategoryId)
        {
            return _repository.Where(x => ComplaintSubCategoryId.Distinct().Contains(x.ComplaintSubCategoryId) && x.EmailType == EmailType.KPI).ToList();
        }
    }
}
