﻿namespace MagicVilla_VillaAPI.Models.Dtos
{
	public class LogInResponseDto
	{
		public string AccessToken { get; set; } = null!;
		public string RefreshToken { get; set; } = null!;
	}
}
