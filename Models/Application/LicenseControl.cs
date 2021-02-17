using Models.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Application
{
    public class LicenseControl : ActionEntity
    {
        public string LicenseName { get; set; }
        public int LicenseAcceptanceInDay { get; set; }
        public bool IsMandatory { get; set; }
        public int DaysIntimateBeforeExpiry { get; set; }
    }
}
