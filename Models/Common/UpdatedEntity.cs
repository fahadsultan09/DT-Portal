using System;

namespace Models.Common
{
    public class UpdatedEntity : ActionEntity
    {
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
