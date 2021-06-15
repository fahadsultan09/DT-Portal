using System.Threading.Tasks;

namespace UserLoginRegistration.Model
{
    public class Response
    {
        public JsonResponse Data { get; set; }
    }
    public class JsonResponse
    {
        public string Message { get; set; }
        public bool Status { get; set; }
        public string RedirectURL { get; set; }
        public Task<string> HtmlString { get; set; }
    }
}
