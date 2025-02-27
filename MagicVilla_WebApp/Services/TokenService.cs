using MagicVilla_WebApp.Models.Dtos;
using MagicVilla_WebApp.Services.IServices;

namespace MagicVilla_WebApp.Services
{
	public class TokenService : ITokenService
	{
		private readonly IHttpContextAccessor _contextAccessor;
		public TokenService(IHttpContextAccessor contextAccessor)
		{
			_contextAccessor = contextAccessor;
		}
		public void DeleteToken()
		{
			_contextAccessor.HttpContext?.Response.Cookies.Delete(StaticDetails.AccessToken);
			_contextAccessor.HttpContext?.Response.Cookies.Delete(StaticDetails.RefreshToken);
		}

		public string GetAccessToken()
		{
			string? token = null;
			bool flag = _contextAccessor.HttpContext?.Request.Cookies.TryGetValue(StaticDetails.AccessToken, out token) ?? false;
			if (flag)
				return token ?? string.Empty;
			return string.Empty;

		}

		public string GetRefreshToken()
		{
			string? token = null;
			bool flag = _contextAccessor.HttpContext?.Request.Cookies.TryGetValue(StaticDetails.RefreshToken, out token) ?? false;
			if (flag)
				return token ?? string.Empty;
			return string.Empty;
		}

		public void SetToken(TokenDto token)
		{
			_contextAccessor.HttpContext?.Response.Cookies.Append(StaticDetails.AccessToken, token.Accesstoken);
			_contextAccessor.HttpContext?.Response.Cookies.Append(StaticDetails.RefreshToken, token.RefreshToken);
		}
	}
}
