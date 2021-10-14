using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Models.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models.Application
{
    public class SubDistributor : DeletedEntity
    {
        [BindRequired]
        [Required(ErrorMessage = "Select Distributor.")]
        public int DistributorId { get; set; }
        [ForeignKey("DistributorId")]
        public virtual Distributor Distributor { get; set; }
        public int SubDistributorId { get; set; }
        [ForeignKey("SubDistributorId")]
        public virtual Distributor SubDistributors { get; set; }
        public bool IsParent { get; set; }
        [NotMapped]
        [BindRequired]
        [Required(ErrorMessage = "Select Sub Distributor.")]
        public int[] SubDistributorIds { get; set; }
        [NotMapped]
        public SelectList DistributorList { get; set; }
        [NotMapped]
        public SelectList SubDistributorList { get; set; }
    }
}
