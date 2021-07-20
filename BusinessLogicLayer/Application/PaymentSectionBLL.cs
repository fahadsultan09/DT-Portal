using DataAccessLayer.Repository;
using DataAccessLayer.WorkProcess;
using Microsoft.AspNetCore.Mvc.Rendering;
using Models.Application;
using System.Collections.Generic;
using System.Linq;

namespace BusinessLogicLayer.Application
{
    public class PaymentSectionBLL
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGenericRepository<PaymentSection> _repository;
        public PaymentSectionBLL(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _repository = unitOfWork.GenericRepository<PaymentSection>();
        }

        public List<PaymentSection> GetAllPaymentSection()
        {
            return _repository.GetAllList().Where(x => x.IsDeleted == false).ToList();
        }

        public SelectList DropDownPaymentSectionList()
        {
            var selectList = GetAllPaymentSection().Where(x => x.IsActive == true).Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = x.Name.Trim()
            });

            return new SelectList(selectList, "Value", "Text");
        }
    }
}
