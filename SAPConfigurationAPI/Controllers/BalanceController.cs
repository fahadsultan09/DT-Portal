using SAPConfigurationAPI.BusinessLogic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SAPConfigurationAPI.Controllers
{
    public class BalanceController : ApiController
    {
        private SAPConnectivity connectivity;
        public BalanceController()
        {
            connectivity = new SAPConnectivity();
        }
       
        public decimal GetBalance()
        {
            var table = connectivity.GETBalanceFromSAP("ZWAS_IT_DP_DIST_BALANCE_BAPI", "DISTRIBUTOR");
            return 0;
        }
    }
}
