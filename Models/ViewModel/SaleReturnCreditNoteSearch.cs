using Microsoft.AspNetCore.Mvc.Rendering;

namespace Models.ViewModel
{
    public class SaleReturnCreditNoteSearch : Search
    {
        public int? SaleReturnNo { get; set; }
        public int? CompanyId { get; set; }
        public SelectList CompanyList { get; set; }
    }
}
