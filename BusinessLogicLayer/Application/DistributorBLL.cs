using BusinessLogicLayer.Application;
using BusinessLogicLayer.HelperClasses;
using DataAccessLayer.Repository;
using DataAccessLayer.WorkProcess;
using Microsoft.AspNetCore.Mvc.Rendering;
using Models.Application;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace BusinessLogicLayer.ApplicationSetup
{
    public class DistributorBLL
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGenericRepository<Distributor> repository;
        private readonly RegionBLL regionBLL;
        public DistributorBLL(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            repository = unitOfWork.GenericRepository<Distributor>();
            regionBLL = new RegionBLL(_unitOfWork);
        }
        public int AddDistributor(Distributor module)
        {
            module.CNIC = module.CNIC.Replace("-", "");
            module.MobileNumber = module.MobileNumber.Replace("-", "");
            module.CreatedBy = SessionHelper.LoginUser.Id;
            module.IsDeleted = false;
            module.CreatedDate = DateTime.Now;
            repository.Insert(module);
            return _unitOfWork.Save();
        }
        public bool AddRange(List<Distributor> distributors)
        {
            var region = regionBLL.GetAllRegion();
            distributors.ForEach(e => e.RegionId = region.First(c => c.SAPId == e.RegionCode).Id);
            distributors.ForEach(x => x.DistributorName = x.DistributorName.Trim());
            repository.AddRange(distributors);
            return _unitOfWork.Save() > 0;
        }
        public int UpdateDistributor(Distributor module)
        {
            var item = GetDistributorBySAPId(module.DistributorSAPCode);
            var region = regionBLL.GetAllRegion();
            item.RegionId = region.First(c => c.SAPId == module.Region.SAPId).Id;
            item.City = module.City;
            item.DistributorSAPCode = module.DistributorSAPCode;
            item.DistributorCode = module.DistributorCode;
            item.DistributorName = module.DistributorName.Trim();
            item.DistributorAddress = module.DistributorAddress;
            item.EmailAddress = module.EmailAddress;
            item.NTN = module.NTN;
            item.CNIC = module.CNIC.Replace("-", "");
            item.MobileNumber = module.MobileNumber.Replace("-", "");
            item.CustomerGroup = module.CustomerGroup;
            item.IsActive = true;
            item.IsDeleted = false;
            item.UpdatedBy = SessionHelper.LoginUser.Id;
            item.UpdatedDate = DateTime.Now;
            repository.Update(item);
            return _unitOfWork.Save();
        }
        public int DeleteDistributor(int id)
        {
            var item = repository.GetById(id);
            item.IsDeleted = true;
            repository.Delete(item);
            return _unitOfWork.Save();
        }
        public Distributor GetDistributorById(int id)
        {
            return repository.GetById(id);
        }
        public Distributor GetDistributorBySAPId(string id)
        {
            return repository.FirstOrDefault(e => e.DistributorSAPCode == id);
        }
        public List<Distributor> GetAllDistributor()
        {
            return repository.GetAllList().Where(x => x.IsDeleted == false).ToList();
        }
        public List<Distributor> GetAllDistributors()
        {
            return repository.GetAllList().ToList();
        }
        public bool CheckDistributorName(int Id, string DistributorCode)
        {
            int? DistributorId = Id == 0 ? null : (int?)Id;
            var model = repository.GetAllList().ToList().Where(x => x.IsDeleted == false && x.DistributorCode == DistributorCode && x.Id != DistributorId || (DistributorId == null && x.Id == null)).FirstOrDefault();
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
        public SelectList DropDownDistributorList(int? SelectedValue)
        {
            var selectList = GetAllDistributor().Where(x => x.IsActive == true && !x.IsDeleted).Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = x.DistributorName.ToString()
            });

            return new SelectList(selectList, "Value", "Text", SelectedValue);
        }
        public SelectList DropDownDistributorList()
        {
            var selectList = GetAllDistributor().Where(x => x.IsActive == true && !x.IsDeleted).Select(x => new SelectListItem
            {
                Value = x.DistributorSAPCode.ToString(),
                Text = x.DistributorName.ToString()
            });

            return new SelectList(selectList, "Value", "Text");
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
        public List<Distributor> Where(Expression<Func<Distributor, bool>> predicate)
        {
            return repository.Where(predicate);
        }
        public Distributor FirstOrDefault(Expression<Func<Distributor, bool>> predicate)
        {
            return repository.FirstOrDefault(predicate);
        }
    }
}
