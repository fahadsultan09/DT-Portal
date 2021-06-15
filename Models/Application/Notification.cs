using Models.Common;
using System;
using System.ComponentModel.DataAnnotations.Schema;

public class Notification : CreatedEntity
{
	public int ApplicationPageId { get; set; }
	public int RequestId { get; set; }
	public int DistributorId { get; set; }
	public string Status { get; set; }
	public string Message { get; set; }
	public bool IsView { get; set; }
	public string URL { get; set; }
	[NotMapped]
	public string RelativeTime { get; set; }
}