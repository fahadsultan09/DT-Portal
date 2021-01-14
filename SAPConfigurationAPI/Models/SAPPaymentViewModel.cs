using System;
using System.Collections.Generic;
using System.Text;

namespace SAPConfigurationAPI.ViewModel
{
    public class SAPPaymentViewModel
    {
        public string PAY_ID { get; set; }
        public string REF { get; set; } //Payment No
        public string COMPANY { get; set; }
        public string AMOUNT { get; set; } 
        public string DISTRIBUTOR { get; set; } //Distributor SAP Code
        public string B_CODE { get; set; } //Bank Code
    }
}
