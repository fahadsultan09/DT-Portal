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
    public class OrderReturnController : ApiController
    {
        private SAPConnectivity connectivity;
        public OrderReturnController()
        {
            connectivity = new SAPConnectivity();
        }
        [HttpPost]
        public List<SAPOrderStatus> Post(List<OrderStatusViewModel> Table)
        {
            var Data = connectivity.PostOrdertoSAP("ZWAS_ORDER_RETURN_UPLOAD", "ZWAS_IT_DP_VA01UP_ITAB", Table);
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
    }
}
