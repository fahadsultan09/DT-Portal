using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAPConfigurationAPI.Models
{
    public class OrderPendingQuantity
    {
        public string ProductCode { get; set; }
        public string OrderQuantity { get; set; }
        public string DispatchQuantity { get; set; }
        public string PendingQuantity { get; set; }


    }
}