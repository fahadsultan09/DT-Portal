using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Models.ViewModel
{
    public class SAPOrderPendingQuantity
    {
        public int Id { get; set; }
        public string ProductCode { get; set; }
        public string OrderQuantity { get; set; }
        public string DispatchQuantity { get; set; }
        public string PendingQuantity { get; set; }
        public string ProductName { get; set; }
    }
}