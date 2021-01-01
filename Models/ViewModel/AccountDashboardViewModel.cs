using System;
using System.Collections.Generic;
using System.Text;

namespace Models.ViewModel
{
    public class AccountDashboardViewModel
    {
        public string UnverifiedPayment { get; set; }
        public string VerifiedPayment { get; set; }
        public string TodayVerifiedPayment { get; set; }
        public List<PaymentWiseStatus> PaymentWiseStatus { get; set; }
        public List<PaymentWiseComparision> PaymentWiseComparision { get; set; }
        public List<PaymentWiseAmount> PaymentWiseAmount { get; set; }
        public List<RecentPayment> RecentPayment { get; set; }
    }
}
