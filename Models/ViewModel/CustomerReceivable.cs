using Models.Application;
using System.Collections.Generic;
using Utility;

namespace Models.ViewModel
{
    public class CustomerReceivable
    {
        public int DistributorId { get; set; }
        public List<Distributor> Distributor { get; set; }
        public int CompanyId { get; set; }
        public string CompanyName { get; set; }
        public double ApproveOrdes { get; set; }
        public double UnApproveOrdes { get; set; }
        public double CurrentBalance { get; set; }
        public DebitCreditIndicator DebitCreditIndicator { get; set; }
        public double UnapprovedPayments { get; set; }
        public double NetValue { get; set; }
        public ReceivableAdvanceIndicator ReceivableAdvanceIndicator { get; set; }
    }
}
