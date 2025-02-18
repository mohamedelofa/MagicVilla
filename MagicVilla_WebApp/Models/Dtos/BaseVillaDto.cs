using System.ComponentModel.DataAnnotations;

namespace MagicVilla_WebApp.Models.Dtos
{
    public class BaseVillaDto
    {
        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = null!;
        public string Details { get; set; } = null!;
        public double Rate { get; set; }
        public int Sqft { get; set; }
        public int Occupancy { get; set; }
        public string Amenity { get; set; } = null!;
    }
}
