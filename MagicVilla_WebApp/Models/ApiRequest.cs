using static MagicVilla_WebApp.StaticDetails;

namespace MagicVilla_WebApp.Models
{
    public class ApiRequest
    {
        public ApiType apiType = ApiType.GET;
        public string Url { get; set; }
        public object Data { get; set; }
        public string Token { get; set; }
    }
}
