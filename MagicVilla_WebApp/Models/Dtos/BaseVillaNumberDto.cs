using System.ComponentModel.DataAnnotations;

namespace MagicVilla_WebApp.Models.Dtos
{
    public class BaseVillaNumberDto
    {
        [Required]
        [Range(minimum:1 , maximum:int.MaxValue,ErrorMessage = "VillaNo must be greater than 0")]
        public int VillaNo { get; set; }
        [Required]
        public int VillaId { get; set; }
        [Required]
        [MaxLength(255)]
        public string SpecialDetails { get; set; } = null!;
    }
}
