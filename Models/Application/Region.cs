using Microsoft.AspNetCore.Mvc.Rendering;
using Models.Common;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models.Application
{
    public class Region : ActionEntity
    {
        public string SAPId { get; set; }
        [DisplayName("Region Name")]
        [Required(ErrorMessage = "Enter your region name.")]
        [StringLength(100)]
        public string RegionName { get; set; }
        [NotMapped]
        public SelectList ZoneList { get; set; }
    }
}
