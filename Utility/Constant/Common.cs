namespace Utility.Constant
{
    public class Common
    {
        public class ControllerAction
        {
            public const string Home = "Home";
            public const string Index = "Index";
        }

        public static string[] permittedExtensions = new string[] { ".docx", ".doc", ".pdf", ".jpg", ".png", ".jpeg" };
        public class FolderName
        {
            public const string Order = "Order";
            public const string OrderUpload = "OrderUpload";
            public const string OrderReturn = "OrderReturn";
            public const string Payment = "Payment";
            public const string Complaint = "Complaint";
            public const string TaxChallan = "TaxChallan";
            public const string DistributorLicense = "DistributorLicense";
            public const string OrderStatus = "OrderStatus";
            public const string PendingQuantity = "PendingQuantity";
            public const string PendingValue = "PendingValue";
        }

        public class OrderContant
        {
            public const string OrderDraft = "Successfully saved to draft";
            public const string OrderSubmit = "Order submitted successfully";
            public const string OrderReturnSubmit = "Order return submitted successfully";
            public const string OrderItem = "Please add at least one product";
        }

        public class AcceptURLs
        {
            public const string Home = "/Home/Index";
        }
    }
    public class LastYearMonth
    {
        public string MonthName { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public int LastYear { get; set; }
    }
}
