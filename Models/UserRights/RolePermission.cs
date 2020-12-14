using Models.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models.UserRights
{
    public class RolePermission : CreatedEntity
    {
        public int RoleId { get; set; }
        [ForeignKey("RoleId")]
        public virtual Role Role { get; set; }
        public int ApplicationPageId { get; set; }
        [ForeignKey("ApplicationPageId")]
        public virtual ApplicationPage ApplicationPage { get; set; }
        public int ApplicationPageActionId { get; set; }
        [ForeignKey("ApplicationPageActionId")]
        public virtual ApplicationPageAction ApplicationPageAction { get; set; }
        public int ApplicationActionId { get; set; }
        [ForeignKey("ApplicationActionId")]
        public virtual ApplicationAction ApplicationAction { get; set; }
    }
}
