using Models.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Models.Application
{
    public class PaymentMode : ActionEntity
    {
        [Required(ErrorMessage = "Enter your payment name.")]
        [StringLength(255)]
        public string PaymentName { get; set; }
    }
}
