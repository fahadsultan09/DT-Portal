using System.ComponentModel.DataAnnotations;

namespace Utility
{
    public enum IsActive
    {
        True = 1,
        False = 0
    }

    public enum IsDeleted
    {
        True = 1,
        False = 0
    }    

    public enum LoginStatus
    {
        Success,
        Failed
    }

    public enum DistributorStatus
    {
        Local = 1,
        Home = 2
    }

    public enum OrderStatus
    {
        Draft = 1,
        Submit = 2,
        Approved = 3,
        OnHold = 4,
        Reject = 5,
        Complete = 6
    }
    public enum ProductVisibility
    { 
        Visible = 1,
        Hide = 2
    }

    public enum OrderValues
    {
        [Display(Name = "0% Supplies")]
        Supplies0 = 0,
        [Display(Name = "1% Supplies")]
        Supplies1 = 1,
        [Display(Name = "4% Supplies")]
        Supplies4 = 2,
        [Display(Name = "Total Order Value")]
        TotalOrderValues = 3,
        [Display(Name = "Pending Order Values")]
        PendingOrderValues = 4,
        [Display(Name = "Current Balance")]
        CurrentBalance = 5,
        [Display(Name = "UnConfirmed Payment")]
        UnConfirmedPayment = 6,
        [Display(Name = "Net Payable")]
        NetPayable = 7,
    }
}
