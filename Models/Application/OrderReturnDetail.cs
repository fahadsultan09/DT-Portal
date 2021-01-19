using Models.Common;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models.Application
{
    public class OrderReturnDetail : CreatedEntity
    {
        public int CompanyId { get; set; }
        [ForeignKey("CompanyId")]
        public virtual Company Company { get; set; }
        public int OrderReturnId { get; set; }
        [ForeignKey("OrderReturnId")]
        public virtual OrderReturnMaster OrderReturnMaster { get; set; }
        public int ProductId { get; set; }
        [ForeignKey("ProductId")]
        public virtual ProductMaster ProductMaster { get; set; }
        public int Quantity { get; set; }
        public double MRP { get; set; }
        public string BatchNo { get; set; }
        public string InvoiceNo { get; set; }
        public string InvoiceDate { get; set; }
        public DateTime ExpiryDate { get; set; }
        public DateTime ManufactureDate { get; set; }
        public DateTime IntimationDate { get; set; }
        public string Remarks { get; set; }
        [NotMapped]
        public string TP { get; set; }
        [NotMapped]
        public Double Discount { get; set; }
        [NotMapped]
        public double NetAmount { get; set; }
    }
}
