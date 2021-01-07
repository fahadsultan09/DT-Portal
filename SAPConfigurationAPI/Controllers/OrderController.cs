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
        public List<OrderStatusViewModel> GetBalance(List<OrderStatusViewModel> Table)
        {
            var Data = connectivity.PostOrdertoSAP("ZWAS_IT_DP_ORDER_CREATE_BAPI", "ZWAS_IT_DP_VA01UP_ITAB", Table);
            return Table;
        }
    }
}
