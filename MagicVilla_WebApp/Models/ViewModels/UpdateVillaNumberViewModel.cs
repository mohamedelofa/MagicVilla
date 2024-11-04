using MagicVilla_WebApp.Models.Dtos;
using System.ComponentModel.DataAnnotations;

namespace MagicVilla_WebApp.Models.ViewModels
{
	public class UpdateVillaNumberViewModel
	{
		public int VillaNo { get; set; }
		[Required]
		[MaxLength(255)]
		public string SpecialDetails { get; set; } = null!;
		[Required]
		public int VillaId { get; set; }
		public IEnumerable<GetVillaDto>? Villas { get; set; }
	}
}
