namespace Utility.HelperClasses
{
    public class AjaxResponse
    {
        public bool Success { get; set; }
        public string Heading { get; set; }
        public string Message { get; set; }
        public string Icon { get; set; }
        public string RedirectToUrl { get; set; }
    }
    public class JsonResponseFactory
    {
        public static object ErrorResponse(string error)
        {
            return new { Success = false, Message = error };
        }

        public static object SuccessResponse(string url)
        {
            return new { Success = true, RedirectToUrl = url };
        }

        public static object SuccessResponse(object referenceObject, string url)
        {
            return new { Success = true, Message = referenceObject, RedirectToUrl = url };
        }
    }
}
