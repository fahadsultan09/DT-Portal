using Models.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Models.Application
{
    public class PaymentMaster : UpdatedEntity
    {
        public int DistributorId { get; set; }
        [ForeignKey("DistributorId")]
        public virtual Distributor Distributor { get; set; }
        public DateTime DepositDate { get; set; }
        public DateTime ValueClearingDate { get; set; }
        public int PaymentMode { get; set; }
        public string DepositorBankName { get; set; }
    }
}
