using Microsoft.AspNetCore.Mvc.Rendering;

namespace Models.ViewModel
{
    public class InvoiceSearch : Search
    {
        public int? InvoiceNo { get; set; }
        public int? CompanyId { get; set; }
        public SelectList CompanyList { get; set; }
    }
}
