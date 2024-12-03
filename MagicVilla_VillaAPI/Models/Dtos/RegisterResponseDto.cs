namespace MagicVilla_VillaAPI.Models.Dtos
{
    public class RegisterResponseDto
    {
        public UserDto? User { get; set; }
        public string Errors { get; set; } = string.Empty;
    }
}
