using Models.Common;
using System.ComponentModel.DataAnnotations;

namespace Models.ErrorLog
{
    public class ExceptionLog : CreatedEntity
    {
        [StringLength(255)]
        public string MemberName { get; set; }
        [StringLength(255)]
        public string MethodBase { get; set; }
        [StringLength(255)]
        public string MemberType { get; set; }
        [StringLength(255)]
        public string FilePath { get; set; }
        [StringLength(255)]
        public string ClassName { get; set; }
        [StringLength(255)]
        public string ExceptionMessage { get; set; }
        [StringLength(255)]
        public string Source { get; set; }
        [StringLength(10000)]
        public string Trace { get; set; }
        [StringLength(255)]
        public string ErrorLineNumber { get; set; }
        [StringLength(255)]
        public string ThrowLineNumber { get; set; }
        [StringLength(255)]
        public string ColumnNumber { get; set; }
        [StringLength(255)]
        public string HelpLink { get; set; }
    }
}
