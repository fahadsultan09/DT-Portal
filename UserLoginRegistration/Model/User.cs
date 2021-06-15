using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace UserLoginRegistration.Model
{
    public class User 
    {
        [Required(ErrorMessage = "User name field is required.")]
        [StringLength(8)]
        [MaxLength(8, ErrorMessage = "User name max length must be 8 characters")]
        [RegularExpression(@"^[0-9-]*$", ErrorMessage = "Only number is allowed")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "Password field is required.")]
        [StringLength(100)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,15}$", ErrorMessage = "Invalid password")]
        public string Password { get; set; }
        public bool IsDistributor { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string MacAddresses { get; set; }
    }
}
