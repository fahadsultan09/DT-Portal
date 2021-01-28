using System;
using System.Collections.Generic;
using System.Text;
using Utility;

namespace Models.ViewModel
{
    public class ComplaintSearch : Search
    {
        public ComplaintStatus? Status { get; set; }

        public int? ComplaintNo { get; set; }
    }
}
