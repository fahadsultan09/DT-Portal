using System;
using System.Collections.Generic;
using System.Text;

namespace Models.ViewModel
{
    public class JsonResponse
    {
        public string Message { get; set; }
        public bool Status { get; set; }
        public string RedirectURL { get; set; }
    }
}
