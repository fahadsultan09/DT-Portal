using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Models.Common;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Utility;

namespace Models.Application
{
    public class Complaint : UpdatedEntity
    {
        public int DistributorId { get; set; }
        [ForeignKey("DistributorId")]
        public virtual Distributor Distributor { get; set; }
        [DisplayName("Complaint Category")]
        [Required(ErrorMessage = "Select Complaint Category.")]
        [BindRequired]
        public int ComplaintCategoryId { get; set; }
        [ForeignKey("ComplaintCategoryId")]
        public virtual ComplaintCategory ComplaintCategory { get; set; }

        [DisplayName("Complaint Sub Category")]
        [Required(ErrorMessage = "Select Complaint Sub Category.")]
        [BindRequired]
        public int ComplaintSubCategoryId { get; set; }
        [ForeignKey("ComplaintSubCategoryId")]
        public virtual ComplaintSubCategory ComplaintSubCategory { get; set; }
        [Required(ErrorMessage = "Enter Description.")]
        [StringLength(255)]
        public string Description { get; set; }
        [StringLength(255)]
        public string Remarks { get; set; }
        public ComplaintStatus Status { get; set; }
        [StringLength(255)]
        public string File { get; set; }
        [NotMapped]
        public IFormFile FormFile { get; set; }
        [NotMapped]
        public SelectList ComplaintCategoryList { get; set; }
        [NotMapped]
        public SelectList ComplaintSubCategoryList { get; set; }
    }
}
