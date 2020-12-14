﻿using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Models.Common;
using Models.UserRights;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models.Application
{
    public class User : DeletedEntity
    {
        [DisplayName("First Name")]
        [Required(ErrorMessage = "The First Name field is required.")]
        [StringLength(100)]
        public string FirstName { get; set; }

        [DisplayName("Last Name")]
        [Required(ErrorMessage = "The Last Name field is required.")]
        [StringLength(100)]
        public string LastName { get; set; }

        [DisplayName("Username")]
        [Required(ErrorMessage = "The User name field is required.")]
        [StringLength(6)]
        [MaxLength(6, ErrorMessage = "User name max length must be 6 characters")]
        [MinLength(6, ErrorMessage = "User name min length must be 6 characters")]
        [RegularExpression(@"^[0-9-]*$", ErrorMessage = "Only number is allowed")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "The Password field is required.")]
        [StringLength(100)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,15}$", ErrorMessage = "Invalid password")]
        public string Password { get; set; }

        [StringLength(50)]
        [Required(ErrorMessage = "The Email field is required.")]
        [RegularExpression(@"^([a-zA-Z0-9_\.\-])+\@(([a-zA-Z0-9\-])+\.)+([a-zA-Z0-9]{2,4})+$", ErrorMessage = "Invalid email address")]
        public string Email { get; set; }

        public bool IsDistributor { get; set; }
        public int? DistributorId { get; set; }

        [NotMapped]
        [DisplayName("Confirm Password")]
        [Required(ErrorMessage = "The Confirm Password field is required.")]
        [Compare("Password", ErrorMessage = "Password doesn't match.")]
        public string ConfirmPassword { get; set; }

        [BindRequired]
        [DisplayName("Role")]
        [Required(ErrorMessage = "Please select role.")]
        public int RoleId { get; set; }
        [ForeignKey("RoleId")]
        public virtual Role Role { get; set; }
        [NotMapped]
        public SelectList RoleList { get; set; }

    }
}
