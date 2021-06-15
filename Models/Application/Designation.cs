using Models.Common;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Models.Application
{
    public class Designation : DeletedEntity
    {
        [DisplayName("Designation Name")]
        [Required(ErrorMessage = "Enter your designation name.")]
        [StringLength(45)]
        public string DesignationName { get; set; }
    }
}
