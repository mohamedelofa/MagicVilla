namespace MagicVilla_WebApp.Models.Dtos
{
    public class RegisterRequestDto
    {
        public string UserName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string Role { get; set; } = null!;
        public string Address { get; set; } = null!;
    }
}
