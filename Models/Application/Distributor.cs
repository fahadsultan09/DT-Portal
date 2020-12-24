using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Models.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Utility;

namespace Models.Application
{
    public class Distributor : DeletedEntity
    {
        [BindRequired]
        [Required(ErrorMessage = "The City field is required.")]
        public string City { get; set; }
        [Required(ErrorMessage = "The Region field is required.")]
        [BindRequired]
        public int RegionId { get; set; }
        [ForeignKey("RegionId")]
        public virtual Region Region { get; set; }
        public int DistributorSAPCode { get; set; }
        [Required(ErrorMessage = "The Distributor Code field is required.")]
        public string DistributorCode { get; set; }
        [Required(ErrorMessage = "The Distributor Name field is required.")]
        public string DistributorName { get; set; }
        [Required(ErrorMessage = "The Registered Address field is required.")]
        public string DistributorAddress { get; set; }
        [Required(ErrorMessage = "The First Name field is required.")]
        public string NTN { get; set; }
        [Required(ErrorMessage = "The CNIC field is required.")]
        public string CNIC { get; set; }
        [StringLength(50)]
        [Required(ErrorMessage = "The Email field is required.")]
        [RegularExpression(@"^([a-zA-Z0-9_\.\-])+\@(([a-zA-Z0-9\-])+\.)+([a-zA-Z0-9]{2,4})+$", ErrorMessage = "Invalid email address")]
        public string EmailAddress { get; set; }
        [Required(ErrorMessage = "The Mobile Number field is required.")]
        public string MobileNumber { get; set; }
        [Required(ErrorMessage = "The Customer Grp field is required.")]
        public string CustomerGroup { get; set; }
        public DistributorStatus Status { get; set; }
        [NotMapped]
        public SelectList RegionList { get; set; }
        [NotMapped]
        public string RegionCode { get; set; }
    }
}
