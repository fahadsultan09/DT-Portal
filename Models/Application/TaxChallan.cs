using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Models.Common;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Utility;

namespace Models.Application
{
    public class TaxChallan : UpdatedEntity
	{
		public int DistributorId { get; set; }
		[ForeignKey("DistributorId")]
		public virtual Distributor Distributor { get; set; }
		public TaxChallanStatus Status { get; set; }
		public int SNo { get; set; }
		[Required(ErrorMessage = "CPR No is required.")]
		public string CPRNo { get; set; }
		[Required(ErrorMessage = "CPR Date is required.")]
		public DateTime CPRDate { get; set; }
		[Required(ErrorMessage = "Tax Period is required.")]
		public DateTime TaxPeriod { get; set; }
		[Required(ErrorMessage = "Select Payment Section.")]
		[BindRequired] 
		public int PaymentSection { get; set; }
		[Range(1, 999999999)]
		[Column(TypeName = "double")]
		[Required(ErrorMessage = "Amount is required.")]
		public decimal AmountOnTaxWitheld { get; set; }
		[Range(1, 999999999)]
		[Column(TypeName = "double")]
		[Required(ErrorMessage = "Tax Amount is required.")]
		public decimal TaxAmount { get; set; }
		[StringLength(255)]
		[Required(ErrorMessage = "Remarks is required.")]
		public string Remarks { get; set; }
		[StringLength(255)]
		public string RejectedRemarks { get; set; }
		public int? RejectedBy { get; set; }
		public DateTime? RejectedDate { get; set; }
		public int? ApprovedBy { get; set; }
		public DateTime? ApprovedDate { get; set; }
		[StringLength(255)]
		[Required(ErrorMessage = "Attachment is required")]
		public string Attachment { get; set; }
		[NotMapped]
		public IFormFile FormFile { get; set; }
		[NotMapped]
		public string CreatedName { get; set; }
		[NotMapped]
		public string ApprovedName { get; set; }
		[NotMapped]
		public string RejectedName { get; set; }
		[NotMapped]
		public SelectList PaymentSectionList { get; set; }
	}
}
