using Models.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Models.Application
{
    public class Designation : DeletedEntity
    {
        [DisplayName("Designation Name")]
        [Required(ErrorMessage = "Enter your designation name.")]
        [StringLength(45)]
        public string DesignationName { get; set; }
    }
}
