namespace MagicVilla_VillaAPI.Models.Dtos
{
    public class UpdateVillaNumberDto 
    {
        [Required]
        [MaxLength(255)]
        public string SpecialDetails { get; set; } = null!;
        [Required]
        public int VillaId { get; set; }
    }
}
