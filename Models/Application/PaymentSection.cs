using Models.Common;
using System.ComponentModel.DataAnnotations;

namespace Models.Application
{
    public class PaymentSection : DeletedEntity
	{
		[StringLength(50)]
		public string Name { get; set; }
		[StringLength(50)]
		public int GLAccount { get; set; }
	}
}
