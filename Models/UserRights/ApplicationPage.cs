using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Models.Common;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models.UserRights
{
    public class ApplicationPage : DeletedEntity
    {
        [DisplayName("Application Module")]
        [Required(ErrorMessage = "Please select application module.")]
        [BindRequired]
        public int ApplicationModuleId { get; set; }
        [ForeignKey("ApplicationModuleId")]
        public virtual ApplicationModule ApplicationModule { get; set; }
        [StringLength(100)]
        [DisplayName("Controller Name")]
        [Required(ErrorMessage = "Enter your controller name.")]
        public string ControllerName { get; set; }
        [StringLength(100)]
        [DisplayName("Page Title")]
        [Required(ErrorMessage = "Enter your page title.")]
        public string PageTitle { get; set; }
        [DisplayName("Page URL")]
        [Required(ErrorMessage = "Enter your page url.")]
        [StringLength(200)]
        public string PageURL { get; set; }
        [StringLength(50)]
        [DisplayName("Page Icon")]
        [Required(ErrorMessage = "Enter your page icon.")]
        public string PageIcon { get; set; }
        [DisplayName("Sort")]
        [Required(ErrorMessage = "Enter your sort.")]
        public int Sort { get; set; }
        [NotMapped]
        public SelectList ApplicationModuleList { get; set; }
        [NotMapped]
        [DisplayName("Application Actions")]
        [Required(ErrorMessage = "Please select application action.")]
        [BindRequired]
        public int[] ApplicationActionsId { get; set; }
        [NotMapped]
        public MultiSelectList ApplicationActionsList { get; set; }
    }
}
