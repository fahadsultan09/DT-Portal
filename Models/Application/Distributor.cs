using Models.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Utility;

namespace Models.Application
{
    public class Distributor : DeletedEntity
    {
        public int DistributorSAPCode { get; set; }
        public string DistributorCode { get; set; }
        public string DistributorName { get; set; }
        public string DistributorAddress { get; set; }
        public int CityId { get; set; }
        [ForeignKey("CityId")]
        public virtual City City { get; set; }
        public int SubRegionId { get; set; }
        [ForeignKey("SubRegionId")]
        public virtual SubRegion SubRegion { get; set; }
        public int RegionId { get; set; }
        [ForeignKey("RegionId")]
        public virtual Region Region { get; set; }
        public string NTN { get; set; }
        public string CNIC { get; set; }
        public string EmailAddress { get; set; }
        public DistributorStatus Status { get; set; }
    }
}
