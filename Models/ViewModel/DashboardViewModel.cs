using System.Collections.Generic;
using Utility;

namespace Models.ViewModel
{
    public class OrderWiseComparision
    {
        public string Month { get; set; }
        public int LastYear { get; set; }
        public int CurrentYear { get; set; }
    }
    public class PaymentWiseComparision
    {
        public string Month { get; set; }
        public double LastYear { get; set; }
        public double CurrentYear { get; set; }
    }
    public class PaymentWiseStatus
    {
        public string Month { get; set; }
        public double VerifiedPayment { get; set; }
        public double UnverifiedPayment { get; set; }
    }
    public class RegionWiseOrder
    {
        public string Region { get; set; }
        public int OrderCount { get; set; }
    }
    public class PaymentWiseAmount
    {
        public string PaymentMode { get; set; }
        public double Amount { get; set; }
    }
    public class DistributorViewModel
    {
        public string DistributorName { get; set; }
        public int OrderCount { get; set; }
        public double Payment { get; set; }
    }
    public class ProductViewModel
    {
        public string ProductName { get; set; }
        public string CurrentPrice { get; set; }
        public int OrderCount { get; set; }
        public int Quantity { get; set; }
    }
    public class RecentOrder
    {
        public int OrderNo { get; set; }
        public string DistributorName { get; set; }
        public double Amount { get; set; }
        public OrderStatus Status { get; set; }
    }
    public class RecentPayment
    {
        public int PaymentId { get; set; }
        public string PaymentMode { get; set; }
        public string DistributorName { get; set; }
        public double Amount { get; set; }
        public PaymentStatus Status { get; set; }
    }
    public class AccountRecentPaymentStatus 
    {
        public List<PaymentWiseAmount> PaymentWiseAmount { get; set; }
        public List<RecentPayment> RecentPayment { get; set; }
    }
    public class DistributorRecentPaymentStatus
    {
        public List<RecentOrder> RecentOrder { get; set; }
        public List<RecentPayment> RecentPayment { get; set; }
        public List<ProductViewModel> ProductViewModelOrder { get; set; }
        public List<ProductViewModel> ProductViewModelQuantity { get; set; }
    }
    public class AdminRecentPaymentStatus
    {
        public List<DistributorViewModel> DistributorViewModelOrder { get; set; }
        public List<DistributorViewModel> DistributorViewModelPayment { get; set; }
        public List<ProductViewModel> ProductViewModelOrder { get; set; }
        public List<ProductViewModel> ProductViewModelQuantity { get; set; }
    }
}
