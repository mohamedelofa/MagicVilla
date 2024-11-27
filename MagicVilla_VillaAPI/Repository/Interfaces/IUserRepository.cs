namespace MagicVilla_VillaAPI.Repository.Interfaces
{
	public interface IUserRepository
	{
		Task<bool> IsUnique(string userName);
		Task<LogInResponseDto?> LogIn(LogInRequestDto dto);
		Task<LocalUser?> Register(RegisterRequestDto dto);
	}
}
