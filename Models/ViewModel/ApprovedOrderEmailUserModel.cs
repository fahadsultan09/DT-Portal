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
        public string SAPOrder { get; set; }
        public string SAPOrderNumber { get; set; }
        public string DPOrder { get; set; }
        public string DPOrderNumber { get; set; }
        public string ToAcceptTemplate { get; set; }
        public string Subject { get; set; }
        public int CreatedBy { get; set; }
        public string CCEmail { get; set; }
    }
}
