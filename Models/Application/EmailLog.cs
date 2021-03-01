using Models.Common;
using System.ComponentModel.DataAnnotations;

namespace Models.Application
{
    public class EmailLog : CreatedEntity
    {
        [Required(ErrorMessage = "Enter To Email.")]
        [StringLength(100)]
        public string ToEmail { get; set; }
        public string CCEmail { get; set; }
        [Required(ErrorMessage = "Enter Subject.")]
        [StringLength(255)]
        public string Subject { get; set; }
        [Required(ErrorMessage = "Enter Message.")]
        [StringLength(5000)]
        public string Message { get; set; }
        public bool IsSend { get; set; }
    }
}
