﻿using Models.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Utility;

namespace Models.Application
{
    public class ProductDetail : DeletedEntity
    {
        public int ProductMasterId { get; set; }
        [ForeignKey("ProductMasterId")]
        public virtual ProductMaster ProductMaster { get; set; }
        public int CompanyId { get; set; }
        [ForeignKey("CompanyId")]
        public virtual Company Company { get; set; }
        public int? LicenseControlId { get; set; }
        [ForeignKey("LicenseControlId")]
        public virtual LicenseControl LicenseControl { get; set; }
        public ProductVisibility ProductVisibilityId { get; set; }
        public int PlantLocationId { get; set; }
        [ForeignKey("PlantLocationId")]
        public virtual PlantLocation PlantLocation { get; set; }
        public string WTaxRate { get; set; }
        public int Factor { get; set; }
        public string ParentDistributor { get; set; }
        public string S_OrderType { get; set; }
        public string R_OrderType { get; set; }
        public string SaleOrganization { get; set; }
        public string DistributionChannel { get; set; }
        public string Division { get; set; }
        public string DispatchPlant { get; set; }
        public string S_StorageLocation { get; set; }
        public string R_StorageLocation { get; set; }
        public string SalesItemCategory { get; set; }
        public string ReturnItemCategory { get; set; }
        [NotMapped]
        public double TotalPrice { get; set; }
        public bool IsTaxApplicable { get; set; }
        [NotMapped]
        public int OrderNumber { get; set; }
        [NotMapped]
        public bool IsProductSelected { get; set; }
        [NotMapped]
        public string PendingQuantity { get; set; }
        [NotMapped]
        public double Discount { get; set; }
    }
}
