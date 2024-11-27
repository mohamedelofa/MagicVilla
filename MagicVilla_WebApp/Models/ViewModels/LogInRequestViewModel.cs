using System.ComponentModel.DataAnnotations;

namespace MagicVilla_WebApp.Models.ViewModels
{
	public class LogInRequestViewModel
	{
		public string UserName { get; set; } = null!;

		[DataType(DataType.Password)]
		public string Password { get; set; } = null!;
	}
}
