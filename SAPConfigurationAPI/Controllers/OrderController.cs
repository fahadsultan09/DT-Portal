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
        private SAPConnectivity connectivity;
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
            for (int i = 0; i < Data.RowCount; i++)
            {
                list.Add(new OrderPendingQuantity()
                {
                    ProductCode = Data[i].GetString("MATNR"),
                    OrderQuantity = Data[i].GetString("KWMENG"),    
                    DispatchQuantity = Data[i].GetString("LFIMG"),
                    PendingQuantity = Data[i].GetString("PENDING")
                });
            }
            return list;
        }
        [HttpPost]
        [Route("api/Order/GetInProcessOrderStatus")]
        public List<SAPOrderStatus> GetPendingOrderStatus(List<SAPOrderStatus> OrderNoList)
        {
            var Data = connectivity.GetPendingOrderStatus("ZWAS_IT_DP_ORDER_STATUS_BAPI", "VBAK", OrderNoList);
            List<SAPOrderStatus> list = new List<SAPOrderStatus>();
            for (int i = 0; i < Data.RowCount; i++)
            {
                list.Add(new SAPOrderStatus()
                {
                    SAPOrderNo = Data[i].GetString("VBELN"),
                    OrderStatus = Data[i].GetString("GBSTK"),
                });
            }
            return list;
        }
    }
}
