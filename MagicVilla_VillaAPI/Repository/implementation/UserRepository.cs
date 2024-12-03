
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MagicVilla_VillaAPI.Repository.implementation
{
	public class UserRepository(
		AppDbContext context ,
		IMapper mapper , 
		IConfiguration configuration ,
		UserManager<ApplicationUser> userManager,
		RoleManager<IdentityRole> roleManager) 
		: IUserRepository
	{
		private readonly AppDbContext _context = context;
		private readonly IMapper _mapper = mapper;
		private readonly IConfiguration _configuration = configuration;
		private readonly UserManager<ApplicationUser> _userManager = userManager;
		private readonly RoleManager<IdentityRole> _roleManager = roleManager;
		public async Task<bool> IsUnique(string userName,string email)
		{
			return !await (_context.Users.AnyAsync(u => u.UserName == userName || u.Email == email));
		}

		public async Task<LogInResponseDto?> LogIn(LogInRequestDto dto)
		{
			//ApplicationUser? user = await _context.Users.FirstOrDefaultAsync(u => u.UserName.ToLower() == dto.UserName.ToLower());
			ApplicationUser? user = await _userManager.FindByNameAsync(dto.UserName);
			if (user is null) return null;
			bool isValid = await _userManager.CheckPasswordAsync(user, dto.Password);
			if (!isValid) return null;
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
				Subject = await GenerateClaimsAsync(user)
			};
			var token = tokenHandler.CreateToken(tokenDescriptor);
			return new LogInResponseDto()
			{
				User = _mapper.Map<UserDto>(user),
				Token = tokenHandler.WriteToken(token)
			};
			
		}

		private async Task<ClaimsIdentity> GenerateClaimsAsync(ApplicationUser user)
		{
			var roles = await _userManager.GetRolesAsync(user);
			var claimsIdentity = new ClaimsIdentity();
			claimsIdentity.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()));
			claimsIdentity.AddClaim(new Claim(ClaimTypes.Name, user.UserName));
			//claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, user.Role));
			foreach(var role in roles)
			{
                claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, role));
            }
			return claimsIdentity;
		}

		public async Task<RegisterResponseDto?> Register(RegisterRequestDto dto)
		{
			using (var transaction = _context.Database.BeginTransaction())
			{
                ApplicationUser user = _mapper.Map<ApplicationUser>(dto);
                var result = await _userManager.CreateAsync(user, dto.Password);
                if (result.Succeeded)
                {
                    var roles = dto.Roles.Split(',');
					foreach (var role in roles)
					{
						if (!_roleManager.RoleExistsAsync(role).Result)
						{
							await _roleManager.CreateAsync(new IdentityRole(role));
						}
					}
                    if (_userManager.AddToRolesAsync(user, roles).Result.Succeeded)
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
	}
}
