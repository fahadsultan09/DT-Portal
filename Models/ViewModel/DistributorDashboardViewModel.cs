using System;
using System.Collections.Generic;
using System.Text;

namespace Models.ViewModel
{
    public class DistributorDashboardViewModel
    {
        public int InProcessOrder { get; set; }
        public string UnverifiedPayment { get; set; }
        public int ReturnOrder { get; set; }
        public int Complaint { get; set; }
        public decimal VerifiedPayment { get; set; }
        public decimal OutstandingBalance { get; set; }
        public int PendingApproval { get; set; }
        public int PaymentVerified { get; set; }
        public int InProcess { get; set; }
        public int PartiallyProcessed { get; set; }
        public int CompletelyProcessed { get; set; }
        public int OnHold { get; set; }
        public int Reject { get; set; }
        public int Draft { get; set; }
        public List<OrderWiseStatus> OrderWiseStatus { get; set; }
        public List<PaymentWiseStatus> PaymentWiseStatus { get; set; }
        public List<RecentOrder> RecentOrder { get; set; }
        public List<RecentPayment> RecentPayment { get; set; }
        public List<ProductViewModel> ProductViewModelOrder { get; set; }
        public List<ProductViewModel> ProductViewModelQuantity { get; set; }
    }
}
