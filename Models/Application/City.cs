using Models.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Models.Application
{
    public class City : ActionEntity
    {
        public int SubRegionId { get; set; }
        [ForeignKey("SubRegionId")]
        public virtual SubRegion SubRegion { get; set; }
        public string CityName { get; set; }
    }
}
