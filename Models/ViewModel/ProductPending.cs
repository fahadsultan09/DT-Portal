using Fingers10.ExcelExport.Attributes;
using Utility;

namespace Models.ViewModel
{
    public class ProductPending
    {
        [IncludeInReport(Order = 1)]
        public string SAPProductCode { get; set; }
        [IncludeInReport(Order = 2)]
        public string ProductName { get; set; }
        [IncludeInReport(Order = 3)]
        public string PackSize { get; set; }
        [IncludeInReport(Order = 4)]
        public string Comapny { get; set; }
        [IncludeInReport(Order = 5)]
        public int PendingQuantity { get; set; }
        [IncludeInReport(Order = 6)]
        public double Rate { get; set; }
        
        [IncludeInReport(Order = 7)]
        public double IncomeTax { get; set; }
        [IncludeInReport(Order = 8)]
        public double SalesTax { get; set; }
        [IncludeInReport(Order = 9)]
        public double AdditionalSalesTax { get; set; }
        [IncludeInReport(Order = 10)]
        public double PendingValue { get; set; }
        [IncludeInReport(Order = 11)]
        public OrderStatus Status { get; set; }
        public int CompanyId { get; set; }
        public int DistributorId { get; set; }
        public int productId { get; set; }
        public double Discount { get; set; }
    }
}
