using Microsoft.AspNetCore.Mvc.Rendering;
using Models.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models.Application
{
    public class PaymentSection : DeletedEntity
	{
		public int CompanyId { get; set; }
		[ForeignKey("CompanyId")]
		public virtual Company Company { get; set; }
		[StringLength(50)]
		[Required(ErrorMessage = "Enter your Form No")]
		public string FormNo { get; set; }
		[Range(1, 999999999)]
		[Column(TypeName = "int")]
		public int GLAccount { get; set; }
		[Range(1, 999999999)]
		[Column(TypeName = "double")]
		public double TaxRate { get; set; }
		[NotMapped]
		public SelectList CompanyList { get; set; }
	}
}
