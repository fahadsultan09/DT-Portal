using Models.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models.Application
{
    public class DistributorPendingValue : UpdatedEntity
    {
        public int DistributorId { get; set; }
        [ForeignKey("DistributorId")]
        public virtual Distributor Distributor { get; set; }
        public string CompanyCode { get; set; }
        public string PendingValue { get; set; }
    }
}
