using Microsoft.AspNetCore.Mvc.Rendering;
using System;

namespace Models.ViewModel
{
    public class CustomerBalanceSearch : Search
    {
        public DateTime? Date { get; set; }
        public string SAPCompanyCode { get; set; }
        public SelectList CompanyList { get; set; }
    }
}
