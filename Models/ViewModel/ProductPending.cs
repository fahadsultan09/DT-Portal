using Fingers10.ExcelExport.Attributes;
using Models.Application;
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
        public double PendingQuantity { get; set; }
        [IncludeInReport(Order = 6)]
        public double Rate { get; set; }
        
        [IncludeInReport(Order = 7)]
        public double AdvanceTax { get; set; }
        [IncludeInReport(Order = 8)]
        public double SalesTax { get; set; }
        [IncludeInReport(Order = 9)]
        public double AdSalesTax { get; set; }
        [IncludeInReport(Order = 10)]
        public double PendingValus { get; set; }
        [IncludeInReport(Order = 11)]
        public OrderStatus Status { get; set; }
        public ProductMaster ProductMaster { get; set; }
        public int CompanyId { get; set; }
        public int DistributorId { get; set; }
        public int productId { get; set; }
    }
}
