using Utility;

namespace Models.ViewModel
{
    public class OrderWiseStatus
    {
        public string Month { get; set; }
        public int LastYear { get; set; }
        public int CurrentYear { get; set; }
    }
    public class PaymentWiseStatus
    {
        public string Month { get; set; }
        public decimal VerifiedPayment { get; set; }
        public decimal UnverifiedPayment { get; set; }
    }
    public class RegionWiseOrder
    {
        public string Region { get; set; }
        public int OrderCount { get; set; }
    }
    public class PaymentWiseAmount
    {
        public string PaymentMode { get; set; }
        public decimal Amount { get; set; }
    }
    public class DistributorViewModel
    {
        public string DistributorName { get; set; }
        public int OrderCount { get; set; }
        public decimal Payment { get; set; }
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
        public decimal Amount { get; set; }
        public PaymentStatus Status { get; set; }
    }
}
