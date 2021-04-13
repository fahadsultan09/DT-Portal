using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Models.Common;
using Models.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Utility;

namespace Models.Application
{
    public class OrderMaster : DeletedEntity
    {
        public int DistributorId { get; set; }
        [ForeignKey("DistributorId")]
        public virtual Distributor Distributor { get; set; }
        public string ReferenceNo { get; set; }
        public string Remarks { get; set; }
        public string Attachment { get; set; }
        [NotMapped]
        public IFormFile AttachmentFormFile { get; set; }
        public double TotalValue { get; set; }
        public OrderStatus Status { get; set; }
        public int? OnHoldBy { get; set; }
        public DateTime? OnHoldDate { get; set; }
        public string OnHoldComment { get; set; }
        public int? RejectedBy { get; set; }
        public DateTime? RejectedDate { get; set; }
        public string RejectedComment { get; set; }
        public int? ApprovedBy { get; set; }
        public DateTime? ApprovedDate { get; set; }        
        [NotMapped]
        public List<OrderDetail> OrderDetail { get; set; }
        [NotMapped]
        public List<ProductMaster> ProductMaster { get; set; }
        [NotMapped]
        public SelectList ProductList { get; set; }
        [NotMapped]
        public OrderValueViewModel OrderValueViewModel { get; set; }
        [NotMapped]
        public List<ProductDetail> productDetails { get; set; }
        [NotMapped]
        public List<DistributorWiseProductDiscountAndPrices> DistributorWiseProduct { get; set; }
        [NotMapped]
        public string CreatedName { get; set; }
        [NotMapped]
        public string ApprovedName { get; set; }
        [NotMapped]
        public string RejectedName { get; set; }
    }
}
