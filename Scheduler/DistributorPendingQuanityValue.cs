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
                List<Distributor> distributors = _distributorBll.Where(x => x.IsActive && !x.IsDeleted);
                List<ProductMaster> productMasters = _ProductMasterBLL.GetAllProductMaster();
                List<ProductDetail> productDetails = _ProductDetailBLL.GetAllProductDetail();
                productDetails.ForEach(x => x.ProductMaster = productMasters.First(y => y.Id == x.ProductMasterId));
                ExtensionUtility.WriteToFile("Count distributors - " + distributors.Count(), FolderName.PendingQuantity, fileName);

                foreach (var item in distributors)
                {
                    List<DistributorPendingQuantity> List = _OrderBLL.GetDistributorOrderPendingQuantitys(item.DistributorSAPCode, _Configuration);
                    ExtensionUtility.WriteToFile("DistributorSAPCode - " + item.DistributorSAPCode, FolderName.PendingQuantity, fileName);
                    var distributorWiseProduct = _DistributorWiseProductDiscountAndPricesBLL.Where(e => e.DistributorId == item.Id && e.ProductDetailId != null);
                    distributorWiseProduct = distributorWiseProduct.Where(x => productDetails.Select(y => y.ProductMaster.SAPProductCode).Contains(x.SAPProductCode)).ToList();
                    List.ForEach(x => x.Distributor = distributors.FirstOrDefault(y => y.Id == item.Id) != null ? distributors.First(y => y.Id == item.Id) : null);
                    List.ForEach(x => x.ProductDetail = productDetails.FirstOrDefault(y => y.ProductMaster.SAPProductCode == x.ProductCode) != null ? productDetails.First(y => y.ProductMaster.SAPProductCode == x.ProductCode) : null);
                    List.ForEach(e => e.DistributorId = item.Id);
                    List.ForEach(e => e.CreatedDate = DateTime.Now);
                    List.ForEach(x => x.Rate = distributorWiseProduct.FirstOrDefault(y => y.SAPProductCode == x.ProductCode) != null ? distributorWiseProduct.First(y => y.SAPProductCode == x.ProductCode).Rate : 0);
                    List.ForEach(x => x.Discount = distributorWiseProduct.FirstOrDefault(y => y.SAPProductCode == x.ProductCode) != null ? distributorWiseProduct.First(y => y.SAPProductCode == x.ProductCode).Discount : 0);
                    List.ForEach(x => x.SalesTax = x.Distributor.IsSalesTaxApplicable ? (x.ProductDetail != null ? x.ProductDetail.SalesTax : 0) : (x.ProductDetail != null ? x.ProductDetail.SalesTax + x.ProductDetail.AdditionalSalesTax : 0));
                    List.ForEach(x => x.IncomeTax = x.Distributor.IsIncomeTaxApplicable ? (x.ProductDetail != null ? x.ProductDetail.IncomeTax : 0) : (x.ProductDetail != null ? x.ProductDetail.IncomeTax * 2 : 0));
                    List.ForEach(x => x.PendingValue = Math.Round(_OrderBLL.CalculatePendingValue(x.PendingQuantity, x.Rate, x.Discount, x.SalesTax, x.IncomeTax), 2));
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
