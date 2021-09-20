using Models.Common;
using System.ComponentModel.DataAnnotations.Schema;
using Utility;

namespace Models.Application
{
    public class OrderDetail : CreatedEntity
    {
        public int OrderId { get; set; }
        [ForeignKey("OrderId")]
        public virtual OrderMaster OrderMaster { get; set; }
        public int ProductId { get; set; }
        [ForeignKey("ProductId")]
        public virtual ProductMaster ProductMaster { get; set; }
        public int Quantity { get; set; }
        public double Amount { get; set; }
        public string SAPOrderNumber { get; set; }
        public OrderStatus? OrderProductStatus { get; set; }
        public int ApprovedQuantity { get; set; }
        public bool IsProductSelected { get; set; }
        public double ProductPrice { get; set; }
        public double Discount { get; set; }
        public double QuanityCarton { get; set; }
        public double QuanitySF { get; set; }
        public double QuanityLoose { get; set; }
        public string ParentDistributor { get; set; }
        public string S_OrderType { get; set; }
        public string SaleOrganization { get; set; }
        public string DistributionChannel { get; set; }
        public string Division { get; set; }
        public string DispatchPlant { get; set; }
        public string S_StorageLocation { get; set; }
        public string SalesItemCategory { get; set; }
        public double? SalesTax { get; set; }
        public double? IncomeTax { get; set; }
        public double? AdditionalSalesTax { get; set; }
        [NotMapped]
        public double InclusiveSalesTax { get; set; }
        [NotMapped]
        public double CalculateIncomeTax { get; set; }
        [NotMapped]
        public bool IsRowDeleted { get; set; }
        [NotMapped]
        public ProductDetail ProductDetail { get; set; }
    }
}
