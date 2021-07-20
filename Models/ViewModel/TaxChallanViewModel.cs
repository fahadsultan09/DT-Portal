using Microsoft.AspNetCore.Mvc.Rendering;
using Models.Application;
using System;
using System.Collections.Generic;
using System.Text;
using Utility;

namespace Models.ViewModel
{
    public class TaxChallanViewModel
    {
        public List<TaxChallan> TaxChallan { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public TaxChallanStatus? Status { get; set; }
        public int? DistributorId { get; set; }
        public int? TaxChallanNo { get; set; }
        public SelectList DistributorList { get; set; }
    }
}
