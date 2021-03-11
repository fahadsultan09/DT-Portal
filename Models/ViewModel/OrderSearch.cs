using Microsoft.AspNetCore.Mvc.Rendering;
using Models.Application;
using System;
using System.Collections.Generic;
using System.Text;
using Utility;

namespace Models.ViewModel
{
    public class OrderSearch : Search
    {
        public OrderStatus? Status { get; set; }
        public int? OrderNo { get; set; }
        public List<OrderMaster> OrderMaster { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public int? DistributorId { get; set; }
        public SelectList DistributorList { get; set; }
    }
}
