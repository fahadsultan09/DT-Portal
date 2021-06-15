using Microsoft.AspNetCore.Http;
using Models.Application;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Utility;

namespace Models.ViewModel
{
    public class OrderViewModel
    {
        public int Id { get; set; }
        public List<DistributorWiseProductDiscountAndPrices> ProductDetails { get; set; }
        [StringLength(64)]
        public string ReferenceNo { get; set; }
        public IFormFile AttachmentFormFile { get; set; }
        [StringLength(255)]
        public string Remarks { get; set; }
        [StringLength(500)]
        public string Attachment { get; set; }
        public OrderStatus Status { get; set; }
        public OrderValueViewModel OrderValues { get; set; }
    }
}
