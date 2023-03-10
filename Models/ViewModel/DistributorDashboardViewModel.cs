using Models.Application;
using System;
using System.Collections.Generic;
using System.Text;

namespace Models.ViewModel
{
    public class DistributorDashboardViewModel
    {
        public int InProcessOrderCount { get; set; }
        public string InProcessOrderValue { get; set; }
        public int UnverifiedPaymentAllCount { get; set; }
        public string UnverifiedPaymentAll { get; set; }
        public double UnverifiedPayment { get; set; }
        public int ReturnOrder { get; set; }
        public int Complaint { get; set; }
        public double VerifiedPayment { get; set; }
        public int PendingApproval { get; set; }
        public int Approved { get; set; }
        public int InProcess { get; set; }
        public int PartiallyProcessed { get; set; }
        public int CompletelyProcessed { get; set; }
        public int OnHold { get; set; }
        public int Reject { get; set; }
        public int Draft { get; set; }
        public double SAMIBalance { get; set; }
        public double HealthTekBalance { get; set; }
        public double PhyTekBalance { get; set; }
        public List<OrderWiseComparision> OrderWiseComparision { get; set; }
        public List<PaymentWiseComparision> PaymentWiseComparision { get; set; }
        public List<PaymentWiseStatus> PaymentWiseStatus { get; set; }
        public List<RecentOrder> RecentOrder { get; set; }
        public List<RecentPayment> RecentPayment { get; set; }
        public List<ProductViewModel> ProductViewModelOrder { get; set; }
        public List<ProductViewModel> ProductViewModelQuantity { get; set; }
        public List<Policy> PolicyList { get; set; }
    }
}
