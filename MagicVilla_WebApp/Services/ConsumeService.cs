using MagicVilla_WebApp.Models;
using MagicVilla_WebApp.Services.IServices;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;
using static MagicVilla_WebApp.StaticDetails;

namespace MagicVilla_WebApp.Services
{
    public class ConsumeService : IConsumeService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public ConsumeService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;

        }
        public ApiResponse Response { get; set; } = new ApiResponse();

        public async Task<ApiResponse?> SendAsync(ApiRequest apiRequest)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("VillaApi");
                HttpRequestMessage message = new HttpRequestMessage();
                if (apiRequest.ContentType == ContentType.MultipartFormData)
                {
                    message.Headers.Add("Accept", "*/*");
                    var content = new MultipartFormDataContent();
                    foreach (var prop in apiRequest.Data.GetType().GetProperties())
                    {
                        var value = prop.GetValue(apiRequest.Data);
                        if (value is FormFile)
                        {
                            var file = value as FormFile;
                            content.Add(new StreamContent(file.OpenReadStream()), prop.Name, file.FileName);
                        }
                        else
                        {
                            content.Add(new StringContent(value?.ToString() ?? string.Empty), prop.Name);
                        }
                    }
                    message.Content = content;
                }
                else
                {
                    message.Headers.Add("Accept", "application/json");
                    if (apiRequest.Data is not null)
                    {
                        message.Content = new StringContent(
                            JsonConvert.SerializeObject(apiRequest.Data), Encoding.UTF8, "application/json"
                        );
                    }
                }
                message.RequestUri = new Uri(apiRequest.Url);
                if (!string.IsNullOrWhiteSpace(apiRequest.Token))
                    message.Headers.Authorization = new AuthenticationHeaderValue("Bearer", apiRequest.Token);
                switch (apiRequest.apiType)
                {
                    case ApiType.POST:
                        message.Method = HttpMethod.Post;
                        break;
                    case ApiType.PUT:
                        message.Method = HttpMethod.Put;
                        break;
                    case ApiType.DELETE:
                        message.Method = HttpMethod.Delete;
                        break;
                    default:
                        message.Method = HttpMethod.Get;
                        break;
                }
                HttpResponseMessage responseMesssage = await client.SendAsync(message);
                var responseContent = await responseMesssage.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<ApiResponse>(responseContent);
            }
            catch (Exception ex)
            {
                return new ApiResponse()
                {
                    IsSuccess = false,
                    Errors = new List<string>() { ex.Message }
                };
            }
        }
    }
}
