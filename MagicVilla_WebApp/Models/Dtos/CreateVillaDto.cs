using System.ComponentModel.DataAnnotations;

namespace MagicVilla_WebApp.Models.Dtos
{
    public class CreateVillaDto : BaseVillaDto
    {
        [Required]
        public IFormFile Image { get; set; }
    }
}
