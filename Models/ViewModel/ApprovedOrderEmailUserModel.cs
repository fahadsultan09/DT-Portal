using System;
using System.Collections.Generic;
using System.Text;

namespace Models.ViewModel
{
    public class ApprovedOrderEmailUserModel
    {
        public string Date { get; set; }
        public string ShipToPartyName { get; set; }
        public string City { get; set; }
        public string OrderNumber { get; set; }
        public string ToAcceptTemplate { get; set; }
        public string Subject { get; set; }
        public int CreatedBy { get; set; }
    }
}
