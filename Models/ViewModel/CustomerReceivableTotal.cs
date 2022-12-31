using System;
using System.Collections.Generic;
using System.Text;

namespace Models.ViewModel
{
    public class CustomerReceivableTotal
    {
        public IEnumerable<CustomerReceivable> CustomerReceivable { get; set; }
        public IEnumerable<CustomerReceivable> CustomersTotal { get; set; }
        public IEnumerable<CustomerReceivable> CompanyTotal { get; set; }
    }
}
