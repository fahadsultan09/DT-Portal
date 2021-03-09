using Models.Common;
using System.ComponentModel.DataAnnotations;

namespace Models.Application
{
    public class PlantLocation : UpdatedEntity
    {
        [StringLength(255)]
        public string PlantLocationName { get; set; }
        [StringLength(255)]
        public string Address { get; set; }
    }
}
