namespace MagicVilla_VillaAPI.Models.Dtos
{
    public class CreateVillaDto : BaseVillaDto
    {
        [Required]
        public IFormFile Image { get; set; } = null!;
    }
}
