using System;
using System.Collections.Generic;
using System.Text;

namespace Models.ViewModel
{
    public class AccountDashboardViewModel
    {
        public decimal UnverifiedPayment { get; set; }
        public decimal VerifiedPayment { get; set; }
        public decimal TodayVerifiedPayment { get; set; }
        public List<PaymentWiseStatus> PaymentWiseStatus { get; set; }
        public List<PaymentWiseAmount> PaymentWiseAmount { get; set; }
        public List<RecentPayment> RecentPayment { get; set; }
    }
}
