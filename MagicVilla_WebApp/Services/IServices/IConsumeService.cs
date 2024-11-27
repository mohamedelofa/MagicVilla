using MagicVilla_WebApp.Models;

namespace MagicVilla_WebApp.Services.IServices
{
	public interface IConsumeService
	{
		ApiResponse Response { get; set; }
		Task<ApiResponse?> SendAsync(ApiRequest apiRequest);
	}
}
