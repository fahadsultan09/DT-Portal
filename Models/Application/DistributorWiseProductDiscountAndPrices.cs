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
        public string ProductName { get; set; }
        public string ProductDescription { get; set; }
        public double ProductPrice { get; set; }
        public double CartonSize { get; set; }
        public double Discount { get; set; }
    }
}
