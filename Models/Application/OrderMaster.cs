using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Models.Common;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Utility;

namespace Models.Application
{
    public class OrderMaster : UpdatedEntity
    {
        public int DistributorId { get; set; }
        [ForeignKey("DistributorId")]
        public virtual Distributor Distributor { get; set; }
        [Required(ErrorMessage = "Reference No is required.")]
        public string ReferenceNo { get; set; }
        [Required(ErrorMessage = "Remarks is required.")]
        public string Remarks { get; set; }

        [Required(ErrorMessage = "Attachment is required.")]
        public string Attachment { get; set; }
        [NotMapped]
        public IFormFile AttachmentFormFile { get; set; }
        public double TotalValue { get; set; }
        public OrderStatus Status { get; set; }        
        [NotMapped]
        public List<OrderDetail> OrderDetail { get; set; }
        [NotMapped]
        public List<ProductMaster> ProductMaster { get; set; }
        [NotMapped]
        public SelectList ProductList { get; set; }
    }
}
