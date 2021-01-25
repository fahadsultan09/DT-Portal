﻿using System.ComponentModel.DataAnnotations;

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
        [Display(Name = "Pending Approval")]
        PendingApproval = 2,
        [Display(Name = "Payment Verified")]
        PaymentVerified = 3,
        [Display(Name = "In Process")]
        InProcess = 4,
        [Display(Name = "Partially Processed")]
        PartiallyProcessed = 5,
        [Display(Name = "Completely Processed")]
        CompletelyProcessed = 6,
        Reject = 7,
        Submit = 8,
        [Display(Name = "On hold")]
        Onhold = 9,
        Approved = 10,
        [Display(Name = "Not Yet Process")]
        NotYetProcess = 11

    }
    public enum PaymentStatus
    {
        Verified = 1,
        Unverified = 2,
        Reject = 3
    }
    public enum ProductVisibility
    {
        Visible = 1,
        Hide = 2
    }

    public enum CompanyEnum
    {
        SAMI = 1,
        Phytek = 2,
        Healthtek = 3
    }

    public enum SubmitStatus
    {
        OrderNow = 1,
        Draft = 2
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
        Resolved = 2
    }

    public enum LicenseStatus
    {
        Submitted = 1,
        Verified = 2,
        Rejected = 3,
        Apply = 0
    }

    public enum LicenseType
    {
        License = 1,
        Challan = 2
    }
    public enum OrderReturnStatus
    {
        Draft = 1,
        Submitted = 2,
        Received = 3
    }
}
