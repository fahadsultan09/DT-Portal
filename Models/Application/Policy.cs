using Models.Common;
using System.ComponentModel.DataAnnotations;

namespace Models.Application
{
    public class Policy : DeletedEntity
    {
        [Required(ErrorMessage = "Enter your title.")]
        [StringLength(64)]
        public string Title { get; set; }
        [Required(ErrorMessage = "Enter your icon.")]
        [StringLength(64)]
        public string Icon { get; set; }
        [Required(ErrorMessage = "Enter your Style.")]
        [StringLength(64)]
        public string Style { get; set; }
        [Required(ErrorMessage = "Enter your message.")]
        [StringLength(255)]
        public string Message { get; set; }
        [Required(ErrorMessage = "Enter Sort.")]
        public int Sort { get; set; }
    }
}
