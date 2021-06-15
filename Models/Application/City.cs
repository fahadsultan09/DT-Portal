using Microsoft.AspNetCore.Mvc.Rendering;
using Models.Common;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models.Application
{
    public class City : DeletedEntity
    {
        [DisplayName("City Code")]
        [Required(ErrorMessage = "Enter your city code.")]
        [StringLength(50)]
        public string CityCode { get; set; }

        [DisplayName("City Name")]
        [Required(ErrorMessage = "Enter your city name.")]
        [StringLength(50)]
        public string CityName { get; set; }

        //[DisplayName("Sub Region")]
        //[Required(ErrorMessage = "Please select sub region.")]
        //public int SubRegionId { get; set; }
        //[ForeignKey("SubRegionId")]
        //public virtual SubRegion SubRegion { get; set; }

        //[NotMapped]
        //[DisplayName("Region")]
        //[Required(ErrorMessage = "Please select region.")]
        //public long RegionId { get; set; }

        //[NotMapped]
        //public SelectList RegionList { get; set; }

        [NotMapped]
        public SelectList SubRegionList { get; set; }
    }
}
