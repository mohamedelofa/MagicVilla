namespace MagicVilla_VillaAPI.Helpers
{
	public class MailJetSettings
	{
		public string ApiKey { get; set; } = string.Empty;
		public string SecretKey { get; set; } = string.Empty;
		public long TemplateId { get; set; }
		public string Url { get; set; } = string.Empty;
	}
}
