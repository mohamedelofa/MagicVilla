using MagicVilla_WebApp.Models.Dtos;

namespace MagicVilla_WebApp.Models.ViewModels
{
	public class CreateVillaNumberViewModel :BaseVillaNumberDto
	{
		public IEnumerable<GetVillaDto>? Villas { get; set; }
	}
}
