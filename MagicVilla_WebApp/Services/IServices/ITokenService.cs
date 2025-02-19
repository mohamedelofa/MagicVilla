namespace MagicVilla_WebApp.Services.IServices
{
	public interface ITokenService
	{
		void SetToken(string accessToken);
		string GetToken();
		void DeleteToken();
	}
}
