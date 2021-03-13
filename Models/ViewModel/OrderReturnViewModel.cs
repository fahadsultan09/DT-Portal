using Microsoft.AspNetCore.Mvc.Rendering;
using Models.Application;
using System;
using System.Collections.Generic;
using Utility;

namespace Models.ViewModel
{
    public class OrderReturnViewModel
    {
        public List<OrderReturnMaster> OrderReturnMaster { get; set; }
        public int? OrderReturnNo { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public OrderReturnStatus? Status { get; set; }
        public int? DistributorId { get; set; }
        public SelectList DistributorList { get; set; }
    }
}
