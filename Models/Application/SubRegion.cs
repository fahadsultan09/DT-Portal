using Microsoft.AspNetCore.Mvc.Rendering;
using Models.Common;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models.Application
{
    public class SubRegion : ActionEntity
    {
        [DisplayName("Region")]
        [Required(ErrorMessage = "Please select region.")]
        public int RegionId { get; set; }
        [ForeignKey("RegionId")]
        public virtual Region Region { get; set; }

        [DisplayName("Sub Region Name")]
        [Required(ErrorMessage = "Enter your sub region name.")]
        [StringLength(100)]
        public string SubRegionName { get; set; }

        [NotMapped]
        public SelectList RegionList { get; set; }
    }
}
