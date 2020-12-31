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
        PendingApproval = 2,
        PaymentVerified = 3,
        InProcess = 4,
        PartiallyProcessed = 5,
        CompletelyProcessed = 6,
        Onhold = 6,
        Reject = 7

    }
    public enum PaymentStatus
    {
        Verified = 1,
        Unverified = 2
    }
    public enum ProductVisibility
    {
        Visible = 1,
        Hide = 2
    }
}
