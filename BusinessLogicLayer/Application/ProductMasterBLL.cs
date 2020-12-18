using BusinessLogicLayer.HelperClasses;
using DataAccessLayer.WorkProcess;
using Models.Application;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BusinessLogicLayer.Application
{
    public class ProductMasterBLL
    {
        private readonly IUnitOfWork _unitOfWork;
        public ProductMasterBLL(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public int AddProductMaster(ProductMaster module)
        {
            module.CreatedBy = SessionHelper.LoginUser.Id;
            module.IsDeleted = false;
            module.CreatedDate = DateTime.Now;
            _unitOfWork.GenericRepository<ProductMaster>().Insert(module);
            return _unitOfWork.Save();
        }

        public int UpdateProductMaster(ProductMaster module)
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

        public int DeleteProductMaster(int id)
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
    }
}
