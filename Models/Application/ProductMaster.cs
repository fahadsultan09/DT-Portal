using Models.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Models.Application
{
    public class ProductMaster : DeletedEntity
    {
        public int SAPProductCode { get; set; }
        public string PackCode { get; set; }
        public string ProductName { get; set; }
        public string ProductDescription { get; set; }
        public int ProductPrice { get; set; }
        public string ProductOrigin { get; set; }
        public string PackSize { get; set; }
        public string Strength { get; set; }
        public int CartonSize { get; set; }
        public int SFSize { get; set; }
        public int TradePrice { get; set; }
        public int Rate { get; set; }
        public int Discount { get; set; }
        [NotMapped]
        public ProductDetail ProductDetail { get; set; }
        [NotMapped]
        public bool IsRowDeleted { get; set; }
    }
}
