using BusinessLogicLayer.Application;
using BusinessLogicLayer.ApplicationSetup;
using BusinessLogicLayer.ErrorLog;
using DataAccessLayer.WorkProcess;
using Models.Application;
using System;
using System.Collections.Generic;
using System.Linq;
using Utility;
using Utility.HelperClasses;
using static Utility.Constant.Common;

namespace Scheduler
{
    public class DistributorPendingQuanityValue
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly Configuration _Configuration;
        private readonly OrderBLL _OrderBLL;
        private readonly DistributorBLL _distributorBll;
        private readonly DistributorPendingQuanityBLL _DistributorPendingQuanityBLL;
        private readonly DistributorWiseProductDiscountAndPricesBLL _DistributorWiseProductDiscountAndPricesBLL;
        private readonly ProductMasterBLL _ProductMasterBLL;
        private readonly ProductDetailBLL _ProductDetailBLL;
        public DistributorPendingQuanityValue(IUnitOfWork unitOfWork, Configuration configuration)
        {
            _unitOfWork = unitOfWork;
            _Configuration = configuration;
            _Configuration = configuration;
            _OrderBLL = new OrderBLL(_unitOfWork);
            _distributorBll = new DistributorBLL(_unitOfWork);
            _DistributorWiseProductDiscountAndPricesBLL = new DistributorWiseProductDiscountAndPricesBLL(_unitOfWork);
            _DistributorPendingQuanityBLL = new DistributorPendingQuanityBLL(_unitOfWork);
            _ProductMasterBLL = new ProductMasterBLL(_unitOfWork);
            _ProductDetailBLL = new ProductDetailBLL(_unitOfWork);

        }
        public void AddDistributorPendingQuantity(string fileName)
        {
            try
            {
                List<DistributorPendingQuantity> DistributorPendingQuantitys = new List<DistributorPendingQuantity>();
                var distributors = _distributorBll.Where(x => x.IsActive && !x.IsDeleted);
                List<ProductMaster> productMasters = _ProductMasterBLL.GetAllProductMaster();
                List<ProductDetail> productDetails = _ProductDetailBLL.GetAllProductDetail();
                productDetails.ForEach(x => x.ProductMaster = productMasters.First(y => y.Id == x.ProductMasterId));
                ExtensionUtility.WriteToFile("Count distributors - " + distributors.Count(), FolderName.PendingQuantity, fileName);

                foreach (var item in distributors)
                {
                    List<DistributorPendingQuantity> List = _OrderBLL.GetDistributorOrderPendingQuantitys(item.DistributorSAPCode, _Configuration);
                    ExtensionUtility.WriteToFile("DistributorSAPCode - " + item.DistributorSAPCode, FolderName.PendingQuantity, fileName);
                    var distributorProduct = _DistributorWiseProductDiscountAndPricesBLL.Where(e => e.DistributorId == item.Id && e.ProductDetailId != null);
                    distributorProduct = distributorProduct.Where(x => productDetails.Select(y => y.ProductMaster.SAPProductCode).Contains(x.SAPProductCode)).ToList();
                    List.ForEach(x => x.ProductDetail = productDetails.FirstOrDefault(y => y.ProductMaster.SAPProductCode == x.ProductCode) != null ? productDetails.FirstOrDefault(y => y.ProductMaster.SAPProductCode == x.ProductCode) : null);
                    List.ForEach(e => e.DistributorId = item.Id);
                    List.ForEach(e => e.CreatedDate = DateTime.Now);
                    List.ForEach(x => x.Rate = distributorProduct.FirstOrDefault(y => y.SAPProductCode == x.ProductCode) != null ? distributorProduct.First(y => y.SAPProductCode == x.ProductCode).Rate : 0);
                    List.ForEach(x => x.Discount = distributorProduct.FirstOrDefault(y => y.SAPProductCode == x.ProductCode) != null ? distributorProduct.First(y => y.SAPProductCode == x.ProductCode).Discount : 0);
                    List.ForEach(x => x.PendingValue = (x.PendingQuantity * x.Rate) - (x.PendingQuantity * x.Rate / 100 * (-1 * x.Discount))
                    + (x.PendingQuantity * x.Rate / 100 * (x.ProductDetail == null ? 0 : x.ProductDetail.SalesTax))
                    + (x.PendingQuantity * x.Rate / 100 * (x.ProductDetail == null ? 0 : x.ProductDetail.IncomeTax))
                    + (x.PendingQuantity * x.Rate / 100 * (x.ProductDetail == null ? 0 : x.ProductDetail.AdditionalSalesTax)));
                    ExtensionUtility.WriteToFile("List - " + List.Count(), FolderName.PendingQuantity, fileName);
                    DistributorPendingQuantitys.AddRange(List);
                }
                _unitOfWork.Begin();
                ExtensionUtility.WriteToFile("Begin - " + DistributorPendingQuantitys.Count(), FolderName.PendingQuantity, fileName);
                _DistributorPendingQuanityBLL.AddRange(DistributorPendingQuantitys.Where(x => !string.IsNullOrEmpty(x.ProductCode)).ToList(), 0);
                _unitOfWork.Commit();
                ExtensionUtility.WriteToFile("Commit - " + DistributorPendingQuantitys.Count(), FolderName.PendingQuantity, fileName);
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                new ErrorLogBLL(_unitOfWork).AddSchedulerExceptionLog(ex);
            }
        }
    }
}
