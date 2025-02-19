using MagicVilla_WebApp.Models;
using MagicVilla_WebApp.Services.IServices;
using static MagicVilla_WebApp.StaticDetails;

namespace MagicVilla_WebApp.Services
{
	public class BaseService : IBaseService
	{
		private readonly IConfiguration _configuration;
		protected readonly IConsumeService _consumeService;
		protected readonly string _url;
		protected readonly string _version = StaticDetails.Version;
		public BaseService(IConfiguration configuration,
			IConsumeService consumeService)
		{
			_consumeService = consumeService;
			_configuration = configuration;
			_url = _configuration.GetValue<string>("ServiceUrls:VillaAPI");
		}



		public virtual async Task<ApiResponse?> CreateAsync<T>(T dto, string endPoint)
		{
			return await _consumeService.SendAsync(new ApiRequest()
			{
				apiType = ApiType.POST,
				Data = dto,
				Url = $"{_url}/api/{_version}/{endPoint}",
			});

		}

		public async Task<ApiResponse?> DeleteAsync(int id, string endPoint)
		{
			return await _consumeService.SendAsync(new ApiRequest()
			{
				apiType = ApiType.DELETE,
				Url = $"{_url}/api/{_version}/{endPoint}/{id}"
			});
		}

		public async Task<ApiResponse?> GetAllAsync(string endPoint)
		{
			return await _consumeService.SendAsync(new ApiRequest()
			{
				apiType = ApiType.GET,
				Url = $"{_url}/api/{_version}/{endPoint}",
			});
		}

		public async Task<ApiResponse?> GetAsync(int id, string endPoint)
		{
			return await _consumeService.SendAsync(new ApiRequest()
			{
				apiType = ApiType.GET,
				Url = $"{_url}/api/{_version}/{endPoint}/{id}",
			});
		}

		public virtual async Task<ApiResponse?> UpdateAsync<T>(T dto, int id, string endPoint)
		{
			return await _consumeService.SendAsync(new ApiRequest()
			{
				apiType = ApiType.PUT,
				Data = dto,
				Url = $"{_url}/api/{_version}/{endPoint}/{id}",
			});
		}
	}
}
