using Models.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Models.Application
{
    public class Company : ActionEntity
    {
        [Required(ErrorMessage = "Enter your company name.")]
        [StringLength(255)]
        public string CompanyName { get; set; }
        public string SAPCompanyCode { get; set; }
    }
}
