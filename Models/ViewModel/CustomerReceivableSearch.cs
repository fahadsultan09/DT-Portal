using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using Utility;

namespace Models.ViewModel
{
    public class CustomerReceivableSearch:Search
    {
        public int? CompanyId { get; set; }
        public SelectList CompanyList { get; set; }
        public List<CustomerReceivable> CustomerReceivable { get; set; }
        public DebitCreditIndicator? DebitCreditIndicator { get; set; }
        public ReceivableAdvanceIndicator? ReceivableAdvanceIndicator { get; set; }
    }
}
