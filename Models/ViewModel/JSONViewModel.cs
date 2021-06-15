using Newtonsoft.Json;
using System.Collections.Generic;

namespace Models.ViewModel
{
    public class ZWASITDPDISTBALANCEBAPIResponse
    {
        public double BAL_HTL { get; set; }
        public double BAL_SAMI { get; set; }
    }
    public class Root
    {
        public Root()
        {
            ZWASITDPDISTBALANCEBAPIResponse = new ZWASITDPDISTBALANCEBAPIResponse();
            ZWASITHRMSBAPIResponse = new ZWASITHRMSBAPIResponse();
        }
        [JsonProperty("ZWAS_IT_DP_DIST_BALANCE_BAPI.Response")]
        public ZWASITDPDISTBALANCEBAPIResponse ZWASITDPDISTBALANCEBAPIResponse { get; set; }
        [JsonProperty("ZWASITHRMSBAPIResponse")]
        public ZWASITHRMSBAPIResponse ZWASITHRMSBAPIResponse { get; set; }
    }
    public class item
    {
        public string KUNNR { get; set; }
    }
    public class DISTRIBUTOR
    {
        public item[] item { get; set; }
    }

    public class PRODUCTS
    {
        public item[] item { get; set; }
    }
    public class ZWASITHRMSBAPIResponse
    {
        public ZWASITHRMSBAPIResponse()
        {
            DISTRIBUTOR = new DISTRIBUTOR();
            PRODUCTS = new PRODUCTS();
        }
        public DISTRIBUTOR DISTRIBUTOR { get; set; }
        public PRODUCTS PRODUCTS { get; set; }
    }
}
