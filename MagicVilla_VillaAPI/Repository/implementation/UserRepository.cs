
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace MagicVilla_VillaAPI.Repository.implementation
{
	public class UserRepository(
		AppDbContext context,
		IMapper mapper,
		IConfiguration configuration,
		UserManager<ApplicationUser> userManager,
		RoleManager<IdentityRole> roleManager)
		: IUserRepository
	{
		private readonly AppDbContext _context = context;
		private readonly IMapper _mapper = mapper;
		private readonly IConfiguration _configuration = configuration;
		private readonly UserManager<ApplicationUser> _userManager = userManager;
		private readonly RoleManager<IdentityRole> _roleManager = roleManager;
		public async Task<bool> IsUnique(string userName, string email)
		{
			return !await (_context.Users.AnyAsync(u => u.UserName == userName || u.Email == email));
		}

		public async Task<LogInResponseDto?> LogIn(LogInRequestDto dto)
		{
			ApplicationUser? user = await _userManager.FindByNameAsync(dto.UserName);
			if (user is null) return null;
			bool isValid = await _userManager.CheckPasswordAsync(user, dto.Password);
			if (!isValid) return null;
			var jwtTokenId = $"JTI{Guid.NewGuid().ToString()}";
			// Generate JWT Access Token
			var accessToken = await GetAccessTokenAsync(user, jwtTokenId);
			// Generate Refresh Token
			var refreshToken = await GenerateRefreshTokenAsync(user.Id, jwtTokenId);
			return new LogInResponseDto()
			{
				AccessToken = accessToken,
				RefreshToken = refreshToken
			};

		}

		public async Task<RegisterResponseDto?> Register(RegisterRequestDto dto)
		{
			using (var transaction = _context.Database.BeginTransaction())
			{
				ApplicationUser user = _mapper.Map<ApplicationUser>(dto);
				var result = await _userManager.CreateAsync(user, dto.Password);
				if (result.Succeeded)
				{
					if (!_roleManager.RoleExistsAsync(dto.Role).Result)
					{
						await _roleManager.CreateAsync(new IdentityRole(dto.Role));
					}
					if (_userManager.AddToRoleAsync(user, dto.Role).Result.Succeeded)
					{
						transaction.Commit();
						return new RegisterResponseDto()
						{
							User = _mapper.Map<UserDto>(user)
						};
					}
					else
					{
						transaction.Rollback();
					}
				}
				return new RegisterResponseDto
				{
					User = null,
					Errors = string.Join(",", result.Errors.ToList().Select(x => x.Description))
				};
			}
		}

		public async Task<TokenDto?> RefreshTokenAsync(TokenDto tokenDto)
		{
			var existingRefreshToken = _context.RefreshTokens.FirstOrDefault(x => x.Token == tokenDto.RefreshToken);
			if (existingRefreshToken is null) return null;
			if (!existingRefreshToken.IsValid)
			{
				await MarkAllTokenInChainAsInvalid(existingRefreshToken.JwtTokenId);
				return null;
			}
			if (!await IsValidAccessToken(existingRefreshToken, tokenDto.AccessToken) || existingRefreshToken.IsExpired)
			{
				await MarkTokenAsInvalid(existingRefreshToken);
				return null;
			}
			await MarkTokenAsInvalid(existingRefreshToken);
			var newRefreshToken = await GenerateRefreshTokenAsync(existingRefreshToken.UserId, existingRefreshToken.JwtTokenId);
			var user = await _userManager.FindByIdAsync(existingRefreshToken.UserId);
			if (user is null) return null;
			var newAccessToken = await GetAccessTokenAsync(user, existingRefreshToken.JwtTokenId);
			return new TokenDto()
			{
				AccessToken = newAccessToken,
				RefreshToken = newRefreshToken
			};

		}

		public async Task<bool> RevokeToken(string refreshToken)
		{
			var existingRefreshToken = _context.RefreshTokens.FirstOrDefault(x => x.Token == refreshToken);
			if (existingRefreshToken is null) return false;
			await MarkTokenAsInvalid(existingRefreshToken);
			return true;
		}

		private async Task<string> GetAccessTokenAsync(ApplicationUser user, string jwtTokenId)
		{
			var tokenHandler = new JwtSecurityTokenHandler();
			var credentials = new SigningCredentials(
				new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_configuration.GetValue<string>("ApiSettings:Secret"))),
				SecurityAlgorithms.HmacSha256
				);
			var tokenDescriptor = new SecurityTokenDescriptor()
			{
				Expires = DateTime.Now.AddMinutes(60),
				SigningCredentials = credentials,
				Subject = await GenerateClaimsAsync(user, jwtTokenId)
			};
			var token = tokenHandler.CreateToken(tokenDescriptor);
			return tokenHandler.WriteToken(token);
		}

		private async Task<ClaimsIdentity> GenerateClaimsAsync(ApplicationUser user, string jwtTokenId)
		{
			var roles = await _userManager.GetRolesAsync(user);
			var claimsIdentity = new ClaimsIdentity();
			claimsIdentity.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()));
			claimsIdentity.AddClaim(new Claim(ClaimTypes.Name, user?.UserName ?? string.Empty));
			claimsIdentity.AddClaim(new Claim(JwtRegisteredClaimNames.Jti, jwtTokenId));
			//claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, user.Role));
			foreach (var role in roles)
			{
				claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, role));
			}
			return claimsIdentity;
		}

		private async Task<string> GenerateRefreshTokenAsync(string userId, string jwtTokenId)
		{
			var refreshToken = new RefreshToken()
			{
				Token = await GetRefreshTokenAsync(),
				ExpireOn = DateTime.UtcNow.AddDays(15),
				JwtTokenId = jwtTokenId,
				UserId = userId
			};
			_context.Add(refreshToken);
			await _context.SaveChangesAsync();
			return refreshToken.Token;
		}

		private async Task<string> GetRefreshTokenAsync()
		{
			var randomNumber = new byte[32];
			using (var rng = RandomNumberGenerator.Create())
			{
				rng.GetBytes(randomNumber);
				return Convert.ToBase64String(randomNumber);
			}
		}
		private async Task<bool> IsValidAccessToken(RefreshToken refreshToken, string accessToken)
		{
			var accessTokenData = ReadAccessToken(accessToken);
			return accessTokenData.isSuccess && refreshToken.UserId == accessTokenData.userId && refreshToken.JwtTokenId == accessTokenData.jwtTokenId;
		}
		private (bool isSuccess, string userId, string jwtTokenId) ReadAccessToken(string accessToken)
		{
			JwtSecurityTokenHandler jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
			jwtSecurityTokenHandler.ValidateToken(accessToken,
				new TokenValidationParameters()
				{
					ValidateIssuerSigningKey = true,
					IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_configuration.GetValue<string>("ApiSettings:Secret"))),
					ValidateIssuer = false,
					ValidateAudience = false
				},
				out SecurityToken validatedToken
			);
			var jwtToken = validatedToken as JwtSecurityToken;
			if (jwtToken is null) return (false, string.Empty, string.Empty);
			var userId = jwtToken?.Claims?.FirstOrDefault(c => c.Type == "nameid")?.Value;
			var jwtTokenId = jwtToken?.Claims?.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti)?.Value;
			return (true, userId ?? string.Empty, jwtTokenId ?? string.Empty);
		}

		private async Task MarkAllTokenInChainAsInvalid(string jwtTokenId)
		{
			await _context.RefreshTokens.Where(r => r.JwtTokenId == jwtTokenId)
					.ExecuteUpdateAsync(p => p.SetProperty(r => r.IsValid, false));
		}
		private async Task MarkTokenAsInvalid(RefreshToken refreshToken)
		{
			refreshToken.IsValid = false;
			_context.Update(refreshToken);
			await _context.SaveChangesAsync();
		}

	}
}
