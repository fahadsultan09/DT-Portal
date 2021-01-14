using Microsoft.AspNetCore.Http.Internal;
using Models.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Utility;

namespace Models.Application
{
    public class DistributorLicense : DeletedEntity
    {
        public int DistributorId { get; set; }
        [ForeignKey("DistributorId")]
        public virtual Distributor Distributor { get; set; }
        public int LicenseId { get; set; }
        [ForeignKey("LicenseId")]
        public virtual LicenseControl LicenseControl { get; set; }
        public int Type { get; set; }
        public string Attachment { get; set; }
        public DateTime IssueDate { get; set; }
        public DateTime Expiry { get; set; }
        public LicenseStatus Status { get; set; }
        [NotMapped]
        public FormFile File { get; set; }
    }
}
