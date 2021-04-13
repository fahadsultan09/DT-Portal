using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Models.ViewModel
{
    public class DistributorWiseProductViewModel
    {
        public int DistributorId { get; set; }
        public string SAPProductCode { get; set; }
        public string PackSize { get; set; }
        public string ProductName { get; set; }
        public string ProductDescription { get; set; }
        public double ProductPrice { get; set; }
        public double CartonSize { get; set; }
        public double Rate { get; set; }
        public double Discount { get; set; }
        public string LicenseType { get; set; }
        public string SFSize { get; set; }
        public int? ProductDetailId { get; set; }
    }
}