using SAPConfigurationAPI.BusinessLogic;
using SAPConfigurationAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

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
            var Data = connectivity.GetOrderPendingQuantity("ZWAS_DP_PENDING_ORDER_BAPI", DistributorId);
            List<OrderPendingQuantity> list = new List<OrderPendingQuantity>();
            if (Data != null)
            {
                for (int i = 0; i < Data.RowCount; i++)
                {
                    list.Add(new OrderPendingQuantity()
                    {
                        ProductCode = Data[i].GetString("MATNR").TrimStart(new char[] { '0' }),
                        OrderQuantity = Data[i].GetString("KWMENG"),
                        DispatchQuantity = Data[i].GetString("LFIMG"),
                        PendingQuantity = Data[i].GetString("PENDING")
                    });
                }
            }
            return list;
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
