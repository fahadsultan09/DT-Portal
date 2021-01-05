using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Models.Common;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Utility;

namespace Models.Application
{
    public class PaymentMaster : UpdatedEntity
    {
        public int DistributorId { get; set; }
        [ForeignKey("DistributorId")]
        public virtual Distributor Distributor { get; set; }
        public int CompanyId { get; set; }
        [ForeignKey("CompanyId")]
        public virtual Company Company { get; set; }
        public DateTime? DepositDate { get; set; }
        public DateTime? ValueClearingDate { get; set; }
        public int DepositorBankName { get; set; }
        public string DepositorBankCode { get; set; }
        public int CompanyBankName { get; set; }
        public string CompanyBankCode { get; set; }
        public double Amount { get; set; }
        public int PaymentModeId { get; set; }
        [ForeignKey("PaymentModeId")]
        public virtual PaymentMode PaymentMode { get; set; }
        public string PaymentModeNo { get; set; }
        public PaymentStatus Status { get; set; }
        public string File { get; set; }
        [NotMapped]
        public IFormFile FormFile { get; set; }
        [NotMapped]
        public SelectList PaymentModeList { get; set; }
        [NotMapped]
        public SelectList BankList { get; set; }
    }
}
