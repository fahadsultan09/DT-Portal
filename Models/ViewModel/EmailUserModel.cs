using Models.Application;
using System;

namespace Models.ViewModel
{
    public class EmailUserModel
    {
        public string Day { get; set; }
        public string ComplaintNo { get; set; }
        public string ComplaintDate { get; set; }
        public string DistributorName { get; set; }
        public string ComplaintCategory { get; set; }
        public string Attachment { get; set; }
        public string ComplaintDetail { get; set; }
        public string URL { get; set; }
        public string ToAcceptTemplate { get; set; }
        public string CCEmail { get; set; }
        public int CreatedBy { get; set; }
    }
}
