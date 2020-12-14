using Models.Common;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Models.UserRights
{
    public class Role : DeletedEntity
    {
        [DisplayName("Role Name")]
        [Required(ErrorMessage = "Enter your role name.")]
        [StringLength(100)]
        public string RoleName { get; set; }
        [StringLength(500)]
        [Required(ErrorMessage = "Enter description.")]
        public string Description { get; set; }
    }
}
