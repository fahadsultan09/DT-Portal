using System;

namespace Models.Common
{
    public class DeletedEntity : UpdatedEntity
    {
        public int? DeletedBy { get; set; }
        public DateTime? DeletedDate { get; set; }
    }
}
