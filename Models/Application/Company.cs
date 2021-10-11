using Models.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Models.Application
{
    public class Company : DeletedEntity
    {
        [Required(ErrorMessage = "Enter your company name.")]
        [StringLength(255)]
        public string CompanyName { get; set; }
        public string SAPCompanyCode { get; set; }
        public bool IsPaymentAllowed { get; set; }
        public bool IsReturnOrderAllowed { get; set; }
        public bool IsPaymentAllowedInSAP { get; set; }
    }
}
