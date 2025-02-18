using System.ComponentModel;

namespace MagicVilla_VillaAPI.Models.Dtos
{
    public class BaseVillaDto
    {
        [Required(ErrorMessage = "Name is required")]
        [MaxLength(50)]
        public string Name { get; set; } = null!;
        [Required(ErrorMessage = "Details is required")]
        public string Details { get; set; } = null!;
        [DefaultValue(2)]
        public double Rate { get; set; }
        [Required(ErrorMessage = "Sqft is required")]
        public int Sqft { get; set; }
        [DefaultValue(2)]
        public int Occupancy { get; set; }
        [Required(ErrorMessage = "Amenity is required")]
        public string Amenity { get; set; } = null!;
    }
}
