using Models.Common;
using System.ComponentModel.DataAnnotations;

namespace Models.Application
{
    public class LicenseForm : UpdatedEntity
    {
        [StringLength(50)]
        public string LicenseFormNo { get; set; }
    }
}
