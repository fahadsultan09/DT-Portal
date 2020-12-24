using Models.Common;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Models.Application
{
    public class Region : ActionEntity
    {
        [DisplayName("Region Name")]
        [Required(ErrorMessage = "Enter your region name.")]
        [StringLength(50)]
        public string RegionName { get; set; }
        public string SAPId { get; set; }
    }
}
