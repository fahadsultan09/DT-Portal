using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAPConfigurationAPI.Models
{
    public class SAPOrderStatus
    {
        public string ProductCode { get; set; }
        public string SAPOrderNo { get; set; }
        public string OrderStatus { get; set; }
    }
}