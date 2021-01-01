using System;
using System.Collections.Generic;
using System.Text;

namespace Models.ViewModel
{
    public class AdminDashboardViewModel
    {
        public int PendingOrder { get; set; }
        public string UnverifiedPayment { get; set; }
        public int ReturnOrder { get; set; }
        public int Complaint { get; set; }
        public string VerifiedPayment { get; set; }
        public int PendingApproval { get; set; }
        public int PaymentVerified { get; set; }
        public int InProcess { get; set; }
        public int PartiallyProcessed { get; set; }
        public int CompletelyProcessed { get; set; }
        public int OnHold { get; set; }
        public int Reject { get; set; }
        public List<OrderWiseComparision> OrderWiseComparision { get; set; }
        public List<PaymentWiseComparision> PaymentWiseComparision { get; set; }
        public List<PaymentWiseStatus> PaymentWiseStatus { get; set; }
        public List<RegionWiseOrder> RegionWiseOrder { get; set; }
        public List<DistributorViewModel> DistributorViewModelOrder { get; set; }
        public List<DistributorViewModel> DistributorViewModelPayment { get; set; }
        public List<ProductViewModel> ProductViewModelOrder { get; set; }
        public List<ProductViewModel> ProductViewModelQuantity { get; set; }

    }
}
