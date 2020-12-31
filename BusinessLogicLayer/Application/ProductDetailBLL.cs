using BusinessLogicLayer.HelperClasses;
using DataAccessLayer.WorkProcess;
using Models.Application;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BusinessLogicLayer.Application
{
    public class ProductDetailBLL
    {
        private readonly IUnitOfWork _unitOfWork;
        public ProductDetailBLL(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public int AddProductDetail(ProductDetail module)
        {
            module.CreatedBy = SessionHelper.LoginUser.Id;
            module.IsDeleted = false;
            module.CreatedDate = DateTime.Now;
            _unitOfWork.GenericRepository<ProductDetail>().Insert(module);
            return _unitOfWork.Save();
        }

        public int UpdateProductDetail(ProductDetail module)
        {
            var item = _unitOfWork.GenericRepository<ProductDetail>().GetById(module.Id);
            //item.Visibility = module.Visibility;
            item.PlantLocation = module.PlantLocation;
            item.CompanyId = module.CompanyId;
            item.WTaxRate = module.WTaxRate;
            item.Factor = module.Factor;
            item.ParentDistributor = module.ParentDistributor;
            item.S_OrderType = module.S_OrderType;
            item.R_OrderType = module.R_OrderType;
            item.SaleOrganization = module.SaleOrganization;
            item.DistributionChannel = module.DistributionChannel;
            item.Division = module.Division;
            item.DispatchPlant = module.DispatchPlant;
            item.S_StorageLocation = module.S_StorageLocation;
            item.R_StorageLocation = module.R_StorageLocation;
            item.SalesItemCategory = module.SalesItemCategory;
            item.ReturnItemCategory = module.ReturnItemCategory;
            item.IsTaxApplicable = module.IsTaxApplicable;
            item.UpdatedBy = SessionHelper.LoginUser.Id;
            item.UpdatedDate = DateTime.Now;
            _unitOfWork.GenericRepository<ProductDetail>().Update(item);
            return _unitOfWork.Save();
        }

        public int DeleteProductDetail(int id)
        {
            var item = _unitOfWork.GenericRepository<ProductDetail>().GetById(id);
            item.IsDeleted = true;
            _unitOfWork.GenericRepository<ProductDetail>().Delete(item);
            return _unitOfWork.Save();
        }

        public ProductDetail GetProductDetailById(int id)
        {
            return _unitOfWork.GenericRepository<ProductDetail>().GetById(id);
        }

        public List<ProductDetail> GetAllProductDetail()
        {
            return _unitOfWork.GenericRepository<ProductDetail>().GetAllList().Where(x => x.IsDeleted == false).ToList();
        }
    }
}
