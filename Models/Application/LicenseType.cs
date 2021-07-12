using Models.Common;
using System.ComponentModel.DataAnnotations;

namespace Models.Application
{
    public class LicenseType : UpdatedEntity
    {
        [StringLength(50)]
        public string LicenseTypeName { get; set; }
    }
}
