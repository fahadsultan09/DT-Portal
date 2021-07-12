using Microsoft.AspNetCore.Http;
using Models.Common;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
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
        [DisplayName("Form No")]
        public int? FormNoId { get; set; }
        [ForeignKey("FormNoId")]
        public virtual LicenseForm LicenseForm { get; set; }
        public int? LicenseTypeId { get; set; }
        [ForeignKey("LicenseTypeId")]
        public virtual LicenseType LicenseType { get; set; }
        [DisplayName("Document Type")]
        public DocumentType? DocumentType { get; set; }
        [DisplayName("Request Type")]
        public LicenseRequestType? RequestType { get; set; }
        [StringLength(50)]
        public string IssuingAuthority { get; set; }
        [StringLength(50)]
        public string LicenseNo { get; set; }
        [StringLength(1000)]
        public string Attachment { get; set; }
        public DateTime IssueDate { get; set; }
        public DateTime Expiry { get; set; }
        public LicenseStatus? Status { get; set; }
        [StringLength(255)]
        public string Remarks { get; set; }
        [NotMapped]
        public IFormFile File { get; set; }
        [NotMapped]
        public int Days { get; set; }
    }
}
