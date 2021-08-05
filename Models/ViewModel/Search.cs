using Microsoft.AspNetCore.Mvc.Rendering;
using System;

namespace Models.ViewModel
{
    public class Search
    {
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public int? DistributorId { get; set; }
        public SelectList DistributorList { get; set; }
        public string DistributorSAPCode { get; set; }
        public SelectList DistributorSAPCodeList { get; set; }
    }
}
