using BusinessLogicLayer.HelperClasses;
using DataAccessLayer.WorkProcess;
using Microsoft.AspNetCore.Mvc.Rendering;
using Models.Application;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BusinessLogicLayer.GeneralSetup
{
    public class BankBLL
    {
        private readonly IUnitOfWork _unitOfWork;
        public BankBLL(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public List<Bank> GetAllBank()
        {
            return _unitOfWork.GenericRepository<Bank>().GetAllList().Where(x => x.IsDeleted == false).ToList();
        }
        public SelectList DropDownBankList()
        {
            var selectList = GetAllBank().Where(x => x.IsActive == true).Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = x.BankName.Trim()
            });

            return new SelectList(selectList, "Value", "Text");
        }
    }
}
