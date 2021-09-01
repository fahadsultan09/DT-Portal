using BusinessLogicLayer.Application;
using BusinessLogicLayer.ErrorLog;
using DataAccessLayer.WorkProcess;
using Models.ViewModel;
using OrderStatusRequest;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Xml;
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
        public void GetInProcessOrderProductStatus(string GetInProcessOrderStatus)
        {
            try
            {
                BasicHttpBinding binding = new BasicHttpBinding
                {
                    SendTimeout = TimeSpan.FromSeconds(100000),
                    MaxBufferSize = int.MaxValue,
                    MaxReceivedMessageSize = int.MaxValue,
                    AllowCookies = true,
                    ReaderQuotas = XmlDictionaryReaderQuotas.Max,
                };
                binding.Security.Mode = BasicHttpSecurityMode.TransportCredentialOnly;
                binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Basic;
                EndpointAddress address = new EndpointAddress(GetInProcessOrderStatus);
                Ord_status_Request_OUTClient client = new Ord_status_Request_OUTClient(binding, address);
                client.ClientCredentials.UserName.UserName = _Configuration.POUserName;
                client.ClientCredentials.UserName.Password = _Configuration.POPassword;
                if (client.InnerChannel.State == CommunicationState.Faulted)
                {
                }
                else
                {
                    client.OpenAsync();
                }
                Ord_status_Request_IN disPortalRequestIn = new Ord_status_Request_IN();
                disPortalRequestIn.ORDERS = PlaceProductOrderStatus().ToArray();
                if (disPortalRequestIn.ORDERS != null)
                {
                    ZWAS_IT_DP_ORDER_STATUS_BAPIResponse ZST_PENDING_ORDER_OUTList = client.Ord_status_Request_OUT(disPortalRequestIn);
                    client.CloseAsync();
                    List<SAPOrderStatus> SAPOrderStatus = new List<SAPOrderStatus>();
                    if (ZST_PENDING_ORDER_OUTList != null)
                    {
                        for (int i = 0; i < ZST_PENDING_ORDER_OUTList.STATUSES.Length; i++)
                        {
                            SAPOrderStatus.Add(new SAPOrderStatus()
                            {
                                SAPOrderNo = ZST_PENDING_ORDER_OUTList.STATUSES[i].VBELN.ToString(),
                                OrderStatus = ZST_PENDING_ORDER_OUTList.STATUSES[i].GBSTK.ToString(),
                            });
                        }
                        _OrderDetailBLL.UpdateProductOrderStatus(SAPOrderStatus);
                    }
                }
            }
            catch (Exception ex)
            {
                new ErrorLogBLL(_unitOfWork).AddSchedulerExceptionLog(ex);
            }
        }
        public void GetInProcessOrderReturnProductStatus(string GetInProcessOrderStatus)
        {
            try
            {
                BasicHttpBinding binding = new BasicHttpBinding
                {
                    SendTimeout = TimeSpan.FromSeconds(100000),
                    MaxBufferSize = int.MaxValue,
                    MaxReceivedMessageSize = int.MaxValue,
                    AllowCookies = true,
                    ReaderQuotas = XmlDictionaryReaderQuotas.Max,
                };
                binding.Security.Mode = BasicHttpSecurityMode.TransportCredentialOnly;
                binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Basic;
                EndpointAddress address = new EndpointAddress(GetInProcessOrderStatus);
                Ord_status_Request_OUTClient client = new Ord_status_Request_OUTClient(binding, address);
                client.ClientCredentials.UserName.UserName = _Configuration.POUserName;
                client.ClientCredentials.UserName.Password = _Configuration.POPassword;
                if (client.InnerChannel.State == CommunicationState.Faulted)
                {
                }
                else
                {
                    client.OpenAsync();
                }
                Ord_status_Request_IN disPortalRequestIn = new Ord_status_Request_IN();
                disPortalRequestIn.ORDERS = PlaceProductOrderReturnStatus().ToArray();
                if (disPortalRequestIn.ORDERS != null)
                {
                    ZWAS_IT_DP_ORDER_STATUS_BAPIResponse ZST_PENDING_ORDER_OUTList = client.Ord_status_Request_OUT(disPortalRequestIn);
                    client.CloseAsync();
                    List<SAPOrderStatus> SAPOrderReturnStatus = new List<SAPOrderStatus>();
                    if (ZST_PENDING_ORDER_OUTList != null)
                    {
                        for (int i = 0; i < ZST_PENDING_ORDER_OUTList.STATUSES.Length; i++)
                        {
                            SAPOrderReturnStatus.Add(new SAPOrderStatus()
                            {
                                SAPOrderNo = ZST_PENDING_ORDER_OUTList.STATUSES[i].VBELN.ToString(),
                                OrderStatus = ZST_PENDING_ORDER_OUTList.STATUSES[i].GBSTK.ToString(),
                            });
                        }
                        _OrderReturnDetailBLL.UpdateProductOrderStatus(SAPOrderReturnStatus);
                    }
                }
            }
            catch (Exception ex)
            {
                new ErrorLogBLL(_unitOfWork).AddSchedulerExceptionLog(ex);
            }
        }
        public List<Ord_status_Request_main> PlaceProductOrderStatus()
        {
            List<Ord_status_Request_main> model = new List<Ord_status_Request_main>();
            var InProcessOrderStatus = _OrderDetailBLL.GetInProcessOrderStatus();
            foreach (var item in InProcessOrderStatus)
            {
                model.Add(new Ord_status_Request_main()
                {
                    VBELN = item.SAPOrderNo.ToString(),
                });
            }
            return model;
        }
        public List<Ord_status_Request_main> PlaceProductOrderReturnStatus()
        {
            List<Ord_status_Request_main> model = new List<Ord_status_Request_main>();
            var InProcessOrderStatus = _OrderReturnDetailBLL.GetInProcessOrderReturnStatus();
            foreach (var item in InProcessOrderStatus)
            {
                model.Add(new Ord_status_Request_main()
                {
                    VBELN = item.SAPOrderNo.ToString(),
                });
            }
            return model;
        }
    }
}
