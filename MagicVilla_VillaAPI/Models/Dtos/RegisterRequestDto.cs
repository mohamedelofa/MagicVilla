namespace MagicVilla_VillaAPI.Models.Dtos
{
	public class RegisterRequestDto
	{
		public string UserName { get; set; } = null!;
		public string Email { get; set; } = null!;
		public string Password { get; set; } = null!;
		public string Roles { get; set; } = null!;
		public string Address { get; set; } = null!;
	}
}
