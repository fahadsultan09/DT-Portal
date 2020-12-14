using Models.Common;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Models.UserRights
{
    public class ApplicationAction : UpdatedEntity
    {
        [DisplayName("Action Name")]
        [Required(ErrorMessage = "Enter your action name.")]
        [StringLength(50)]
        public string ActionName { get; set; }
    }
}
