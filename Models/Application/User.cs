using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Models.Common;
using Models.UserRights;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models.Application
{
    public class User : DeletedEntity
    {
        [StringLength(1000)]
        public string AccessToken { get; set; }
        [DisplayName("First Name")]
        [Required(ErrorMessage = "First Name field is required.")]
        [StringLength(100)]
        public string FirstName { get; set; }
        [DisplayName("Last Name")]
        [Required(ErrorMessage = "Last Name field is required.")]
        [StringLength(100)]
        public string LastName { get; set; }
        [DisplayName("Username")]
        [Required(ErrorMessage = "User name field is required.")]
        [MaxLength(8, ErrorMessage = "User name max length must be 8 characters")]
        [MinLength(6, ErrorMessage = "User name min length must be 6 characters")]
        [RegularExpression(@"^[0-9-]*$", ErrorMessage = "Only number is allowed")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "Password field is required.")]
        [StringLength(100)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,15}$", ErrorMessage = "Invalid password")]
        public string Password { get; set; }
        [StringLength(50)]
        [Required(ErrorMessage = "Email field is required.")]
        [RegularExpression(@"^([a-zA-Z0-9_\.\-])+\@(([a-zA-Z0-9\-])+\.)+([a-zA-Z0-9]{2,4})+$", ErrorMessage = "Invalid email address")]
        public string Email { get; set; }
        public string RegisteredAddress { get; set; }
        [Required(ErrorMessage = "Mobile Number field is required.")]
        public string MobileNumber { get; set; }
        public bool IsDistributor { get; set; }
        public bool IsParentDistributor { get; set; }
        public bool IsStoreKeeper { get; set; }
        public int? DistributorId { get; set; }
        [ForeignKey("DistributorId")]
        public virtual Distributor Distributor { get; set; }
        public int? PlantLocationId { get; set; }
        [ForeignKey("PlantLocationId")]
        public virtual PlantLocation PlantLocation { get; set; }
        public int? CompanyId { get; set; }
        [ForeignKey("CompanyId")]
        public virtual Company Company { get; set; }
        public int? EmailIntimationId { get; set; }
        [BindRequired]
        [Required(ErrorMessage = "Select designation.")]
        public int DesignationId { get; set; }
        [ForeignKey("DesignationId")]
        public virtual Designation Designation { get; set; }
        [NotMapped]
        [DisplayName("Confirm Password")]
        [Required(ErrorMessage = "Confirm Password field is required.")]
        [Compare("Password", ErrorMessage = "Password doesn't match.")]
        public string ConfirmPassword { get; set; }
        [BindRequired]
        [Required(ErrorMessage = "Select city.")]
        public int? CityId { get; set; }
        [ForeignKey("CityId")]
        public virtual City City { get; set; }
        [BindRequired]
        [DisplayName("Role")]
        [Required(ErrorMessage = "Select role.")]
        public int RoleId { get; set; }
        [ForeignKey("RoleId")]
        public virtual Role Role { get; set; }
        [NotMapped]
        public SelectList RoleList { get; set; }
        [NotMapped]
        public SelectList DistributorList { get; set; }
        [NotMapped]
        public SelectList DesignationList { get; set; }
        [NotMapped]
        public SelectList CityList { get; set; }
        [NotMapped]
        public SelectList CompanyList { get; set; }
        [NotMapped]
        public SelectList PlantLocationList { get; set; }
        [NotMapped]
        public string MacAddresses { get; set; }
    }
}
