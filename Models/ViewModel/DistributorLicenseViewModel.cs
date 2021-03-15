using Microsoft.AspNetCore.Mvc.Rendering;
using Models.Application;
using System;
using System.Collections.Generic;
using System.Text;
using Utility;

namespace Models.ViewModel
{
    public class DistributorLicenseViewModel
    {
        public List<DistributorLicense> DistributorLicenseList { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public LicenseStatus? Status { get; set; }
        public int? DistributorId { get; set; }
        public int? LicenseId { get; set; }
        public SelectList DistributorList { get; set; }
        public SelectList LicenseControlList { get; set; }
    }
}
