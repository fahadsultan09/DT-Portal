using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Models.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Models.Application
{
    public class SubRegion : ActionEntity
    {
        [DisplayName("Region Name")]
        [Required(ErrorMessage = "Select Region.")]
        [BindRequired]
        public int RegionId { get; set; }
        [ForeignKey("RegionId")]
        public virtual Region Region { get; set; }
        [DisplayName("Sub Region Name")]
        [Required(ErrorMessage = "Enter your sub region name.")]
        [StringLength(50)]
        public string SubRegionName { get; set; }
        [NotMapped]
        public SelectList RegionList { get; set; }
    }
}
