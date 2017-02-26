using System.ComponentModel;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;

namespace CMon.Web.Services
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string message);
    }

	public class EmailSenderOptions
	{
		public string From { get; set; }

		public string Host { get; set; }

		public int Port { get; set; }
		
		[DefaultValue(true)]
		public bool UseSsl { get; set; }

		public string UserName { get; set; }

		public string Password { get; set; }
	}

	public class MailKitEmailSender : IEmailSender
	{
		private readonly IOptions<EmailSenderOptions> _options;

		public MailKitEmailSender(IOptions<EmailSenderOptions> options)
		{
			_options = options;
		}

		public async Task SendEmailAsync(string email, string subject, string message)
		{
			var options = _options.Value;

			var emailMessage = new MimeMessage();

			emailMessage.From.Add(MailboxAddress.Parse(options.From));
			emailMessage.To.Add(MailboxAddress.Parse(email));
			emailMessage.Subject = subject;
			emailMessage.Body = new TextPart(TextFormat.Html) { Text = message };

			using (var client = new SmtpClient())
			{
				await client.ConnectAsync(options.Host, options.Port,
					options.UseSsl ? SecureSocketOptions.StartTls : SecureSocketOptions.Auto).ConfigureAwait(false);
				await client.AuthenticateAsync(options.UserName, options.Password);
				await client.SendAsync(emailMessage).ConfigureAwait(false);
				await client.DisconnectAsync(true).ConfigureAwait(false);
			}
		}
	}
}
