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
    public class BalanceController : ApiController
    {
        private readonly SAPConnectivity connectivity;
        public BalanceController()
        {
            connectivity = new SAPConnectivity();
        }
       
        public DistributorBalance GetBalance(string DistributorId)
        {
            var Data = connectivity.GETBalanceFromSAP("ZWAS_IT_DP_DIST_BALANCE_BAPI", "DISTRIBUTOR", DistributorId);
            return Data;
        }
    }
}
