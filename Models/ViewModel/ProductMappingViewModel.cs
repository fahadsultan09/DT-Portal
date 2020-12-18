using Models.Application;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Models.ViewModel
{
    public class ProductMappingViewModel
    {
        public ProductMaster ProductMaster { get; set; }
        public List<ProductDetail> ProductDetail { get; set; }
        //public string PackCode { get; set; }
        //public string ProductName { get; set; }
        //public string PackSize { get; set; }
        //public int ProductMasterId { get; set; }
        //[ForeignKey("ProductMasterId")]
        //public virtual ProductMaster ProductMaster { get; set; }
        //public int Visibility { get; set; }
        //public string PlantLocation { get; set; }
        //public Company Company { get; set; }
        //public string WTaxRate { get; set; }
        //public int Factor { get; set; }
        //public string ParentDistributor { get; set; }
        //public string S_OrderType { get; set; }
        //public string R_OrderType { get; set; }
        //public string SaleOrganization { get; set; }
        //public string DistributionChannel { get; set; }
        //public string Division { get; set; }
        //public string DispatchPlant { get; set; }
        //public string S_StorageLocation { get; set; }
        //public string R_StorageLocation { get; set; }
        //public string SalesItemCategory { get; set; }
        //public string ReturnItemCategory { get; set; }
    }
}
