
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MagicVilla_VillaAPI.Repository.implementation
{
	public class UserRepository(AppDbContext context , IMapper mapper , IConfiguration configuration) : IUserRepository
	{
		private readonly AppDbContext _context = context;
		private readonly IMapper _mapper = mapper;
		private readonly IConfiguration _configuration = configuration;
		public async Task<bool> IsUnique(string userName)
		{
			return !await (_context.Users.AnyAsync(u => u.UserName == userName));
		}

		public async Task<LogInResponseDto?> LogIn(LogInRequestDto dto)
		{
			LocalUser? user = await _context.Users.FirstOrDefaultAsync(u => u.UserName.ToLower() == dto.UserName.ToLower() && u.Password == dto.Password);
			if (user is null) return null;

			// Generate JWT Token
			var tokenHandler = new JwtSecurityTokenHandler();
			var credentials = new SigningCredentials(
				new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_configuration.GetValue<string>("ApiSettings:Secret"))),
				SecurityAlgorithms.HmacSha256
				);
			var tokenDescriptor = new SecurityTokenDescriptor()
			{
				Expires = DateTime.Now.AddDays(1),
				SigningCredentials = credentials,
				Subject = GenerateClaims(user)
			};
			var token = tokenHandler.CreateToken(tokenDescriptor);
			return new LogInResponseDto()
			{
				User = user,
				Token = tokenHandler.WriteToken(token)
			};
			
		}

		private ClaimsIdentity GenerateClaims(LocalUser user)
		{
			var claimsIdentity = new ClaimsIdentity();
			claimsIdentity.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()));
			claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, user.Role));
			return claimsIdentity;
		}

		public async Task<LocalUser?> Register(RegisterRequestDto dto)
		{
			LocalUser user = _mapper.Map<LocalUser>(dto);
			await _context.Users.AddAsync(user);
			await _context.SaveChangesAsync();
			user.Password = string.Empty;
			return user;
		}
	}
}
