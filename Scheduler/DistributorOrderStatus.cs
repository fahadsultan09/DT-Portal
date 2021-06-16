using BusinessLogicLayer.Application;
using BusinessLogicLayer.ErrorLog;
using DataAccessLayer.WorkProcess;
using Models.ViewModel;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using Utility.HelperClasses;

namespace Scheduler
{
    public class DistributorOrderStatus
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly OrderDetailBLL _OrderDetailBLL;
        private readonly OrderReturnDetailBLL _OrderReturnDetailBLL;
        private readonly Configuration _Configuration;
        public DistributorOrderStatus(IUnitOfWork unitOfWork, Configuration _configuration)
        {
            _unitOfWork = unitOfWork;
            _OrderDetailBLL = new OrderDetailBLL(_unitOfWork);
            _OrderReturnDetailBLL = new OrderReturnDetailBLL(_unitOfWork);
            _Configuration = _configuration;
        }

        public void GetInProcessOrderProductStatus()
        {
            try
            {
                var InProcessOrderStatus = _OrderDetailBLL.GetInProcessOrderStatus();
                if (InProcessOrderStatus != null)
                {
                    var Client = new RestClient(_Configuration.GetInProcessOrderStatus);
                    var request = new RestRequest(Method.POST).AddJsonBody(InProcessOrderStatus, "json");
                    IRestResponse response = Client.Execute(request);
                    var resp = JsonConvert.DeserializeObject<List<SAPOrderStatus>>(response.Content);
                    if (resp != null && resp.Count > 0)
                    {
                        _OrderDetailBLL.UpdateProductOrderStatus(resp);
                    }
                }
            }
            catch (Exception ex)
            {
                new ErrorLogBLL(_unitOfWork).AddExceptionLog(ex);
            }
        }
        public void GetInProcessOrderReturnProductStatus()
        {
            try
            {
                var InProcessOrderReturnStatus = _OrderReturnDetailBLL.GetInProcessOrderReturnStatus();
                if (InProcessOrderReturnStatus != null)
                {
                    var Client = new RestClient(_Configuration.GetInProcessOrderStatus);
                    var request = new RestRequest(Method.POST).AddJsonBody(InProcessOrderReturnStatus, "json");
                    IRestResponse response = Client.Execute(request);
                    List<SAPOrderStatus> resp = JsonConvert.DeserializeObject<List<SAPOrderStatus>>(response.Content);
                    if (resp != null && resp.Count > 0)
                    {
                        _OrderReturnDetailBLL.UpdateProductOrderStatus(resp);
                    }
                }
            }
            catch (Exception ex)
            {
                new ErrorLogBLL(_unitOfWork).AddExceptionLog(ex);
            }
        }
    }
}
