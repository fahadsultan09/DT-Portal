using System;
using System.Collections.Generic;

namespace Models.ViewModel
{
    public class AuditEntry
    {
        public Guid Id { get; set; }
        public int ActionId { get; set; }
        public int PageId { get; set; }
        public string AuditUser { get; set; }
        public int Key { get; set; }
        public string TableName { get; set; }
        public Dictionary<string, object> KeyValue { get; } = new Dictionary<string, object>();
        public Dictionary<string, object> OldValue { get; } = new Dictionary<string, object>();
        public Dictionary<string, object> NewValue { get; } = new Dictionary<string, object>();
    }
}
