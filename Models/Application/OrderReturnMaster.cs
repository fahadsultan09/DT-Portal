using Microsoft.AspNetCore.Mvc.Rendering;
using Models.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Utility;

namespace Models.Application
{
    public class OrderReturnMaster : UpdatedEntity
    {
        public int DistributorId { get; set; }
        [ForeignKey("DistributorId")]
        public virtual Distributor Distributor { get; set; }
        public OrderReturnStatus Status { get; set; }
        public string Transporter { get; set; }
        public string TRNo { get; set; }
        public DateTime TRDate { get; set; }
        public string DebitNoteNo { get; set; }
        public DateTime DebitNoteDate { get; set; }
        public OrderReturnStatus ReturnReson { get; set; }
        public double TotalValue { get; set; }
        [NotMapped]
        public List<OrderReturnDetail> OrderReturnDetail { get; set; }
        [NotMapped]
        public List<ProductMaster> ProductMaster { get; set; }
        [NotMapped]
        public SelectList ProductList { get; set; }
        [NotMapped]
        public List<ProductDetail> ProductDetail { get; set; }
    }
}
