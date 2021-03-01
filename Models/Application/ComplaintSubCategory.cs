using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Models.Common;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Utility;

namespace Models.Application
{
    public class ComplaintSubCategory : UpdatedEntity
    {
        [DisplayName("Complaint Category")]
        [Required(ErrorMessage = "Select Complaint Category.")]
        [BindRequired]
        public int ComplaintCategoryId { get; set; }
        [ForeignKey("ComplaintCategoryId")]
        public virtual ComplaintCategory ComplaintCategory { get; set; }
        [Required(ErrorMessage = "Enter Complaint Sub Category name.")]
        [StringLength(255)]
        public string ComplaintSubCategoryName { get; set; }
        [BindRequired]
        [Required(ErrorMessage = "Select Email To.")]
        public int UserEmailTo { get; set; }
        [ForeignKey("UserEmailTo")]
        public virtual User User { get; set; }
        public int? KPIDay { get; set; }
        public virtual List<ComplaintUserEmail> ComplaintUserEmail { get; set; }
        [NotMapped]
        public EmailType EmailType { get; set; }
        [NotMapped]
        public string[] UserEmailKPI { get; set; }
        [NotMapped]
        public string[] UserEmailCC { get; set; }
        [NotMapped]
        public SelectList ComplaintCategoryList { get; set; }
        [NotMapped]
        public SelectList UserList { get; set; }
        //[NotMapped]
        //public MultiSelectList UserEmailKPIList { get; set; }
        //[NotMapped]
        //public MultiSelectList UserEmailCCList { get; set; }
    }
}
