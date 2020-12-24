using SAPConfigurationAPI.BusinessLogic;
using SAPConfigurationAPI.Models;
using System.Collections.Generic;
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
        public List<Distributor> Get()
        {
            var table = connectivity.GETTableFromSAP("ZWAS_IT_HRMS_BAPI", "DISTRIBUTOR");            
            List<Distributor> list = new List<Distributor>();
            for (int i = 0; i < table.RowCount; i++)
            {
                list.Add(new Distributor()
                {
                    DistributorSAPCode = table[i].GetString("KUNNR"),
                    DistributorName = table[i].GetString("NAME1"),
                    City = table[i].GetString("ORT01"),
                    RegionCode = table[i].GetString("REGIO"),
                    CustomerGroup = table[i].GetString("KDGRPT"),
                    DistributorAddress = table[i].GetString("STRAS"),
                    NTN = table[i].GetString("STCD2"),
                    CNIC = table[i].GetString("STCD1"),
                    EmailAddress = table[i].GetString("EMAIL"),
                    MobileNumber = table[i].GetString("TELF1")                    
                });
            }
            return list;
        }
    }
}
