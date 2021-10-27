using BusinessLogicLayer.HelperClasses;
using DataAccessLayer.Repository;
using DataAccessLayer.WorkProcess;
using Microsoft.AspNetCore.Mvc.Rendering;
using Models.Application;
using Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Utility;

namespace BusinessLogicLayer.Application
{
    public class ProductDetailBLL
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGenericRepository<ProductDetail> _repository;

        public ProductDetailBLL(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _repository = unitOfWork.GenericRepository<ProductDetail>();
        }
        public int AddProductDetail(ProductDetail module)
        {
            module.CreatedBy = SessionHelper.LoginUser.Id;
            module.IsActive = true;
            module.IsDeleted = false;
            module.CreatedDate = DateTime.Now;
            _repository.Insert(module);
            return _unitOfWork.Save();
        }
        public int UpdateProductDetail(ProductDetail module)
        {
            var item = _repository.GetById(module.Id);
            item.FOCProductCode = module.FOCProductCode;
            item.ProductVisibilityId = module.ProductVisibilityId;
            item.PlantLocationId = module.PlantLocationId;
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
            item.SalesTax = module.SalesTax;
            item.IncomeTax = module.IncomeTax;
            item.AdditionalSalesTax = module.AdditionalSalesTax;
            item.LicenseControlId = module.LicenseControlId;
            item.IsPlaceOrderInSAP = module.IsPlaceOrderInSAP;
            item.FOCQuantityRatio = module.FOCQuantityRatio;
            item.UpdatedBy = SessionHelper.LoginUser.Id;
            item.UpdatedDate = DateTime.Now;
            _repository.Update(item);
            return _unitOfWork.Save();
        }
        public int UpdateRange(List<ProductDetail> ProductDetailList)
        {
            _repository.UpdateRange(ProductDetailList);
            return _unitOfWork.Save();
        }
        public int DeleteProductDetail(int id)
        {
            var item = _repository.GetById(id);
            item.IsDeleted = true;
            _repository.Delete(item);
            return _unitOfWork.Save();
        }
        public ProductDetail GetProductDetailById(int id)
        {
            return _repository.GetById(id);
        }
        public ProductDetail GetProductDetailByMasterId(int id)
        {
            return _repository.Where(e => e.ProductMasterId == id).FirstOrDefault();
        }
        public List<ProductDetail> GetAllProductDetail()
        {
            return _repository.GetAllList().Where(x => x.IsDeleted == false).ToList();
        }
        public List<ProductDetail> GetAllProductDetailById(int[] list, int OrderId)
        {
            var detail = _repository.GetAllList().Where(x => x.IsDeleted == false && list.Contains(x.ProductMasterId)).ToList();
            var OrderDetail = _unitOfWork.GenericRepository<OrderDetail>().Where(e => list.Contains(e.ProductId) && e.OrderId == OrderId).ToList();
            foreach (var item in detail)
            {
                item.ProductMaster.Quantity = OrderDetail.First(e => e.ProductId == item.ProductMasterId).Quantity;
            }
            return detail;
        }
        public SelectList DropDownProductList()
        {
            var selectList = GetAllProductDetail().Where(x => x.IsActive == true).OrderBy(x => x.ProductMaster.ProductName).Select(x => new SelectListItem
            {
                Value = x.ProductMasterId.ToString(),
                Text = x.ProductMaster.ProductDescription.Trim()
            });

            return new SelectList(selectList, "Value", "Text");
        }
        public SelectList DropDownProductReturnList()
        {
            var selectList = GetAllProductDetail().Where(x => x.IsActive == true && (x.ProductVisibilityId == ProductVisibility.Visible || x.ProductVisibilityId == ProductVisibility.OrderReturn)).OrderBy(x => x.ProductMaster.ProductName).Select(x => new SelectListItem
            {
                Value = x.ProductMasterId.ToString(),
                Text = x.ProductMaster.ProductDescription.Trim()
            });

            return new SelectList(selectList, "Value", "Text");
        }
        public ProductDetail FirstOrDefault(Expression<Func<ProductDetail, bool>> predicate)
        {
            return _repository.FirstOrDefault(predicate);
        }
        public List<ProductDetail> Where(Expression<Func<ProductDetail, bool>> expression)
        {
            return _repository.Where(expression);
        }
        public List<ProductMappingModel> GetViewModelForExcelMapped(List<ProductMaster> productMasters, List<ProductDetail> productDetails)
        {
            productMasters = productMasters.Where(x => productDetails.Select(y => y.ProductMasterId).Contains(x.Id)).ToList();
            var GetAllProduct = GetAllProductDetail();
            List<ProductMappingModel> ProductMappingModel = new List<ProductMappingModel>();
            foreach (var item in productMasters)
            {
                ProductMappingModel.Add(new ProductMappingModel()
                {
                    ProductCode = item.SAPProductCode,
                    BrandName = item.ProductName,
                    ProductDescription = item.ProductDescription,
                    PackSize = item.PackSize,
                    FOCProductCode = item.ProductDetail.FOCProductCode,
                    Visibility = Enum.GetName(typeof(ProductVisibility), item.ProductDetail.ProductVisibilityId),
                    PlantLocation = item.ProductDetail.PlantLocation is null ? "" : item.ProductDetail.PlantLocation.PlantLocationName,
                    Company = item.ProductDetail.Company is null ? "" : item.ProductDetail.Company.CompanyName,
                    WTaxRate = item.ProductDetail.WTaxRate,
                    Factor = item.ProductDetail.Factor,
                    ParentDistributor = item.ProductDetail.ParentDistributor,
                    S_OrderType = item.ProductDetail.S_OrderType,
                    R_OrderType = item.ProductDetail.R_OrderType,
                    SaleOrganization = item.ProductDetail.SaleOrganization,
                    DistributionChannel = item.ProductDetail.DistributionChannel,
                    Division = item.ProductDetail.Division,
                    DispatchPlant = item.ProductDetail.DispatchPlant,
                    S_StorageLocation = item.ProductDetail.S_StorageLocation,
                    R_StorageLocation = item.ProductDetail.R_StorageLocation,
                    SalesItemCategory = item.ProductDetail.SalesItemCategory,
                    ReturnItemCategory = item.ProductDetail.ReturnItemCategory,
                    IncomeTax = item.ProductDetail.IncomeTax.ToString(),
                    SalesTax = item.ProductDetail.SalesTax.ToString(),
                    AdditionalSalesTax = item.ProductDetail.AdditionalSalesTax.ToString(),
                    LicenseType = item.ProductDetail.LicenseControl is null ? "" : item.ProductDetail.LicenseControl.LicenseName,
                    FOCQuantityRatio = item.ProductDetail.FOCQuantityRatio.ToString(),
                });
            }
            return ProductMappingModel;
        }
        public List<ProductMappingModel> GetViewModelForExcelUnMapped(List<ProductMaster> productMasters, List<ProductDetail> productDetails)
        {
            productMasters = productMasters.Where(x => !productDetails.Select(y => y.ProductMasterId).Contains(x.Id)).ToList();
            var GetAllProduct = GetAllProductDetail();
            List<ProductMappingModel> ProductMappingModel = new List<ProductMappingModel>();
            foreach (var item in productMasters)
            {
                ProductMappingModel.Add(new ProductMappingModel()
                {
                    ProductCode = item.SAPProductCode,
                    BrandName = item.ProductName,
                    ProductDescription = item.ProductDescription,
                    PackSize = item.PackSize,
                    FOCProductCode = item.ProductDetail.FOCProductCode,
                    Visibility = Enum.GetName(typeof(ProductVisibility), item.ProductDetail.ProductVisibilityId),
                    PlantLocation = item.ProductDetail.PlantLocation is null ? "" : item.ProductDetail.PlantLocation.PlantLocationName,
                    Company = item.ProductDetail.Company is null ? "" : item.ProductDetail.Company.CompanyName,
                    WTaxRate = item.ProductDetail.WTaxRate,
                    Factor = item.ProductDetail.Factor,
                    ParentDistributor = item.ProductDetail.ParentDistributor,
                    S_OrderType = item.ProductDetail.S_OrderType,
                    R_OrderType = item.ProductDetail.R_OrderType,
                    SaleOrganization = item.ProductDetail.SaleOrganization,
                    DistributionChannel = item.ProductDetail.DistributionChannel,
                    Division = item.ProductDetail.Division,
                    DispatchPlant = item.ProductDetail.DispatchPlant,
                    S_StorageLocation = item.ProductDetail.S_StorageLocation,
                    R_StorageLocation = item.ProductDetail.R_StorageLocation,
                    SalesItemCategory = item.ProductDetail.SalesItemCategory,
                    ReturnItemCategory = item.ProductDetail.ReturnItemCategory,
                    IncomeTax = item.ProductDetail.IncomeTax.ToString(),
                    SalesTax = item.ProductDetail.SalesTax.ToString(),
                    AdditionalSalesTax = item.ProductDetail.AdditionalSalesTax.ToString(),
                    LicenseType = item.ProductDetail.LicenseControl is null ? "" : item.ProductDetail.LicenseControl.LicenseName,
                    FOCQuantityRatio = item.ProductDetail.FOCQuantityRatio.ToString(),
                });
            }
            return ProductMappingModel;
        }
        public List<ProductMappingModel> GetViewModelForExcelAll(List<ProductMaster> productMasters, List<ProductDetail> productDetails)
        {
            var GetAllProduct = GetAllProductDetail();
            productMasters.ForEach(x => x.ProductDetail = productDetails.Where(y => y.ProductMasterId == x.Id).FirstOrDefault() ?? new ProductDetail());
            List<ProductMappingModel> ProductMappingModel = new List<ProductMappingModel>();
            foreach (var item in productMasters)
            {
                ProductMappingModel.Add(new ProductMappingModel()
                {
                    ProductCode = item.SAPProductCode,
                    BrandName = item.ProductName,
                    ProductDescription = item.ProductDescription,
                    PackCode = item.PackCode,
                    CartonSize = item.CartonSize.ToString(),
                    PackSize = item.PackSize,
                    FOCProductCode = item.ProductDetail.FOCProductCode,
                    Visibility = Enum.GetName(typeof(ProductVisibility), item.ProductDetail.ProductVisibilityId),
                    PlantLocation = item.ProductDetail.PlantLocation is null ? "" : item.ProductDetail.PlantLocation.PlantLocationName,
                    Company = item.ProductDetail.Company is null ? "" : item.ProductDetail.Company.CompanyName,
                    WTaxRate = item.ProductDetail.WTaxRate,
                    Factor = item.ProductDetail.Factor,
                    ParentDistributor = item.ProductDetail.ParentDistributor,
                    S_OrderType = item.ProductDetail.S_OrderType,
                    R_OrderType = item.ProductDetail.R_OrderType,
                    SaleOrganization = item.ProductDetail.SaleOrganization,
                    DistributionChannel = item.ProductDetail.DistributionChannel,
                    Division = item.ProductDetail.Division,
                    DispatchPlant = item.ProductDetail.DispatchPlant,
                    S_StorageLocation = item.ProductDetail.S_StorageLocation,
                    R_StorageLocation = item.ProductDetail.R_StorageLocation,
                    SalesItemCategory = item.ProductDetail.SalesItemCategory,
                    ReturnItemCategory = item.ProductDetail.ReturnItemCategory,
                    IncomeTax = item.ProductDetail.IncomeTax.ToString(),
                    SalesTax = item.ProductDetail.SalesTax.ToString(),
                    AdditionalSalesTax = item.ProductDetail.AdditionalSalesTax.ToString(),
                    LicenseType = item.ProductDetail.LicenseControl is null ? "" : item.ProductDetail.LicenseControl.LicenseName,
                    FOCQuantityRatio = item.ProductDetail.FOCQuantityRatio.ToString(),
                });
            }
            return ProductMappingModel;
        }
    }
}
