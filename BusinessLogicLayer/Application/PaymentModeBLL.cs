using BusinessLogicLayer.HelperClasses;
using DataAccessLayer.WorkProcess;
using Microsoft.AspNetCore.Mvc.Rendering;
using Models.Application;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utility.HelperClasses;

namespace BusinessLogicLayer.Application
{
    public class PaymentModeBLL
    {
        private readonly IUnitOfWork _unitOfWork;
        public PaymentModeBLL(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public List<PaymentMode> GetAllPaymentMode()
        {
            return _unitOfWork.GenericRepository<PaymentMode>().GetAllList().Where(x => x.IsDeleted == false).ToList();
        }

        public SelectList DropDownPaymentModeList()
        {
            var selectList = GetAllPaymentMode().Where(x => x.IsActive == true).Select(x => new SelectListItem
            {
                Value = EncryptDecrypt.Encrypt(x.Id.ToString()),
                Text = x.PaymentName.Trim()
            });

            return new SelectList(selectList, "Value", "Text");
        }
    }
}
