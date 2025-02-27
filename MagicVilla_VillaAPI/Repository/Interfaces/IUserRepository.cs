namespace MagicVilla_VillaAPI.Repository.Interfaces
{
	public interface IUserRepository
	{
		Task<bool> IsUnique(string userName, string email);
		Task<LogInResponseDto?> LogIn(LogInRequestDto dto);
		Task<RegisterResponseDto?> Register(RegisterRequestDto dto);
		Task<TokenDto> RefreshTokenAsync(TokenDto tokenDto);
		Task<bool> RevokeToken(string refreshToken);
	}
}
