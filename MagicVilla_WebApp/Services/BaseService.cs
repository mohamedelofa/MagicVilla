using MagicVilla_WebApp.Models;
using MagicVilla_WebApp.Models.Dtos;
using MagicVilla_WebApp.Services.IServices;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.ComponentModel;
using System.Text;
using static MagicVilla_WebApp.StaticDetails;

namespace MagicVilla_WebApp.Services
{
    public class BaseService : ConsumeService , IBaseService
    {
        private readonly IConfiguration _configuration;
		protected readonly string _url;
		protected readonly string _version = StaticDetails.Version;
		public BaseService(IHttpClientFactory httpClientFactory , IConfiguration configuration) : base(httpClientFactory)
        {
            _configuration = configuration;
            _url = _configuration.GetValue<string>("ServiceUrls:VillaAPI");
		}



		public async Task<ApiResponse?> CreateAsync<T>(T dto , string endPoint, string token)
		{
			return await SendAsync(new ApiRequest()
			{
				apiType = ApiType.POST,
				Data = dto,
				Url = $"{_url}/api/{_version}/{endPoint}",
				Token = token
			});

		}

		public async Task<ApiResponse?> DeleteAsync(int id , string endPoint, string token)
		{
			return await SendAsync(new ApiRequest()
			{
				apiType = ApiType.DELETE,
				Url = $"{_url}/api/{_version}/{endPoint}/{id}",
                Token = token
            });
		}

		public async Task<ApiResponse?> GetAllAsync(string endPoint, string token)
		{
			return await SendAsync(new ApiRequest()
			{
				apiType = ApiType.GET,
				Url = $"{_url}/api/{_version}/{endPoint}",
                Token = token
            });
		}

		public async Task<ApiResponse?> GetAsync(int id , string endPoint, string token)
		{
			return await SendAsync(new ApiRequest()
			{
				apiType = ApiType.GET,
				Url = $"{_url}/api/{_version}/{endPoint}/{id}",
                Token = token
            });
		}

		public async Task<ApiResponse?> UpdateAsync<T>(T dto, int id , string endPoint, string token)
		{
			return await SendAsync(new ApiRequest()
			{
				apiType = ApiType.PUT,
				Data = dto,
				Url = $"{_url}/api/{_version}/{endPoint}/{id}",
                Token = token
            });
		}
	}
}
