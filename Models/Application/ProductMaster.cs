using Fingers10.ExcelExport.Attributes;
using Microsoft.AspNetCore.Mvc.Rendering;
using Models.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Models.Application
{
    public class ProductMaster : DeletedEntity
    {
        [IncludeInReport(Order = 1)]
        public string SAPProductCode { get; set; }
        [IncludeInReport(Order = 2)]
        public string PackCode { get; set; }
        [IncludeInReport(Order = 3)]
        public string ProductName { get; set; }
        [IncludeInReport(Order = 4)]
        public string ProductDescription { get; set; }
        [IncludeInReport(Order = 5)]
        public double ProductPrice { get; set; }
        [IncludeInReport(Order = 6)]
        public string ProductOrigin { get; set; }
        [IncludeInReport(Order = 7)]
        public string PackSize { get; set; }
        [IncludeInReport(Order = 8)]
        public string Strength { get; set; }
        [IncludeInReport(Order = 9)]
        public double CartonSize { get; set; }
        [IncludeInReport(Order = 10)]
        public double SFSize { get; set; }
        [IncludeInReport(Order = 11)]
        public double TradePrice { get; set; }
        [IncludeInReport(Order = 12)]
        public double Rate { get; set; }
        [IncludeInReport(Order = 13)]
        public double Discount { get; set; }
        [IncludeInReport(Order = 14)]
        public string LicenseType { get; set; }
        [IncludeInReport(Order = 15)]
        public double SalesTaxRate { get; set; }
        [NotMapped]
        public ProductDetail ProductDetail { get; set; }
        [NotMapped]
        public bool IsRowDeleted { get; set; }
        [NotMapped]
        public int Quantity { get; set; }
        [NotMapped]
        public int ApprovedQuantity { get; set; }
        [NotMapped]
        public double Amount { get; set; }
        [NotMapped]
        public SelectList ProductList { get; set; }
        [NotMapped]
        public int DistributorId { get; set; }
    }
}
