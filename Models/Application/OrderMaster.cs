using Microsoft.AspNetCore.Mvc.Rendering;
using Models.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Utility;

namespace Models.Application
{
    public class OrderMaster : UpdatedEntity
    {
        public int DistributorId { get; set; }
        [ForeignKey("DistributorId")]
        public virtual Distributor Distributor { get; set; }
        public string ReferenceNo { get; set; }
        public string Remarks { get; set; }
        public string Attachment { get; set; }
        public decimal TotalValue { get; set; }
        public OrderStatus Status { get; set; }
        public string Comments { get; set; }
        [NotMapped]
        public List<OrderDetail> OrderDetail { get; set; }
        [NotMapped]
        public SelectList ProductList { get; set; }
    }
}
