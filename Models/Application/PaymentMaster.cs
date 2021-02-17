using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Models.Common;
using Models.ViewModel;
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
        [Required(ErrorMessage = "Company is required.")]
        public int CompanyId { get; set; }
        [ForeignKey("CompanyId")]
        public virtual Company Company { get; set; }
        public DateTime? DepositDate { get; set; }
        public DateTime? ValueClearingDate { get; set; }
        [Required(ErrorMessage = "Depositor Bank Name is required.")]
        public int DepositorBankName { get; set; }
        [Required(ErrorMessage = "Depositor Bank Code is required.")]
        public string DepositorBankCode { get; set; }
        [Required(ErrorMessage = "Company Bank Name is required.")]
        public int CompanyBankName { get; set; }
        [Required(ErrorMessage = "Company Bank Code is required.")]
        public string CompanyBankCode { get; set; }
        [Range(1, 9999999999)]
        [Column(TypeName = "double")]
        [Required(ErrorMessage = "Amount is required.")]
        public double Amount { get; set; }
        [Required(ErrorMessage = "Payment Mode is required.")]
        public int PaymentModeId { get; set; }
        [ForeignKey("PaymentModeId")]
        public virtual PaymentMode PaymentMode { get; set; }
        [Required(ErrorMessage = "Payment Mode No is required.")]
        public string PaymentModeNo { get; set; }
        [StringLength(255)]
        public string Remarks { get; set; }
        public PaymentStatus Status { get; set; }
        public string SAPCompanyCode { get; set; }
        public string SAPFiscalYear { get; set; }
        public string SAPDocumentNumber { get; set; }
        public int? RejectedBy { get; set; }
        public DateTime? RejectedDate { get; set; }
        public int? ApprovedBy { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public string File { get; set; }
        [NotMapped]
        public IFormFile FormFile { get; set; }
        [NotMapped]
        public SelectList PaymentModeList { get; set; }
        [NotMapped]
        public SelectList CompanyList { get; set; }
        [NotMapped]
        public SelectList DepostitorBankList { get; set; }
        [NotMapped]
        public SelectList CompanyBankList { get; set; }
        [NotMapped]
        public string CreatedName { get; set; }
        [NotMapped]
        public string ApprovedName { get; set; }
        [NotMapped]
        public string RejectedName { get; set; }
        [NotMapped]
        public PaymentValueViewModel PaymentValueViewModel { get; set; }
    }
}
