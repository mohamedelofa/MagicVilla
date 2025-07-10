using MagicVilla_WebApp.Models;
using MagicVilla_WebApp.Models.Dtos;

namespace MagicVilla_WebApp.Services.IServices
{
	public interface IAuthService
	{
		Task<ApiResponse?> LogIn(LogInRequestDto dto);
		Task<ApiResponse?> Register(RegisterRequestDto dto);
		Task<ApiResponse?> LogOut();
		Task<ApiResponse?> ConfirmEmailAsync(string email, string token);
	}
}
