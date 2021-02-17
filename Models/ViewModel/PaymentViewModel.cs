using Microsoft.AspNetCore.Mvc.Rendering;
using Models.Application;
using System;
using System.Collections.Generic;
using System.Text;
using Utility;

namespace Models.ViewModel
{
    public class PaymentViewModel
    {
        public List<PaymentMaster> PaymentMaster { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public PaymentStatus? Status { get; set; }
        public int? DistributorId { get; set; }
        public int? PaymentNo { get; set; }
        public SelectList DistributorList { get; set; }
    }
}
