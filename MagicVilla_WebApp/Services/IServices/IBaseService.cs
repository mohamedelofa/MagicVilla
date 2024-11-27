using MagicVilla_WebApp.Models;
using MagicVilla_WebApp.Models.Dtos;

namespace MagicVilla_WebApp.Services.IServices
{
    public interface IBaseService : IConsumeService
    {
		Task<ApiResponse?> GetAllAsync(string endPoint , string token);
		Task<ApiResponse?> GetAsync(int id , string endPoint, string token);
		Task<ApiResponse?> CreateAsync<T>(T dto , string endPoint, string token);
		Task<ApiResponse?> UpdateAsync<T>(T dto, int id , string endPoint, string token);
		Task<ApiResponse?> DeleteAsync(int id , string endPoint, string token);
	}
}
