using MagicVilla_WebApp.Exceptions;
using MagicVilla_WebApp.Models;
using MagicVilla_WebApp.Models.Dtos;
using MagicVilla_WebApp.Services.IServices;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using static MagicVilla_WebApp.StaticDetails;

namespace MagicVilla_WebApp.Services
{
	public class ConsumeService : IConsumeService
	{
		private readonly IHttpClientFactory _httpClientFactory;
		private readonly IConfiguration _configuration;
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly ITokenService _tokenService;
		private string _accessToken = string.Empty;
		private string _refreshToken = string.Empty;
		public ConsumeService(IHttpClientFactory httpClientFactory,
			ITokenService tokenService,
			IConfiguration configuration,
			IHttpContextAccessor httpContextAccessor)
		{
			_httpClientFactory = httpClientFactory;
			_configuration = configuration;
			_tokenService = tokenService;
			_accessToken = _tokenService.GetAccessToken();
			_refreshToken = _tokenService.GetRefreshToken();
			_httpContextAccessor = httpContextAccessor;
		}
		public ApiResponse Response { get; set; } = new ApiResponse();

		public async Task<ApiResponse?> SendAsync(ApiRequest apiRequest, bool needToken = true)
		{
			try
			{
				var client = _httpClientFactory.CreateClient("VillaApi");
				HttpResponseMessage responseMesssage = await client.SendAsync(MessageFactory(apiRequest, needToken));
				if (!responseMesssage?.IsSuccessStatusCode == true)
				{
					if (responseMesssage?.StatusCode == System.Net.HttpStatusCode.Unauthorized && !string.IsNullOrEmpty(_accessToken))
					{
						// access token expired
						// get new token and resend request
						await InvokeRefreshTokenEndPoint(client, _accessToken, _refreshToken);
						// resend request
						responseMesssage = await client.SendAsync(MessageFactory(apiRequest, needToken));
					}
				}
				var responseContent = await responseMesssage.Content.ReadAsStringAsync();
				var response = JsonConvert.DeserializeObject<ApiResponse>(responseContent);
				return response;
			}
			catch (TokenException)
			{
				throw;
			}
			catch (Exception ex)
			{
				return new ApiResponse()
				{
					IsSuccess = false,
					Errors = new List<string>() { ex.Message }
				};
			}
		}

		private HttpRequestMessage MessageFactory(ApiRequest apiRequest, bool needToken)
		{
			HttpRequestMessage message = new HttpRequestMessage();
			if (apiRequest.Data is not null)
			{
				if (apiRequest.ContentType == ContentType.MultipartFormData)
				{
					message.Headers.Add("Accept", "*/*");
					var content = new MultipartFormDataContent();
					foreach (var prop in apiRequest.Data.GetType().GetProperties())
					{
						var value = prop.GetValue(apiRequest.Data);
						if (value is FormFile)
						{
							var file = value as FormFile;
							content.Add(new StreamContent(file.OpenReadStream()), prop.Name, file.FileName);
						}
						else
						{
							content.Add(new StringContent(value?.ToString() ?? string.Empty), prop.Name);
						}
					}
					message.Content = content;
				}
				else
				{
					message.Headers.Add("Accept", "application/json");
					message.Content = new StringContent(
						JsonConvert.SerializeObject(apiRequest.Data), Encoding.UTF8, "application/json"
					);
				}
			}
			message.RequestUri = new Uri(apiRequest.Url);
			if (needToken && !string.IsNullOrEmpty(_accessToken))
				message.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);
			switch (apiRequest.apiType)
			{
				case ApiType.POST:
					message.Method = HttpMethod.Post;
					break;
				case ApiType.PUT:
					message.Method = HttpMethod.Put;
					break;
				case ApiType.DELETE:
					message.Method = HttpMethod.Delete;
					break;
				default:
					message.Method = HttpMethod.Get;
					break;
			}
			return message;
		}

		private async Task InvokeRefreshTokenEndPoint(HttpClient client, string accessToken, string refreshToken)
		{
			try
			{
				var message = new HttpRequestMessage();
				message.Headers.Add("Accept", "application/json");
				message.Content = new StringContent(
						JsonConvert.SerializeObject(new TokenDto()
						{
							Accesstoken = _accessToken,
							RefreshToken = _refreshToken
						}), Encoding.UTF8, "application/json"
				);
				message.Method = HttpMethod.Post;
				message.RequestUri = new Uri($"{_configuration.GetValue<string>("ServiceUrls:VillaAPI")}/api/{StaticDetails.Version}/Authentication/RefreshToken");
				var apiResponse = await client.SendAsync(message);
				var apiContent = await apiResponse?.Content.ReadAsStringAsync();
				var response = JsonConvert.DeserializeObject<ApiResponse>(apiContent);
				if (!response?.IsSuccess == true)
				{
					await _httpContextAccessor.HttpContext.SignOutAsync();
					_tokenService.DeleteToken();
					throw new TokenException();
				}
				else
				{
					var token = JsonConvert.DeserializeObject<TokenDto>(response.Result.ToString());
					await SignInWithNewToken(token);
				}
			}
			catch (Exception ex)
			{
				throw;
			}
		}

		private async Task SignInWithNewToken(TokenDto? token)
		{
			if (token is not null && !string.IsNullOrEmpty(token.Accesstoken))
			{
				var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
				var handler = new JwtSecurityTokenHandler();
				var jwt = handler.ReadJwtToken(token.Accesstoken);
				identity.AddClaim(
					new Claim(
						ClaimTypes.Name,
						jwt?.Claims?
						.FirstOrDefault(c => c.Type == "unique_name")?.Value ?? string.Empty));
				foreach (var role in jwt?.Claims?.Where(c => c.Type == "role")?.Select(c => c.Value)?.ToList())
				{
					identity.AddClaim(new Claim(ClaimTypes.Role, role));
				}
				var principal = new ClaimsPrincipal(identity);
				await _httpContextAccessor.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
				_tokenService.SetToken(token);
				_accessToken = token.Accesstoken;
				_refreshToken = token.RefreshToken;
			}
		}
	}
}

