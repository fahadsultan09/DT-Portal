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
        public DateTime DepositDate { get; set; }
        public DateTime ValueClearingDate { get; set; }
        public int DepositorBankName { get; set; }
        public string DepositorBankCode { get; set; }
        public int CompanyBankName { get; set; }
        public string CompanyBankCode { get; set; }
        public decimal Amount { get; set; }
        public int PaymentModeId { get; set; }
        [ForeignKey("PaymentModeId")]
        public virtual PaymentMode PaymentMode { get; set; }
        public string ChequeNo { get; set; }
        public string OnlineTransactionNo { get; set; }
        public string RTGSNo { get; set; }
        public string PoNo { get; set; }
        [Required(ErrorMessage = "Attachment is required.")]
        public string File { get; set; }
        [NotMapped]
        public IFormFile FormFile { get; set; }
        [NotMapped]
        public SelectList PaymentModeList { get; set; }
        [NotMapped]
        public SelectList BankList { get; set; }
    }
}
