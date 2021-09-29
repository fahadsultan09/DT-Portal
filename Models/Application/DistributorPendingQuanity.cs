using Models.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models.Application
{
    public class DistributorPendingQuantity : UpdatedEntity
    {
        public int DistributorId { get; set; }
        [ForeignKey("DistributorId")]
        public virtual Distributor Distributor { get; set; }
        public string ProductCode { get; set; }
        public int OrderQuantity { get; set; }
        public int DispatchQuantity { get; set; }
        public int PendingQuantity { get; set; }
        public double PendingValue { get; set; }
        [NotMapped]
        public string ProductName { get; set; }
        [NotMapped]
        public double Rate { get; set; }
        [NotMapped]
        public double Discount { get; set; }
        [NotMapped]
        public int CompanyId { get; set; }
        [NotMapped]
        public ProductDetail ProductDetail { get; set; }
    }
}
