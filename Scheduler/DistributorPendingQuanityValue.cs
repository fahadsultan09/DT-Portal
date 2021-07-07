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

        public DistributorPendingQuanityValue(IUnitOfWork unitOfWork, Configuration configuration)
        {
            _unitOfWork = unitOfWork;
            _Configuration = configuration;
            _OrderBLL = new OrderBLL(_unitOfWork);
            _distributorBll = new DistributorBLL(_unitOfWork);
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
                    List.ForEach(e => e.DistributorId = item.Id);
                    List.ForEach(e => e.CreatedDate = DateTime.Now);
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
