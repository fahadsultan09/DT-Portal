using System;
using System.Collections.Generic;
using System.Text;

namespace Models.ViewModel
{
    public class OrderPendingQuantity
    {
        public string ProductCode { get; set; }
        public string OrderQuantity { get; set; }
        public string DispatchQuantity { get; set; }
        public string PendingQuantity { get; set; }
    }
}
