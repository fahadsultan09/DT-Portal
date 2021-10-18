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
        Failed,
        NotRegistered
    }

    public enum DistributorStatus
    {
        Local = 1,
        Home = 2
    }

    public enum ProductEnum
    {
        [Display(Name = "Unmapped")]
        ProductMaster = 1,
        [Display(Name = "Mapped")]
        ProductMapping = 2
    }

    public enum OrderStatus
    {
        Draft = 1,
        [Display(Name = "Pending Approval")]
        PendingApproval = 2,
        [Display(Name = "In Process")]
        InProcess = 4,
        [Display(Name = "Partially Processed")]
        PartiallyProcessed = 5,
        [Display(Name = "Completely Processed")]
        CompletelyProcessed = 6,
        Rejected = 7,
        [Display(Name = "On hold")]
        Onhold = 9,
        Approved = 10,
        [Display(Name = "Not Yet Process")]
        NotYetProcess = 11,
        Canceled = 12,
        [Display(Name = "Partially Approved")]
        PartiallyApproved = 13,
        [Display(Name = "Approved Canceled")]
        ApprovedCanceled = 14
    }
    public enum SAPProductOrderStatus
    {
        NotYetProcessed = 'A',
        PartiallyProcessed = 'B',
        CompletelyProcessed = 'C',
    }

    public enum DistributorOrderStatus
    {
        Draft = 1,
        [Display(Name = "Pending Approval")]
        PendingApproval = 2,
        [Display(Name = "In Process")]
        InProcess = 4,
        [Display(Name = "Partially Processed")]
        PartiallyProcessed = 5,
        [Display(Name = "Completely Processed")]
        CompletelyProcessed = 6,
        Rejected = 7,
        Submitted = 8,
        [Display(Name = "On hold")]
        Onhold = 9,
        Approved = 10,
        [Display(Name = "Not Yet Process")]
        NotYetProcess = 11,
        Canceled = 12,
        [Display(Name = "Partially Approved")]
        PartiallyApproved = 13
    }
    public enum PaymentStatus
    {
        Verified = 1,
        Unverified = 2,
        Rejected = 3,
        Canceled = 4,
        Resubimt = 5,
    }
    public enum TaxChallanStatus
    {
        Verified = 1,
        Unverified = 2,
        Rejected = 3,
        Canceled = 4,
    }
    public enum ProductVisibility
    {
        Visible = 1,
        Hide = 2,
        [Display(Name = "Order Return")]
        OrderReturn = 3,
        [Display(Name = "Order Dispatch")]
        OrderDispatch = 4
    }
    public enum EmailIntimation
    {
        Both = 1,
        [Display(Name = "Order Return")]
        OrderReturn = 2,
        [Display(Name = "Order Dispatch")]
        OrderDispatch = 3
    }

    public enum CompanyEnum
    {
        SAMI = 1,
        Healthtek = 2,
        Phytek = 3
    }

    public enum SubmitStatus
    {
        [Display(Name = "Order Now")]
        OrderNow = 1,
        Draft = 2,
        Submit = 3,
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
    public enum ComplaintStatus
    {
        Pending = 1,
        Resolved = 2,
        Approved = 3,
        Rejected = 4,
        [Display(Name = "In Process")]
        InProcess = 5,
    }
    public enum DistributorComplaintStatus
    {
        Pending = 1,
        Resolved = 2,
        [Display(Name = "In Process")]
        InProcess = 5,
    }

    public enum LicenseStatus
    {
        Submitted = 1,
        Verified = 2,
        Rejected = 3,
        Expired = 4,
        Apply = 0
    }

    public enum DocumentType
    {
        License = 1,
        Challan = 2,
        Application = 3
    }
    public enum LicenseRequestType
    {
        [Display(Name = "New License")]
        NewLicense = 1,
        [Display(Name = "Renewal Of License")]
        RenewalOfLicense = 2,
        [Display(Name = "Change In Particular")]
        ChangeInParticular = 3
    }
    public enum OrderReturnStatus
    {
        Draft = 1,
        Submitted = 2,
        Received = 3,
        [Display(Name = "Completely Processed")]
        CompletelyProcessed = 4,
        [Display(Name = "Partially Received")]
        PartiallyReceived = 5,
        Rejected = 6,
    }
    public enum OrderReturnStatusDD
    {
        Draft = 1,
        Submitted = 2,
        Received = 3,
        CompletelyProcessed = 4,
        PartiallyReceived = 5,
        Rejected = 6,
    }
    public enum ApplicationActions
    {
        View = 1,
        Insert = 2,
        Update = 3,
        Delete = 4,
        Approve = 5,
        Reject = 6,
        Onhold = 7,
        Resolve = 8,
        IsAdmin = 9,
        Sync = 10,
        Export = 11,
        Resubmit = 12,
    }
    public enum ApplicationPages
    {
        ApplicationModule = 1,
        ApplicationPage = 4,
        Role = 5,
        Region = 6,
        SubRegion = 7,
        City = 8,
        Designation = 9,
        Distributor = 10,
        User = 11,
        AdminDashboard = 12,
        AccountDashboard = 13,
        DistributorDashboard = 14,
        Order = 15,
        Payment = 16,
        ProductMapping = 17,
        Product = 18,
        ApplicationAction = 19,
        ComplaintCategory = 20,
        ComplaintSubCategory = 21,
        Complaint = 22,
        Bank = 23,
        Company = 24,
        License = 25,
        DistributorLicense = 26,
        OrderReturn = 27,
        AddLicense = 28,
        OrderReport = 30,
        OrderReturnReport = 31,
        PaymentReport = 32,
        ComplaintReport = 33,
        Dashboard = 34,
        RolePermission = 35,
        AddOrder = 36,
        OrderView = 37,
        OrderApprove = 38,
        AddOrderReturn = 39,
        AddPayment = 40,
        ApprovePayment = 41,
        ViewOrderReturn = 42,
        ApproveOrderReturn = 43,
        AddComplaint = 44,
        GetFile = 45,
        ApproveComplaint = 46,
        ApproveTaxChallan = 61,
        TaxChallan = 60,
        PlantLocation = 69,
        OrderPlantWiseReport = 70,
    }
    public enum EmailType
    {
        CC = 1,
        KPI = 2
    }
    public enum DistributorTransactionStatus
    {
        Verified = 1,
        Rejected = 2,
        Canceled = 3,
        Approved = 4,
        InProcess = 5,
        PartiallyApproved = 6,
        Received = 7,
        PartiallyReceived = 8,
    }
    public enum TransactionStatus
    {
        PendingApproval = 9,
        Unverified = 10,
        Submitted = 11,
    }
}
