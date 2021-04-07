using Models.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models.Application
{
    public class UserSystemInfoDetail : CreatedEntity
    {
        [Required]
        public int UserSystemInfoId { get; set; }
        [ForeignKey("UserSystemInfoId")]
        public virtual UserSystemInfo UserSystemInfo { get; set; }
        [Required]
        [StringLength(50)]
        public string MACAddress { get; set; }
        [NotMapped]
        public bool IsRowDeleted { get; set; }
    }
}
