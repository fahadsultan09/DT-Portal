using Microsoft.AspNetCore.Http;
using Models.Application;
using System;
using System.Collections.Generic;
using System.Text;
using Utility;

namespace Models.ViewModel
{
    public class OrderViewModel
    {
        public int Id { get; set; }
        public List<DistributorWiseProductDiscountAndPrices> ProductDetails { get; set; }
        public string ReferenceNo { get; set; }
        public IFormFile AttachmentFormFile { get; set; }
        public string Remarks { get; set; }
        public string Attachment { get; set; }
        public SubmitStatus SubmitStatus { get; set; }
        public OrderStatus Status { get; set; }
        public OrderValueViewModel OrderValues { get; set; }
    }
}
