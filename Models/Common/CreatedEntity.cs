using System;

namespace Models.Common
{
    public class CreatedEntity : BaseEntity
    {
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }

    }
}
