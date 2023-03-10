using Fingers10.ExcelExport.Attributes;

namespace Models.ViewModel
{
    public class ProductMappingModel
    {
        [IncludeInReport(Order = 1)]
        public string ProductCode { get; set; }

        [IncludeInReport(Order = 2)]
        public string BrandName { get; set; }
        [IncludeInReport(Order = 3)]
        public string ProductDescription { get; set; }
        [IncludeInReport(Order = 4)]
        public string PackCode { get; set; }
        [IncludeInReport(Order = 5)]
        public string CartonSize { get; set; }
        [IncludeInReport(Order = 6)]
        public string PackSize { get; set; }
        [IncludeInReport(Order = 7)]
        public string FOCProductCode { get; set; }
        [IncludeInReport(Order = 8)]
        public string Visibility { get; set; }
        [IncludeInReport(Order = 9)]
        public string PlantLocation { get; set; }
        [IncludeInReport(Order = 10)]
        public string Company { get; set; }
        [IncludeInReport(Order = 11)]
        public string WTaxRate { get; set; }
        [IncludeInReport(Order = 12)]
        public int Factor { get; set; }
        [IncludeInReport(Order = 13)]
        public string ParentDistributor { get; set; }
        [IncludeInReport(Order = 14)]
        public string S_OrderType { get; set; }
        [IncludeInReport(Order = 15)]
        public string R_OrderType { get; set; }
        [IncludeInReport(Order = 16)]
        public string SaleOrganization { get; set; }
        [IncludeInReport(Order = 17)]
        public string DistributionChannel { get; set; }
        [IncludeInReport(Order = 18)]
        public string Division { get; set; }
        [IncludeInReport(Order = 19)]
        public string DispatchPlant { get; set; }
        [IncludeInReport(Order = 20)]
        public string S_StorageLocation { get; set; }
        [IncludeInReport(Order = 21)]
        public string R_StorageLocation { get; set; }
        [IncludeInReport(Order = 22)]
        public string SalesItemCategory { get; set; }
        [IncludeInReport(Order = 23)]
        public string ReturnItemCategory { get; set; }
        [IncludeInReport(Order = 24)]
        public string IncomeTax { get; set; }
        [IncludeInReport(Order = 25)]
        public string SalesTax { get; set; }
        [IncludeInReport(Order = 26)]
        public string AdditionalSalesTax { get; set; }
        [IncludeInReport(Order = 27)]
        public string LicenseType { get; set; }
        [IncludeInReport(Order = 28)]
        public string FOCQuantityRatio { get; set; }
    }
}
