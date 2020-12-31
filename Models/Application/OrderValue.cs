using Models.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Models.Application
{
    public class OrderValue : CreatedEntity
    {
        public int OrderId { get; set; }
        [ForeignKey("OrderId")]
        public virtual OrderMaster OrderMaster { get; set; }
        public int CompanyId { get; set; }
        [ForeignKey("CompanyId")]
        public virtual Company Company { get; set; }
        public double SuppliesZero { get; set; }
        public double SuppliesOne { get; set; }
        public double SuppliesFour { get; set; }
        public double TotalOrderValues { get; set; }
        public double PendingOrderValues { get; set; }
        public double CurrentBalance { get; set; }
        public double UnConfirmedPayment { get; set; }
        public double NetPayable { get; set; }
    }
}
