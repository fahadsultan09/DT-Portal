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
        public string SAPProductCode { get; set; }
        public string PackCode { get; set; }
        public string ProductName { get; set; }
        public string ProductDescription { get; set; }
        public double ProductPrice { get; set; }
        public string ProductOrigin { get; set; }
        public string PackSize { get; set; }
        public string Strength { get; set; }
        public double CartonSize { get; set; }
        public double SFSize { get; set; }
        public double TradePrice { get; set; }
        public double Rate { get; set; }
        public double Discount { get; set; }
        [NotMapped]
        public ProductDetail ProductDetail { get; set; }
        [NotMapped]
        public bool IsRowDeleted { get; set; }
        [NotMapped]
        public int Quantity { get; set; }
        [NotMapped]
        public SelectList ProductList { get; set; }
    }
}
