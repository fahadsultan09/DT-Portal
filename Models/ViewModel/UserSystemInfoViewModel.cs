using System;
using System.Collections.Generic;
using System.Text;

namespace Models.ViewModel
{
    public class UserSystemInfoViewModel
    {
        public int Id { get; set; }
        public string MACAddress { get; set; }
        public bool IsRowDeleted { get; set; }
    }
}
