﻿namespace MagicVilla_WebApp.Models.Dtos
{
	public class UpdateVillaDto : BaseVillaDto
	{
		public int Id { get; set; }
		public IFormFile? Image { get; set; }
	}
}
