using Models.Common;
using System.ComponentModel.DataAnnotations;

namespace Models.Application
{
    public class ComplaintCategory : UpdatedEntity
    {
        [Required(ErrorMessage = "Enter your Complaint Category name.")]
        [StringLength(255)]
        public string ComplaintCategoryName { get; set; }
    }
}
