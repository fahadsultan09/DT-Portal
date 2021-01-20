using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Models.Common;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models.Application
{
    public class Bank : DeletedEntity
    {
        [DisplayName("Company")]
        [Required(ErrorMessage = "Select Company.")]
        [BindRequired]
        public int CompanyId { get; set; }
        [ForeignKey("CompanyId")]
        public virtual Company Company { get; set; }
        [Required(ErrorMessage = "Enter your Bank Name.")]
        [StringLength(255)]
        public string BankName { get; set; }
        [Required(ErrorMessage = "Enter your Branch Name.")]
        [StringLength(255)]
        public string Branch { get; set; }
        [Required(ErrorMessage = "Enter your Branch Code.")]
        [StringLength(10)]
        public string BranchCode { get; set; }
        [Required(ErrorMessage = "Enter your Account No.")]
        [StringLength(30)]
        public string AccountNo { get; set; }
        [Required(ErrorMessage = "Enter IBAN No name.")]
        [StringLength(30)]
        public string IBANNo { get; set; }
        [StringLength(10)]
        public string GLAccount { get; set; }
        [NotMapped]
        public SelectList CompanyList { get; set; }
    }
}
