using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Text;
using Utility;

namespace Models.ViewModel
{
    public class OrderSearch
    {
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public OrderStatus? Status { get; set; }
        public int? Distributor { get; set; }
        public SelectList DistributorList { get; set; }
    }
}
