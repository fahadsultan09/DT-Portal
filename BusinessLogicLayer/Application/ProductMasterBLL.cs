using BusinessLogicLayer.HelperClasses;
using DataAccessLayer.Repository;
using DataAccessLayer.WorkProcess;
using Microsoft.AspNetCore.Mvc.Rendering;
using Models.Application;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace BusinessLogicLayer.Application
{
    public class ProductMasterBLL
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGenericRepository<ProductMaster> repository;
        public ProductMasterBLL(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            repository = unitOfWork.GenericRepository<ProductMaster>();
        }
        public int Add(ProductMaster module)
        {
            module.CreatedBy = SessionHelper.LoginUser.Id;
            module.IsDeleted = false;
            module.CreatedDate = DateTime.Now;
            _unitOfWork.GenericRepository<ProductMaster>().Insert(module);
            return _unitOfWork.Save();
        }
        public bool AddRange(List<ProductMaster> ProductMaster)
        {            
            repository.AddRange(ProductMaster);
            return _unitOfWork.Save() > 0;
        }
        public int Update(ProductMaster module)
        {
            var item = _unitOfWork.GenericRepository<ProductMaster>().GetById(module.Id);
            item.ProductName = module.ProductName;
            item.PackCode = module.PackCode;
            item.SAPProductCode = module.SAPProductCode;
            item.Discount = module.Discount;
            item.Rate = module.Rate;
            item.TradePrice = module.TradePrice;
            item.SFSize = module.SFSize;
            item.CartonSize = module.CartonSize;
            item.Strength = module.Strength;
            item.PackSize = module.PackSize;
            item.ProductOrigin = module.ProductOrigin;
            item.ProductPrice = module.ProductPrice;
            item.ProductDescription = module.ProductDescription;
            item.IsActive = module.IsActive;
            item.UpdatedBy = SessionHelper.LoginUser.Id;
            item.UpdatedDate = DateTime.Now;
            _unitOfWork.GenericRepository<ProductMaster>().Update(item);
            return _unitOfWork.Save();
        }
        public int UpdateRange(List<ProductMaster> module)
       {
            repository.UpdateRange(module);
            return _unitOfWork.Save();
        }
        public int Delete(int id)
        {
            var item = _unitOfWork.GenericRepository<ProductMaster>().GetById(id);
            item.IsDeleted = true;
            _unitOfWork.GenericRepository<ProductMaster>().Delete(item);
            return _unitOfWork.Save();
        }
        public ProductMaster GetProductMasterById(int id)
        {
            return _unitOfWork.GenericRepository<ProductMaster>().GetById(id);
        }
        public List<ProductMaster> GetAllProductMaster()
        {
            return _unitOfWork.GenericRepository<ProductMaster>().GetAllList().Where(x => x.IsDeleted == false).ToList();
        }
        public List<ProductMaster> Where(Expression<Func<ProductMaster, bool>> predicate)
        {
            return repository.Where(predicate);
        }
        public ProductMaster FirstOrDefault(Expression<Func<ProductMaster, bool>> predicate)
        {
            return repository.FirstOrDefault(predicate);
        }
        public SelectList DropDownProductList()
        {
            var selectList = GetAllProductMaster().Where(x => x.IsActive == true).Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = x.ProductName.Trim() + " " + x.ProductDescription.Trim() 
            });

            return new SelectList(selectList, "Value", "Text");
        }
        public SelectList DropDownProductSAPCodeList()
        {
            var selectList = GetAllProductMaster().Where(x => x.IsActive == true).Select(x => new SelectListItem
            {
                Value = x.SAPProductCode.ToString(),
                Text = x.ProductName.Trim() + " - " + x.ProductDescription.Trim()
            });

            return new SelectList(selectList, "Value", "Text");
        }
    }
}
