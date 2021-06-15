using Models.Common;
using System.ComponentModel.DataAnnotations;

namespace Models.Application
{
    public class LicenseControl : ActionEntity
    {
        [Required(ErrorMessage = "Enter your license name.")]
        [StringLength(255)]
        [MaxLength(255)]
        public string LicenseName { get; set; }
        public int LicenseAcceptanceInDay { get; set; }
        public bool IsMandatory { get; set; }
        public int DaysIntimateBeforeExpiry { get; set; }
    }
}
