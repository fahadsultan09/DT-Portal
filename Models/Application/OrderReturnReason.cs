using Models.Common;
using System.ComponentModel.DataAnnotations;

namespace Models.Application
{
    public class OrderReturnReason : ActionEntity
    {
        [StringLength(255)]
        public string ReasonName { get; set; }
    }
}
