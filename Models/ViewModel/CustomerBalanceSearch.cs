using Microsoft.AspNetCore.Mvc.Rendering;
using System;

namespace Models.ViewModel
{
    public class CustomerBalanceSearch
    {
        public DateTime? Date { get; set; }
        public int? DistributorId { get; set; }
        public SelectList DistributorList { get; set; }
        public int? CompanyId { get; set; }
        public SelectList CompanyList { get; set; }
    }
}
