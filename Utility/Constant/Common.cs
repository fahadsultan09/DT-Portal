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
        }

        public class OrderContant
        {
            public const string OrderDraft = "Successfully save to draft";
            public const string OrderSubmit = "Order Submitted Successfully";
            public const string OrderItem = "Please add atleast one product";
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
