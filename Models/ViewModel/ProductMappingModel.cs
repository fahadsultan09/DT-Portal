using Fingers10.ExcelExport.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Models.ViewModel
{
    public class ProductMappingModel
    {
        [IncludeInReport(Order = 1)]
        public string ProductCode { get; set; }
        [IncludeInReport(Order = 2)]
        public string ProductName { get; set; }
        [IncludeInReport(Order = 3)]
        public string PackSize { get; set; }
        [IncludeInReport(Order = 4)]
        public string Visibility { get; set; }
        [IncludeInReport(Order = 5)]
        public string PlantLocation { get; set; }
        [IncludeInReport(Order = 6)]
        public string Company { get; set; }
        [IncludeInReport(Order = 7)]
        public string WTaxRate { get; set; }
        [IncludeInReport(Order = 8)]
        public int Factor { get; set; }
        [IncludeInReport(Order = 9)]
        public string ParentDistributor { get; set; }
        [IncludeInReport(Order = 10)]
        public string S_OrderType { get; set; }
        [IncludeInReport(Order = 11)]
        public string R_OrderType { get; set; }
        [IncludeInReport(Order = 12)]
        public string SaleOrganization { get; set; }
        [IncludeInReport(Order = 3)]
        public string DistributionChannel { get; set; }
        [IncludeInReport(Order = 4)]
        public string Division { get; set; }
        [IncludeInReport(Order = 15)]
        public string DispatchPlant { get; set; }
        [IncludeInReport(Order = 16)]
        public string S_StorageLocation { get; set; }
        [IncludeInReport(Order = 17)]
        public string R_StorageLocation { get; set; }
        [IncludeInReport(Order = 18)]
        public string SalesItemCategory { get; set; }
        [IncludeInReport(Order = 19)]
        public string ReturnItemCategory { get; set; }
    }
}
