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

        //public List<SAPOrderStatus> Post(int OrderReturnId)
        //{
           
        //}
    }
}
