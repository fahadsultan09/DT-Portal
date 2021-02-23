using Microsoft.AspNetCore.Mvc.Rendering;
using Models.Application;
using System;
using System.Collections.Generic;
using System.Text;
using Utility;

namespace Models.ViewModel
{
    public class ComplaintViewModel
    {
        public List<Complaint> ComplaintList { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public ComplaintStatus? Status { get; set; }
        public int? DistributorId { get; set; }
        public int? ComplaintNo { get; set; }
        public SelectList DistributorList { get; set; }
    }
}
