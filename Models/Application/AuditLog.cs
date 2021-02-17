using Models.Common;
using System;
using System.ComponentModel.DataAnnotations;

namespace Models.Application
{
    public class AuditLog : BaseEntity
    {
        [Required(ErrorMessage = "Enter page name.")]
        [StringLength(50)]
        public string PageName { get; set; }

        [Required(ErrorMessage = "Enter action name.")]
        [StringLength(50)]
        public string ActionName { get; set; }

        [Required(ErrorMessage = "Enter description.")]
        [StringLength(200)]
        public string Description { get; set; }

        [Required(ErrorMessage = "Enter Created By.")]
        [StringLength(10)]
        public string CreatedBy { get; set; }

        [Required(ErrorMessage = "Enter Created Date.")]
        public DateTime CreatedDate { get; set; }
    }
}
