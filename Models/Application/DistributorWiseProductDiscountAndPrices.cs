using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Models.Common;

namespace Models.Application
{
    public class DistributorWiseProductDiscountAndPrices : CreatedEntity
    {
        public int DistributorId { get; set; }
        [ForeignKey("DistributorId")]
        public virtual Distributor Distributor { get; set; }

        public int? ProductDetailId { get; set; }
        [ForeignKey("ProductDetailId")]
        public virtual ProductDetail ProductDetail { get; set; }
        public string SAPProductCode { get; set; }
        public string PackSize { get; set; }
        public string ProductDescription { get; set; }
        public double ProductPrice { get; set; }
        public double CartonSize { get; set; }
        public double Rate { get; set; }
        public double Discount { get; set; }
        public double ReturnMRPDicount { get; set; }
        [NotMapped]
        public int PendingQuantity { get; set; }
        [NotMapped]
        public double SalesTax { get; set; }
        [NotMapped]
        public double IncomeTax { get; set; }
        [NotMapped]
        public double ViewSalesTax { get; set; }
        [NotMapped]
        public double AdditionalSalesTax { get; set; }
        [NotMapped]
        public string ProductCode { get; set; }
        [NotMapped]
        public string Quantity { get; set; }
    }
}
