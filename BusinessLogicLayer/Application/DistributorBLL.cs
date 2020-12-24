using BusinessLogicLayer.HelperClasses;
using DataAccessLayer.WorkProcess;
using Microsoft.AspNetCore.Mvc.Rendering;
using Models.Application;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BusinessLogicLayer.ApplicationSetup
{
    public class DistributorBLL
    {
        private readonly IUnitOfWork _unitOfWork;
        public DistributorBLL(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public int AddDistributor(Distributor module)
        {
            module.CNIC = module.CNIC.Replace("-", "");
            module.MobileNumber = module.MobileNumber.Replace("-", "");
            module.CreatedBy = SessionHelper.LoginUser.Id;
            module.IsDeleted = false;
            module.CreatedDate = DateTime.Now;
            _unitOfWork.GenericRepository<Distributor>().Insert(module);
            return _unitOfWork.Save();
        }

        public int UpdateDistributor(Distributor module)
        {
            var item = _unitOfWork.GenericRepository<Distributor>().GetById(module.Id);
            item.RegionId = module.RegionId;
            //item.SubRegionId = module.SubRegionId;
            item.City = module.City;
            item.DistributorSAPCode = module.DistributorSAPCode;
            item.DistributorCode = module.DistributorCode;
            item.DistributorName = module.DistributorName;
            item.DistributorAddress = module.DistributorAddress;
            item.EmailAddress = module.EmailAddress;
            item.NTN = module.NTN;
            item.CNIC = module.CNIC.Replace("-", "");
            item.MobileNumber = module.MobileNumber.Replace("-", "");
            item.IsActive = module.IsActive;
            item.UpdatedBy = SessionHelper.LoginUser.Id;
            item.UpdatedDate = DateTime.Now;
            _unitOfWork.GenericRepository<Distributor>().Update(item);
            return _unitOfWork.Save();
        }

        public int DeleteDistributor(int id)
        {
            var item = _unitOfWork.GenericRepository<Distributor>().GetById(id);
            item.IsDeleted = true;
            _unitOfWork.GenericRepository<Distributor>().Delete(item);
            return _unitOfWork.Save();
        }

        public Distributor GetDistributorById(int id)
        {
            return _unitOfWork.GenericRepository<Distributor>().GetById(id);
        }

        public List<Distributor> GetAllDistributor()
        {
            return _unitOfWork.GenericRepository<Distributor>().GetAllList().Where(x => x.IsDeleted == false).ToList();
        }

        public bool CheckDistributorName(int Id, string DistributorCode)
        {
            int? DistributorId = Id == 0 ? null : (int?)Id;
            var model = _unitOfWork.GenericRepository<Distributor>().GetAllList().ToList().Where(x => x.IsDeleted == false && x.DistributorCode == DistributorCode && x.Id != DistributorId || (DistributorId == null && x.Id == null)).FirstOrDefault();
            if (model != null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public SelectList DropDownDistributorByUserId(int[] Distributor)
        {
            var selectList = GetAllDistributor().Where(x => x.IsActive == true && Distributor.Contains(x.Id)).Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = x.DistributorName.ToString()
            });

            return new SelectList(selectList, "Value", "Text");
        }


        public SelectList DropDownDistributorList(int SelectedValue)
        {
            var selectList = GetAllDistributor().Where(x => x.IsActive == true).Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = x.DistributorName.ToString()
            });

            return new SelectList(selectList, "Value", "Text", SelectedValue);
        }

        public MultiSelectList DropDownDistributorMultiList(int[] SelectedValue)
        {
            var selectList = GetAllDistributor().Where(x => x.IsActive == true).Select(x => new
            {
                Value = x.Id.ToString(),
                Text = x.DistributorName.ToString()
            });

            return new MultiSelectList(selectList, "Value", "Text", selectedValues: SelectedValue);
        }
    }
}
