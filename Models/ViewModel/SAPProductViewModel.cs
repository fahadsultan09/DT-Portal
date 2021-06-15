using System;
using System.Collections.Generic;
using System.Text;

namespace Models.ViewModel
{
    public class SAPProductViewModel
    {
        public string SAPProductCode { get; set; }
        public string PackSize { get; set; }
        public string ProductName { get; set; }
        public string ProductDescription { get; set; }
        public double ProductPrice { get; set; }
        public string CartonSize { get; set; }
        public double Rate { get; set; }
        public double Discount { get; set; }
        public string LicenseType { get; set; }
        public string SFSize { get; set; }
        public string Strength { get; set; }
        public string ProductOrigin { get; set; }
    }
}
