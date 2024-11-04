using MagicVilla_WebApp.Models;
using MagicVilla_WebApp.Models.Dtos;

namespace MagicVilla_WebApp.Services.IServices
{
    public interface IBaseService
    {
        ApiResponse Response { get; set; }
        Task<ApiResponse?> SendAsync(ApiRequest apiRequest);
		Task<ApiResponse?> GetAllAsync(string endPoint);
		Task<ApiResponse?> GetAsync(int id , string endPoint);
		Task<ApiResponse?> CreateAsync<T>(T dto , string endPoint);
		Task<ApiResponse?> UpdateAsync<T>(T dto, int id , string endPoint);
		Task<ApiResponse?> DeleteAsync(int id , string endPoint);
	}
}
