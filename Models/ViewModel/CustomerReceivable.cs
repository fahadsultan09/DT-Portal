using Models.Application;
using System.Collections.Generic;
using Utility;

namespace Models.ViewModel
{
    public class CustomerReceivable
    {
        public string DistributorSAPCode { get; set; }
        public string DistributorName { get; set; }
        public string City { get; set; }
        public string SAPCompanyCode { get; set; }
        public string CompanyName { get; set; }
        public decimal UnapprovedPayments { get; set; }
        public decimal ApprovedOrders { get; set; }
        public decimal UnapprovedOrders { get; set; }
        public decimal NetValue { get; set; }
        public decimal CurrentBalance { get; set; }
        public DebitCreditIndicator DebitCreditIndicator { get; set; }
        public ReceivableAdvanceIndicator ReceivableAdvanceIndicator { get; set; }
        public List<CustomerBalanceViewModel> CustomerBalance { get; set; }
        
    }

    
}
