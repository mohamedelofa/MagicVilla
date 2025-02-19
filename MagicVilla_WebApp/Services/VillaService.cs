using MagicVilla_WebApp.Models;
using MagicVilla_WebApp.Services.IServices;
using static MagicVilla_WebApp.StaticDetails;

namespace MagicVilla_WebApp.Services
{
	public class VillaService : BaseService, IVillaService
	{
		public VillaService(IConfiguration configuration, IConsumeService consumeService)
			: base(configuration, consumeService)
		{

		}
		public override async Task<ApiResponse?> CreateAsync<T>(T dto, string endPoint)
		{
			return await _consumeService.SendAsync(new ApiRequest()
			{
				apiType = ApiType.POST,
				Data = dto,
				Url = $"{_url}/api/{_version}/{endPoint}",
				ContentType = ContentType.MultipartFormData
			});
		}

		public override async Task<ApiResponse?> UpdateAsync<T>(T dto, int id, string endPoint)
		{
			return await _consumeService.SendAsync(new ApiRequest()
			{
				apiType = ApiType.PUT,
				Data = dto,
				Url = $"{_url}/api/{_version}/{endPoint}/{id}",
				ContentType = ContentType.MultipartFormData
			});
		}
	}
}
