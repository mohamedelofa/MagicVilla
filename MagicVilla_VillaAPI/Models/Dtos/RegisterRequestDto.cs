namespace MagicVilla_VillaAPI.Models.Dtos
{
	public class RegisterRequestDto
	{
		public string UserName { get; set; } = null!;
		public string Name { get; set; } = null!;
		public string Password { get; set; } = null!;
		public string Role { get; set; } = null!;
	}
}
