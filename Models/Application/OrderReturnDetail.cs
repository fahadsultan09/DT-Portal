using Models.Common;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Utility;

namespace Models.Application
{
    public class OrderReturnDetail : CreatedEntity
    {
        public int OrderReturnId { get; set; }
        [ForeignKey("OrderReturnId")]
        public virtual OrderReturnMaster OrderReturnMaster { get; set; }
        public int ProductId { get; set; }
        [ForeignKey("ProductId")]
        public virtual ProductMaster ProductMaster { get; set; }
        public int PlantLocationId { get; set; }
        [ForeignKey("PlantLocationId")]
        public virtual PlantLocation PlantLocation { get; set; }
        public string TRNo { get; set; }
        public int Quantity { get; set; }
        public int ReceivedQty { get; set; }
        public int? ReceivedBy { get; set; }
        public DateTime? ReceivedDate { get; set; }
        [Range(1, 9999999999)]
        [Column(TypeName = "double")]
        [Required(ErrorMessage = "Amount is required.")]
        public double MRP { get; set; }
        [StringLength(10)]
        public string BatchNo { get; set; }
        public string InvoiceNo { get; set; }
        public DateTime InvoiceDate { get; set; }
        public DateTime ExpiryDate { get; set; }
        public DateTime ManufactureDate { get; set; }
        public DateTime IntimationDate { get; set; }
        public string Remarks { get; set; }
        public string ReturnOrderNumber { get; set; }
        public OrderStatus? ReturnOrderStatus { get; set; }
        public bool IsProductSelected { get; set; }
        [NotMapped]
        public double TotalPrice { get; set; }
        [NotMapped]
        public double Discount { get; set; }
        [NotMapped]
        public double NetAmount { get; set; }
        [NotMapped]
        public int OrderReturnNumber { get; set; }        
        [NotMapped]
        public Company Company { get; set; }
        [NotMapped]
        public bool IsFOCProduct { get; set; }
    }
}
