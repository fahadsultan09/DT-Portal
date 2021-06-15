using System.Threading.Tasks;

namespace Models.ViewModel
{
    public class JsonResponse
    {
        public string Message { get; set; }
        public bool Status { get; set; }
        public string RedirectURL { get; set; }
        public Task<string> HtmlString { get; set; }
        public SignalRResponse SignalRResponse { get; set; }
    }
}
