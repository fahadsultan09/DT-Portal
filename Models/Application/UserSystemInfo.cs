using Microsoft.AspNetCore.Mvc.Rendering;
using Models.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models.Application
{
    public class UserSystemInfo : DeletedEntity
    {
        [Required(ErrorMessage = "Please select distributor.")]
        public int DistributorId { get; set; }
        [ForeignKey("DistributorId")]
        public virtual Distributor Distributor { get; set; }
        //[Required(ErrorMessage = "Enter Processor Id.")]
        [StringLength(50)]
        public string ProcessorId { get; set; }
        //[Required(ErrorMessage = "Enter Host Name.")]
        [StringLength(50)]
        public string HostName { get; set; }
        [Required(ErrorMessage = "Enter MAC Address.")]
        [StringLength(50)]
        public string MACAddress { get; set; }
        [StringLength(50)]
        public string OtherMACAddress { get; set; }
        [NotMapped]
        public SelectList DistributorList { get; set; }
    }
}
