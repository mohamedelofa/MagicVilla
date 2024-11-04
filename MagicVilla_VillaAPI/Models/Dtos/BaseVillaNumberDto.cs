namespace MagicVilla_VillaAPI.Models.Dtos
{
    public class BaseVillaNumberDto
    {
        [Required]
        public int VillaNo { get; set; }
        [Required]
        public int VillaId { get; set; }
        [Required]
        [MaxLength(255)]
        public string SpecialDetails { get; set; } = null!;
    }
}
