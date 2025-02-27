namespace MagicVilla_VillaAPI.Models
{
	public class RefreshToken
	{
		[Key]
		public int Id { get; set; }
		public string Token { get; set; }
		public DateTime ExpireOn { get; set; }
		public bool IsExpired => DateTime.UtcNow >= ExpireOn;
		public string UserId { get; set; }
		public string JwtTokenId { get; set; }
		public bool IsValid { get; set; } = true;
	}
}
