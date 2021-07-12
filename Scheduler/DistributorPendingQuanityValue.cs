using BusinessLogicLayer.Application;
using BusinessLogicLayer.ApplicationSetup;
using BusinessLogicLayer.ErrorLog;
using DataAccessLayer.WorkProcess;
using Models.Application;
using Models.ViewModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Utility.HelperClasses;

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
        public DistributorPendingQuanityValue(IUnitOfWork unitOfWork, Configuration configuration)
        {
            _unitOfWork = unitOfWork;
            _Configuration = configuration;
            _Configuration = configuration;
            _OrderBLL = new OrderBLL(_unitOfWork);
            _distributorBll = new DistributorBLL(_unitOfWork);
            _DistributorWiseProductDiscountAndPricesBLL = new DistributorWiseProductDiscountAndPricesBLL(_unitOfWork);
            _DistributorPendingQuanityBLL = new DistributorPendingQuanityBLL(_unitOfWork);

        }
        public void AddDistributorPendingQuantity()
        {
            try
            {
                List<DistributorPendingQuantity> DistributorPendingQuantitys = new List<DistributorPendingQuantity>();
                var distributors = _distributorBll.Where(x => x.IsActive && !x.IsDeleted);

                foreach (var item in distributors)
                {
                    List<DistributorPendingQuantity> List = _OrderBLL.GetDistributorOrderPendingQuantitys(item.DistributorSAPCode, _Configuration);
                    var distributorProduct = _DistributorWiseProductDiscountAndPricesBLL.Where(e => e.DistributorId == item.Id && e.ProductDetailId != null);
                    List.ForEach(e => e.DistributorId = item.Id);
                    List.ForEach(e => e.CreatedDate = DateTime.Now);
                    List.ForEach(x => x.Rate = distributorProduct.FirstOrDefault(y => y.SAPProductCode == x.ProductCode) != null ? distributorProduct.First(y => y.SAPProductCode == x.ProductCode).Rate : 0);
                    List.ForEach(x => x.Discount = distributorProduct.FirstOrDefault(y => y.SAPProductCode == x.ProductCode) != null ? distributorProduct.First(y => y.SAPProductCode == x.ProductCode).Discount : 0);
                    List.ForEach(x => x.PendingValue = (x.PendingQuantity * x.Rate) - (x.PendingQuantity * x.Rate / 100 * (-1 * x.Discount)));
                    DistributorPendingQuantitys.AddRange(List);
                }
                _unitOfWork.Begin();
                _DistributorPendingQuanityBLL.AddRange(DistributorPendingQuantitys.Where(x => !string.IsNullOrEmpty(x.ProductCode)).ToList(), 0);
                _unitOfWork.Commit();
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                new ErrorLogBLL(_unitOfWork).AddSchedulerExceptionLog(ex);
            }
        }
    }
}
