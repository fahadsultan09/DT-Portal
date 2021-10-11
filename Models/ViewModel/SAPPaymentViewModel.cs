namespace Models.ViewModel
{
    public class SAPPaymentViewModel
    {
        public bool IsPaymentAllowedInSAP { get; set; }
        public string PAY_ID { get; set; }
        public string REF { get; set; } //Payment No
        public string COMPANY { get; set; }
        public string AMOUNT { get; set; } 
        public string DISTRIBUTOR { get; set; } //Distributor SAP Code
        public string B_CODE { get; set; } //Bank Code
    }
}
