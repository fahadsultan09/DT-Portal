using System;
using System.Collections.Generic;
using System.Text;

namespace SAPConfigurationAPI.Models
{
    public class SAPPaymentStatus
    {
        public string SAPCompanyCode { get; set; }
        public string SAPFiscalYear { get; set; }
        public string SAPDocumentNumber { get; set; }
    }
}
