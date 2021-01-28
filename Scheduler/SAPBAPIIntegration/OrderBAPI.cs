using BusinessLogicLayer.Application;
using BusinessLogicLayer.ErrorLog;
using DataAccessLayer.WorkProcess;
using Models.Application;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Text;
using Utility.HelperClasses;

namespace Scheduler.SAPBAPIIntegration
{
    public class OrderBAPI
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly OrderDetailBLL _OrderDetailBLL;
        private readonly Configuration _Configuration;
        public OrderBAPI(IUnitOfWork unitOfWork, Configuration _configuration)
        {
            _unitOfWork = unitOfWork;
            _OrderDetailBLL = new OrderDetailBLL(_unitOfWork);
            _Configuration = _configuration;
        }

        public bool GetInProcessOrderStatus()
        {
            try
            {
                var a = _OrderDetailBLL.GetInProcessOrderStatus();
                var Client = new RestClient(_Configuration.GetInProcessOrderStatus);
                var request = new RestRequest(Method.POST).AddJsonBody(a, "json");
                IRestResponse response = Client.Execute(request);
                var resp = JsonConvert.DeserializeObject<List<OrderDetail>>(response.Content);
                if (resp == null)
                {
                    return true;
                }
                else
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                new ErrorLogBLL(_unitOfWork).AddExceptionLog(ex);
                return false;
            }
        }
    }
}
