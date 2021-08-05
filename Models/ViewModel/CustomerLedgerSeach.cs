using Microsoft.AspNetCore.Mvc.Rendering;

namespace Models.ViewModel
{
    public class CustomerLedgerSeach : Search
    {
        public string SAPCompanyCode { get; set; }
        public SelectList CompanyList { get; set; }
    }
}
