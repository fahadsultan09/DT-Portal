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
}
