using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Text;
using Utility;

namespace Models.ViewModel
{
    public class OrderSearch : Search
    {
        public OrderStatus? Status { get; set; }
        public int? OrderNo { get; set; }
    }
}
