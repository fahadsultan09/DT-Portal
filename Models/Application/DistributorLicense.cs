using Microsoft.AspNetCore.Http;
using Models.Common;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using Utility;

namespace Models.Application
{
    public class DistributorLicense : DeletedEntity
    {
        public int DistributorId { get; set; }
        [ForeignKey("DistributorId")]
        public virtual Distributor Distributor { get; set; }
        public int? LicenseId { get; set; }
        [ForeignKey("LicenseId")]
        public virtual LicenseControl LicenseControl { get; set; }
        public LicenseType Type { get; set; }
        public LicenseRequestType RequestType { get; set; }
        public string Attachment { get; set; }
        public DateTime IssueDate { get; set; }
        public DateTime Expiry { get; set; }
        public LicenseStatus Status { get; set; }
        public string Remarks { get; set; }
        [NotMapped]
        public IFormFile File { get; set; }
    }
}
