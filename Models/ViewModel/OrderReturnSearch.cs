using System;
using System.Collections.Generic;
using System.Text;
using Utility;

namespace Models.ViewModel
{
    public class OrderReturnSearch : Search
    {
        public OrderReturnStatus? Status { get; set; }
        public int? OrderReturnNo { get; set; }
        public string TRNo { get; set; }
    }
}
