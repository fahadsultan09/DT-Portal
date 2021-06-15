using SAPConfigurationAPI.BusinessLogic;
using SAPConfigurationAPI.DPPendingOrdersRequest;
using SAPConfigurationAPI.Models;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Web.Http;
using System.Xml;

namespace SAPConfigurationAPI.Controllers
{
    public class OrderController : ApiController
    {
        private readonly SAPConnectivity connectivity;
        public OrderController()
        {
            connectivity = new SAPConnectivity();
        }
        [HttpPost]
        public List<SAPOrderStatus> GetBalance(List<OrderStatusViewModel> Table)
        {
            var Data = connectivity.PostOrdertoSAP("ZWAS_IT_DP_ORDER_CREATE_BAPI", "ZWAS_IT_DP_VA01UP_ITAB", Table);
            List<SAPOrderStatus> list = new List<SAPOrderStatus>();
            for (int i = 0; i < Data.RowCount; i++)
            {
                list.Add(new SAPOrderStatus()
                {
                    ProductCode = Data[i].GetString("MATNR"),
                    SAPOrderNo = Data[i].GetString("VBELN")
                });
            }
            return list;
        }
        [HttpGet]
        [Route("api/Order/GetOrderPendingQuantity")]
        public List<OrderPendingQuantity> GetOrderPendingQuantity(string DistributorId)
        {
            //var Data = connectivity.GetOrderPendingQuantity("ZWAS_DP_PENDING_ORDER_BAPI", DistributorId);
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

            EndpointAddress address = new EndpointAddress("http://s049sappodev.samikhi.com:51000/XISOAPAdapter/MessageServlet?senderParty=&senderService=NSAP_DEV&receiverParty=&receiverService=&interface=DPPendingOrdersRequest_Out&interfaceNamespace=https://www.sami.com/DPPendingOrders");
            DPPendingOrdersRequest_OutClient client = new DPPendingOrdersRequest_OutClient(binding, address);
            client.ClientCredentials.UserName.UserName = "SAMI_PO";
            client.ClientCredentials.UserName.Password = "wasay123";
            client.Open();
            ZST_PENDING_ORDER_OUT[] Data = client.DPPendingOrdersRequest_Out(DistributorId);
            List<OrderPendingQuantity> OrderPendingQuantityList = new List<OrderPendingQuantity>();
            if (Data != null)
            {
                for (int i = 0; i < Data.Length; i++)
                {
                    OrderPendingQuantityList.Add(new OrderPendingQuantity()
                    {
                        ProductCode = Data[i].MATNR.TrimStart(new char[] { '0' }),
                        OrderQuantity = Data[i].KWMENG.ToString(),
                        DispatchQuantity = Data[i].LFIMG.ToString(),
                        PendingQuantity = Data[i].PENDING.ToString(),
                    });
                }
            }
            return OrderPendingQuantityList;
        }
        [HttpPost]
        [Route("api/Order/GetInProcessOrderStatus")]
        public List<SAPOrderStatus> GetPendingOrderStatus(List<SAPOrderStatus> OrderNoList)
        {
            var Data = connectivity.GetPendingOrderStatus("ZWAS_IT_DP_ORDER_STATUS_BAPI", "VBAK", OrderNoList);
            List<SAPOrderStatus> list = new List<SAPOrderStatus>();
            if (Data != null)
            {
                for (int i = 0; i < Data.RowCount; i++)
                {
                    list.Add(new SAPOrderStatus()
                    {
                        SAPOrderNo = Data[i].GetString("VBELN"),
                        OrderStatus = Data[i].GetString("GBSTK"),
                    });
                }
            }
            return list;
        }
        [HttpPost]
        [Route("api/Order/GetPendingOrderValue")]
        public List<OrderPendingValue> GetPendingOrderValue(string DistributorId)
        {
            var Data = connectivity.GetPendingOrderValue("ZWAS_BI_SALES_QUERY_BAPI", DistributorId);
            List<OrderPendingValue> list = new List<OrderPendingValue>();
            if (Data != null)
            {
                for (int i = 0; i < Data.RowCount; i++)
                {
                    list.Add(new OrderPendingValue()
                    {
                        CompanyCode = Data[i].GetString("VKORG"),
                        PendingValue = Data[i].GetString("NETWR"),
                    });
                }
            }
            return list;
        }
        [HttpPost]
        [Route("api/Order/GetValue")]
        public string GetValue(string value1, string value12)
        {

            return value1 + " " + value12;
        }
    }
}
