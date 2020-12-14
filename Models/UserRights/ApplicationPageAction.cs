using Models.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models.UserRights
{
    public class ApplicationPageAction : CreatedEntity
    {
        public int ApplicationPageId { get; set; }
        [ForeignKey("ApplicationPageId")]
        public virtual ApplicationPage ApplicationPage { get; set; }

        public int ApplicationActionId { get; set; }
        [ForeignKey("ApplicationActionId")]
        public virtual ApplicationAction ApplicationAction { get; set; }
    }
}
