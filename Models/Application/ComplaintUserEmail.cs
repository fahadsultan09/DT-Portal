using Models.Common;
using System.ComponentModel.DataAnnotations.Schema;
using Utility;

namespace Models.Application
{
    public class ComplaintUserEmail : CreatedEntity
    {
        public int ComplaintSubCategoryId { get; set; }
        [ForeignKey("ComplaintSubCategoryId")]
        public virtual ComplaintSubCategory ComplaintSubCategory { get; set; }
        public EmailType EmailType { get; set; }
        public string UserEmailId { get; set; }
        //[ForeignKey("UserEmailId")]
        //public virtual User User { get; set; }
    }
}
