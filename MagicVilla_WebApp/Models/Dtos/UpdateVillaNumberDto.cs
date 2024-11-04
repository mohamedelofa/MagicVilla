using System.ComponentModel.DataAnnotations;

namespace MagicVilla_WebApp.Models.Dtos
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
