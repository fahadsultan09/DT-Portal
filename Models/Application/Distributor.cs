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
        [Required(ErrorMessage = "The Region is required.")]
        [BindRequired]
        public int RegionId { get; set; }
        [ForeignKey("RegionId")]
        public virtual Region Region { get; set; }
        public string City { get; set; }
        public string DistributorSAPCode { get; set; }
        public string DistributorCode { get; set; }
        public string DistributorName { get; set; }
        public string DistributorAddress { get; set; }
        public string NTN { get; set; }
        public string CNIC { get; set; }
        public string EmailAddress { get; set; }
        public string MobileNumber { get; set; }
        public string CustomerGroup { get; set; }
        public bool IsSalesTaxApplicable { get; set; }
        public bool IsIncomeTaxApplicable { get; set; }
        public DistributorStatus Status { get; set; }
        [NotMapped]
        public SelectList RegionList { get; set; }
        [NotMapped]
        public string RegionCode { get; set; }
    }
}
