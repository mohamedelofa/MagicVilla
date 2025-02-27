namespace MagicVilla_WebApp
{
	public static class StaticDetails
	{
		public enum ApiType
		{
			GET,
			POST,
			PUT,
			DELETE
		}
		public static string AccessToken = "AccessToken";
		public static string RefreshToken = "RefreshToken";
		public static string Version = "v2";
		public const string Admin = "admin";
		public const string User = "user";
		public enum ContentType
		{
			Json,
			MultipartFormData
		}
	}
}
