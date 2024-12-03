using MagicVilla_WebApp.Models;
using MagicVilla_WebApp.Models.Dtos;
using MagicVilla_WebApp.Services.IServices;

namespace MagicVilla_WebApp.Services
{
	public class AuthenticationService : ConsumeService,  IAuthService
	{
		public readonly IConfiguration _configuration;
		private readonly string _url;
        public AuthenticationService(IHttpClientFactory httpClientFactory, IConfiguration configuration) : base(httpClientFactory)
        {
            _configuration = configuration;
			_url = $"{_configuration.GetValue<string>("ServiceUrls:VillaAPI")}/api/{StaticDetails.Version}/Authentication";
		}
        public async Task<ApiResponse?> LogIn(LogInRequestDto dto)
		{
			return await SendAsync(new ApiRequest
			{
				Url = $"{_url}/LogIn",
				Data = dto,
				apiType = StaticDetails.ApiType.POST
			});
		}

		public async Task<ApiResponse?> Register(RegisterRequestDto dto)
		{
			return await SendAsync(new ApiRequest
			{
				Url = $"{_url}/Register",
				Data = dto,
				apiType = StaticDetails.ApiType.POST
			});
		}
	}
}
