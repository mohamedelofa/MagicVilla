namespace MagicVilla_VillaAPI.Services.Interface
{
	public interface IEmailServivce
	{
		Task<bool> SendEmailAsync(string to, IDictionary<string, object> variables);
		Task<bool> SendConfirmEmailAsync(string userEmail, string userName, string token);
	}
}
