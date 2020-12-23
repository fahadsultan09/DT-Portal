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
    public class DistributorController : ApiController
    {
        private SAPConnectivity connectivity;
        public DistributorController()
        {
            connectivity = new SAPConnectivity();
        }
        public List<Teams> Get()
        {
            var table = connectivity.GETTableFromSAP("ZWAS_IT_HRMS_BAPI", "TEAMS");
            List<Teams> list = new List<Teams>();
            for (int i = 0; i < table.RowCount; i++)
            {
                list.Add(new Teams()
                {
                    KDGRP = table[i].GetString("KDGRP"),
                    SPRAS = table[i].GetString("SPRAS"),
                    KTEXT = table[i].GetString("KTEXT")
                });
            }

            return list;
        }
    }
}
