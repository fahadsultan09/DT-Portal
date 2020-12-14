using Models.Common;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Models.UserRights
{
    public class ApplicationModule : UpdatedEntity
    {
        [DisplayName("Module Name")]
        [Required(ErrorMessage = "Enter your module name.")]
        [StringLength(50)]
        public string ModuleName { get; set; }

        [DisplayName("Module Icon")]
        [Required(ErrorMessage = "Enter your module icon.")]
        [StringLength(50)]
        public string ModuleIcon { get; set; }

        [DisplayName("Sort")]
        [Required(ErrorMessage = "Enter your sort.")]
        public int Sort { get; set; }
    }
}
