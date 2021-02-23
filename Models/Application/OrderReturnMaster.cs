using Microsoft.AspNetCore.Mvc.Rendering;
using Models.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Utility;

namespace Models.Application
{
    public class OrderReturnMaster : UpdatedEntity
    {
        public int DistributorId { get; set; }
        [ForeignKey("DistributorId")]
        public virtual Distributor Distributor { get; set; }
        [Required(ErrorMessage = "Plant Location is required.")]
        public OrderReturnStatus Status { get; set; }
        [Required(ErrorMessage = "Transporter is required.")]
        public string Transporter { get; set; }
        public string TRNo { get; set; }
        [Required(ErrorMessage = "TR Date is required.")]
        public DateTime TRDate { get; set; }
        [Required(ErrorMessage = "Debit Note No is required.")]
        public string DebitNoteNo { get; set; }
        [Required(ErrorMessage = "Debit Note Date is required.")]
        public DateTime DebitNoteDate { get; set; }
        [Required(ErrorMessage = "Return Reson is required.")]
        public int OrderReturnReasonId { get; set; }
        [ForeignKey("OrderReturnReasonId")]
        public virtual OrderReturnReason OrderReturnReason { get; set; }
        [Range(1, 999999999)]
        [Column(TypeName = "double")]
        public double TotalValue { get; set; }
        [NotMapped]
        public List<OrderReturnDetail> OrderReturnDetail { get; set; }
        [NotMapped]
        public List<ProductMaster> ProductMaster { get; set; }
        [NotMapped]
        public SelectList ProductList { get; set; }
        [NotMapped]
        public SelectList returnReasonList { get; set; }
        [NotMapped]
        public List<ProductDetail> ProductDetail { get; set; }
        [NotMapped]
        public string CreatedName { get; set; }
    }
}
