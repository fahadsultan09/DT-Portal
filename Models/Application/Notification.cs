using Models.Common;
using System.ComponentModel.DataAnnotations.Schema;

public class Notification : CreatedEntity
{
	public int? CompanyId { get; set; }
	public int ApplicationPageId { get; set; }
	public int RequestId { get; set; }
	public int DistributorId { get; set; }
	public string Status { get; set; }
	public string Message { get; set; }
	public bool IsView { get; set; }
    public bool IsOrderView { get; set; }
    public bool IsOrderReturnView { get; set; }
    public bool IsPaymentView { get; set; }
    public string URL { get; set; }
	[NotMapped]
	public string RelativeTime { get; set; }
}