using Newtonsoft.Json;

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
            ZWAS_IT_DP_PRICE_DISCOUNT = new ZWAS_IT_DP_PRICE_DISCOUNT();
            ZWAS_BI_SALES_QUERY_BAPI = new ZWAS_BI_SALES_QUERY_BAPI();
            ZWAS_PAYMENT_BAPI_DP = new ZWAS_PAYMENT_BAPI_DP();
            ZWASDPPENDINGORDERBAPIResponse = new ZWASDPPENDINGORDERBAPIResponse();
        }
        [JsonProperty("ZWAS_IT_DP_DIST_BALANCE_BAPI.Response")]
        public ZWASITDPDISTBALANCEBAPIResponse ZWASITDPDISTBALANCEBAPIResponse { get; set; }
        [JsonProperty("ZWAS_IT_HRMS_BAPI.Response")]
        public ZWASITHRMSBAPIResponse ZWASITHRMSBAPIResponse { get; set; }
        [JsonProperty("ZWAS_IT_DP_PRICE_DISCOUNT.Response")]
        public ZWAS_IT_DP_PRICE_DISCOUNT ZWAS_IT_DP_PRICE_DISCOUNT { get; set; }
        [JsonProperty("ZWAS_BI_SALES_QUERY_BAPI.Response")]
        public ZWAS_BI_SALES_QUERY_BAPI ZWAS_BI_SALES_QUERY_BAPI { get; set; }
        [JsonProperty("ZWAS_PAYMENT_BAPI_DP.Response")]
        public ZWAS_PAYMENT_BAPI_DP ZWAS_PAYMENT_BAPI_DP { get; set; }
        [JsonProperty("ZWAS_DP_PENDING_ORDER_BAPI.Response")]
        public ZWASDPPENDINGORDERBAPIResponse ZWASDPPENDINGORDERBAPIResponse { get; set; }
    }
    public class item
    {
        public string KUNNR { get; set; }
        public string NAME1 { get; set; }
        public string ORT01 { get; set; }
        public string REGIO { get; set; }
        //public string KDGRP { get; set; }
        public string KDGRPT { get; set; }
        public string STRAS { get; set; }
        public string STCD2 { get; set; }
        public string STCD1 { get; set; }
        public string EMAIL { get; set; }
        //public string ADRNR { get; set; }
        public string TELF1 { get; set; }
        public string MATNR { get; set; }
        public string MAKTX { get; set; }
        //public string MVGR1 { get; set; }
        //public string MVGR1T { get; set; }
        //public string MVGR2 { get; set; }
        public string MVGR2T { get; set; }
        //public string MVGR3 { get; set; }
        //public string MVGR3T { get; set; }
        //public object MVGR4 { get; set; }
        public string MVGR4T { get; set; }
        public string MVGR5 { get; set; }
        //public string MVGR5T { get; set; }
        public string WRKST { get; set; }
        public string KBETR { get; set; }
        public string CARTON { get; set; }
        public string DISCOUNT { get; set; }
        public string MTPOS { get; set; }
        public string SF { get; set; }
        //public string MANDT { get; set; }
        //public string SPRAS { get; set; }
        //public string LAND1 { get; set; }
        //public string BLAND { get; set; }
        //public string BEZEI { get; set; }
        //public string KTEXT { get; set; }
        //public string BZIRK { get; set; }
        //public string BZTXT { get; set; }
        public string NETWR { get; set; }
        public string VKORG { get; set; }
        public string KWMENG { get; set; }
        public string LFIMG { get; set; }
        public string PENDING { get; set; }
        public string KWMENG_AM { get; set; }
        public string LFIMG_AM { get; set; }
        public string PENDING_AM { get; set; }
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
    public class ZWAS_IT_DP_PRICE_DISCOUNT
    {
        public PRICES PRICES { get; set; }
    }
    public class PRICES
    {
        public item[] item { get; set; }
    }
    public class ZWAS_BI_SALES_QUERY_BAPI
    {
        public PENDING PENDING { get; set; }
    }
    public class PENDING
    {
        public item[] item { get; set; }
    }

    public class ZWAS_PAYMENT_BAPI_DP
    {
        public string COMPANYY { get; set; }
        public string DOCUMENT { get; set; }
        public string FISCAL { get; set; }
    }
    public class ZWASDPPENDINGORDERBAPIResponse
    {
        public PENDING PENDING { get; set; }
    }
}
