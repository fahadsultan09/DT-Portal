using Models.Common;
using System.ComponentModel.DataAnnotations;

namespace Models.Application
{
    public class Bank : ActionEntity
    {
        [Required(ErrorMessage = "Enter your Bank name.")]
        [StringLength(255)]
        public string BankName { get; set; }
    }
}
