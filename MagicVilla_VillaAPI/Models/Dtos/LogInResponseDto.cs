namespace MagicVilla_VillaAPI.Models.Dtos
{
	public class LogInResponseDto
	{
		public UserDto? User { get; set; }
		public string Token { get; set; } = null!;
	}
}
