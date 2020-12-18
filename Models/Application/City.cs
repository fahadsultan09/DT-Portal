using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Models.Common;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models.Application
{
    public class City : ActionEntity
    {
        [DisplayName("Sub Region Name")]
        [Required(ErrorMessage = "Select Sub Region.")]
        [BindRequired]
        public int SubRegionId { get; set; }
        [ForeignKey("SubRegionId")]
        public virtual SubRegion SubRegion { get; set; }
        [DisplayName("City Name")]
        [Required(ErrorMessage = "Enter your city name.")]
        [StringLength(50)]
        public string CityName { get; set; }
        [NotMapped]
        public SelectList SubRegionList { get; set; }
    }
}
