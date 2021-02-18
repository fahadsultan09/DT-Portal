using Models.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Utility;

namespace Models.Application
{
    public class OrderDetail : CreatedEntity
    {
        public int OrderId { get; set; }
        [ForeignKey("OrderId")]
        public virtual OrderMaster OrderMaster { get; set; }
        public int ProductId { get; set; }
        [ForeignKey("ProductId")]
        public virtual ProductMaster ProductMaster { get; set; }
        public int Quantity { get; set; }
        public double Amount { get; set; }
        public string SAPOrderNumber { get; set; }
        public OrderStatus? OrderProductStatus { get; set; }
        public int ApprovedQuantity { get; set; }
        public bool IsProductSelected { get; set; }
        [NotMapped]
        public bool IsRowDeleted { get; set; }
    }
}
