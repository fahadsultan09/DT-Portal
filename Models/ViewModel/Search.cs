using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Text;

namespace Models.ViewModel
{
    public class Search
    {
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public int? DistributorId { get; set; }
        public SelectList DistributorList { get; set; }
    }
}
