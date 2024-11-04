using MagicVilla_WebApp.Models;
using MagicVilla_WebApp.Models.Dtos;
using MagicVilla_WebApp.Services.IServices;
using Newtonsoft.Json;
using System.ComponentModel;
using System.Text;
using static MagicVilla_WebApp.StaticDetails;

namespace MagicVilla_WebApp.Services
{
    public class BaseService(IHttpClientFactory httpClientFactory , IConfiguration configuration) : IBaseService
    {
        public ApiResponse Response { get; set; } = new();
        protected readonly IHttpClientFactory _httpClientFactory = httpClientFactory;
        protected readonly string _url = configuration.GetValue<string>("ServiceUrls:VillaAPI");

        public async Task<ApiResponse?> SendAsync(ApiRequest apiRequest)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("VillaApi");
                HttpRequestMessage message = new HttpRequestMessage();
                message.Headers.Add("Accept", "application/json");
                message.RequestUri = new Uri(apiRequest.Url);
                if(apiRequest.Data is not null)
                {
                    message.Content = new StringContent(JsonConvert.SerializeObject(apiRequest.Data),Encoding.UTF8, "application/json");
                }
                switch(apiRequest.apiType)
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




		public async Task<ApiResponse?> CreateAsync<T>(T dto , string endPoint)
		{
			return await SendAsync(new ApiRequest()
			{
				apiType = ApiType.POST,
				Data = dto,
				Url = $"{_url}/api/{endPoint}"
			});

		}

		public async Task<ApiResponse?> DeleteAsync(int id , string endPoint)
		{
			return await SendAsync(new ApiRequest()
			{
				apiType = ApiType.DELETE,
				Url = $"{_url}/api/{endPoint}/{id}"
			});
		}

		public async Task<ApiResponse?> GetAllAsync(string endPoint)
		{
			return await SendAsync(new ApiRequest()
			{
				apiType = ApiType.GET,
				Url = $"{_url}/api/{endPoint}"
			});
		}

		public async Task<ApiResponse?> GetAsync(int id , string endPoint)
		{
			return await SendAsync(new ApiRequest()
			{
				apiType = ApiType.GET,
				Url = $"{_url}/api/{endPoint}/{id}"
			});
		}

		public async Task<ApiResponse?> UpdateAsync<T>(T dto, int id , string endPoint)
		{
			return await SendAsync(new ApiRequest()
			{
				apiType = ApiType.PUT,
				Data = dto,
				Url = $"{_url}/api/{endPoint}/{id}"
			});
		}
	}
}
