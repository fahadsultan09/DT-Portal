using Models.Application;
using System;
using System.Collections.Generic;
using System.Text;
using Utility;

namespace Models.ViewModel
{
    public class PaymentSearch : Search
    {
        public PaymentStatus? Status { get; set; }
        public int? PaymentNo { get; set; }
        public List<PaymentMaster> paymentMasters { get; set; }
    }
}
