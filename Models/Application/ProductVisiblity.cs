using Models.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Models.Application
{
    public class ProductVisiblity : ActionEntity
    {
        [Required(ErrorMessage = "Enter your visibility name.")]
        [StringLength(255)]
        public string VisibilityName { get; set; }
    }
}
