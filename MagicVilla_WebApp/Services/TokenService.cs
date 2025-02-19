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
		}

		public string GetToken()
		{
			string? token = null;
			bool flag = _contextAccessor.HttpContext?.Request.Cookies.TryGetValue(StaticDetails.AccessToken, out token) ?? false;
			if (flag)
				return token ?? string.Empty;
			return string.Empty;

		}

		public void SetToken(string accessToken)
		{
			var cookieOptions = new CookieOptions() { Expires = DateTime.Now.AddDays(1) };
			_contextAccessor.HttpContext?.Response.Cookies.Append(StaticDetails.AccessToken, accessToken, cookieOptions);
		}
	}
}
