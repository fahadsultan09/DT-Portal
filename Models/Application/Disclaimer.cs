using Models.Common;
using System.ComponentModel.DataAnnotations;

namespace Models.Application
{
    public class Disclaimer : DeletedEntity
    {
        [StringLength(50)]
        [Required(ErrorMessage = "Enter your disclaimer name.")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Enter your description.")]
        [StringLength(2550)]
        public string Description { get; set; }
    }
}
