namespace MagicVilla_WebApp.Models.Dtos
{
	public class LogInResponseDto
	{
		public string AccessToken { get; set; } = null!;
		public string RefreshToken { get; set; } = null!;
	}
}
