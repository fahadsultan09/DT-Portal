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
        [StringLength(2550, ErrorMessage = "The field Description must be a string with a maximum length of 2550. (Including code)")]
        public string Description { get; set; }
    }
}
