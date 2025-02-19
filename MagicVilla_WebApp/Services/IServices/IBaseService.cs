using MagicVilla_WebApp.Models;

namespace MagicVilla_WebApp.Services.IServices
{
	public interface IBaseService
	{
		Task<ApiResponse?> GetAllAsync(string endPoint);
		Task<ApiResponse?> GetAsync(int id, string endPoint);
		Task<ApiResponse?> CreateAsync<T>(T dto, string endPoint);
		Task<ApiResponse?> UpdateAsync<T>(T dto, int id, string endPoint);
		Task<ApiResponse?> DeleteAsync(int id, string endPoint);
	}
}
