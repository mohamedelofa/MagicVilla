using MagicVilla_WebApp.Models;
using MagicVilla_WebApp.Models.Dtos;
using MagicVilla_WebApp.Services.IServices;

namespace MagicVilla_WebApp.Services
{
	public class AuthenticationService : IAuthService
	{
		private readonly IConfiguration _configuration;
		private readonly IConsumeService _consumeService;
		private readonly ITokenService _tokenService;
		private readonly string _url;
		public AuthenticationService(IConfiguration configuration, IConsumeService consumeService, ITokenService tokenService)
		{
			_consumeService = consumeService;
			_configuration = configuration;
			_tokenService = tokenService;
			_url = $"{_configuration.GetValue<string>("ServiceUrls:VillaAPI")}/api/{StaticDetails.Version}/Authentication";
		}
		public async Task<ApiResponse?> LogIn(LogInRequestDto dto)
		{
			return await _consumeService.SendAsync(new ApiRequest
			{
				Url = $"{_url}/LogIn",
				Data = dto,
				apiType = StaticDetails.ApiType.POST
			}, needToken: false);
		}

		public async Task<ApiResponse?> LogOut()
		{
			return await _consumeService.SendAsync(new ApiRequest
			{
				Url = $"{_url}/RevokeToken?refreshToken={_tokenService.GetRefreshToken()}",
				apiType = StaticDetails.ApiType.POST
			});
		}

		public async Task<ApiResponse?> Register(RegisterRequestDto dto)
		{
			return await _consumeService.SendAsync(new ApiRequest
			{
				Url = $"{_url}/Register",
				Data = dto,
				apiType = StaticDetails.ApiType.POST
			}, needToken: false);
		}
	}
}
