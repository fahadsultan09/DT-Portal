using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Models.Common;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
        [Required(ErrorMessage = "Enter your Complaint Category name.")]
        [StringLength(255)]
        public string ComplaintSubCategoryName { get; set; }
        [NotMapped]
        public SelectList ComplaintCategoryList { get; set; }
    }
}
