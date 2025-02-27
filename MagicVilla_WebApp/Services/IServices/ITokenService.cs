using MagicVilla_WebApp.Models.Dtos;

namespace MagicVilla_WebApp.Services.IServices
{
	public interface ITokenService
	{
		void SetToken(TokenDto token);
		string GetAccessToken();
		string GetRefreshToken();
		void DeleteToken();
	}
}
