using Models.Common;
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
        public ProductVisibility ProductVisibilityId { get; set; }
        public string PlantLocation { get; set; }
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
        public bool IsTaxApplicable { get; set; }

    }
}
