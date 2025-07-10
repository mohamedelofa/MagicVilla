using MagicVilla_VillaAPI.Helpers;
using MagicVilla_VillaAPI.Services.Interface;
using Mailjet.Client;
using Mailjet.Client.TransactionalEmails;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using System.Text;

namespace MagicVilla_VillaAPI.Services.Implementation
{
	public class EmailService : IEmailServivce
	{
		private readonly MailJetSettings _mailJetSettings;
		private readonly IHttpContextAccessor _httpContextAccessor;
		public EmailService(IOptions<MailJetSettings> options, IHttpContextAccessor httpContextAccessor)
		{
			_mailJetSettings = options.Value;
			_httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
		}

		public async Task<bool> SendConfirmEmailAsync(string userEmail, string userName, string token)
		{
			var urlToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
			// generate URL for email confirmation
			string confirmationUrl = $"{_mailJetSettings.Url}/Authentication/ConfirmEmail?email={userEmail}&token={urlToken}";

			var variables = new Dictionary<string, object>
			{
				{ "confirmation_link", confirmationUrl },
				{ "To", userName }
			};
			return await SendEmailAsync(userEmail, variables);
		}

		public async Task<bool> SendEmailAsync(string to, IDictionary<string, object> variables)
		{
			var mailjet = new MailjetClient(_mailJetSettings.ApiKey, _mailJetSettings.SecretKey);
			var mail = new TransactionalEmailBuilder()
				.WithTo(new SendContact(to))
				.WithTemplateId(_mailJetSettings.TemplateId)
				.WithTemplateLanguage(true)
				.WithVariables(variables)
				.Build();
			var response = await mailjet.SendTransactionalEmailAsync(mail);
			return response.Messages?.Count() > 0 && response.Messages[0].Status.Equals("success", StringComparison.OrdinalIgnoreCase);

		}
	}
}
