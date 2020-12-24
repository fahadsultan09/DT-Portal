using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Models.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
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
        [Required(ErrorMessage = "The City is required.")]
        public string City { get; set; }
        [Required(ErrorMessage = "The Distributor Code is required.")]
        public int DistributorSAPCode { get; set; }
        [Required(ErrorMessage = "The Distributor Code is required.")]
        public string DistributorCode { get; set; }
        [Required(ErrorMessage = "The Distributor Name is required.")]
        public string DistributorName { get; set; }
        [Required(ErrorMessage = "The Registered Address is required.")]
        public string DistributorAddress { get; set; }
        [Required(ErrorMessage = "The NTN is required.")]
        public string NTN { get; set; }
        [Required(ErrorMessage = "The CNIC is required.")]
        public string CNIC { get; set; }
        [StringLength(50)]
        [Required(ErrorMessage = "The Email is required.")]
        [RegularExpression(@"^([a-zA-Z0-9_\.\-])+\@(([a-zA-Z0-9\-])+\.)+([a-zA-Z0-9]{2,4})+$", ErrorMessage = "Invalid email address")]
        public string EmailAddress { get; set; }
        [Required(ErrorMessage = "The Mobile Number is required.")]
        public string MobileNumber { get; set; }
        [Required(ErrorMessage = "The Customer Grp is required.")]
        public string CustomerGroup { get; set; }
        public DistributorStatus Status { get; set; }
        [NotMapped]
        public SelectList RegionList { get; set; }
        [NotMapped]
        public SelectList SubRegionList { get; set; }
        [NotMapped]
        public SelectList CityList { get; set; }
    }
}
