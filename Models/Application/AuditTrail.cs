using Models.Common;
using Models.UserRights;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models.Application
{
    public class AuditTrail : BaseEntity
    {
        public int PageId { get; set; }
        [ForeignKey("PageId")]
        public virtual ApplicationPage ApplicationPage { get; set; }
        public int ActionId { get; set; }
        [ForeignKey("ActionId")]
        public virtual ApplicationAction ApplicationAction { get; set; }
        public string JsonObject { get; set; }
        public string KeyValue { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }
        [StringLength(255)]
        public string Remarks { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        [NotMapped]
        public object Oject { get; set; }
    }
}
