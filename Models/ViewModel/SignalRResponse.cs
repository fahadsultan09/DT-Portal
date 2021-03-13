using System;
using System.Collections.Generic;
using System.Text;

namespace Models.ViewModel
{
    public class SignalRResponse
    {
        public string UserId { get; set; }
        public string Number { get; set; }
        public string Status { get; set; }
        public string Message { get; set; }
    }
}
