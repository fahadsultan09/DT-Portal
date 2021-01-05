using BusinessLogicLayer.HelperClasses;
using DataAccessLayer.WorkProcess;
using Microsoft.AspNetCore.Mvc.Rendering;
using Models.Application;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BusinessLogicLayer.Application
{
    public class CompanyBLL
    {
        private readonly IUnitOfWork _unitOfWork;
        public CompanyBLL(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public List<Company> GetAllCompany()
        {
            return _unitOfWork.GenericRepository<Company>().GetAllList().Where(x => x.IsDeleted == false).ToList();
        }
        public SelectList DropDownCompanyList()
        {
            var selectList = GetAllCompany().Where(x => x.IsActive == true).Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = x.CompanyName.Trim()
            });

            return new SelectList(selectList, "Value", "Text");
        }
    }
}
