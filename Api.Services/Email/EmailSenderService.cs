using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Api.Models.Options;
using Api.Services.Email.Templates;
using Apis.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using Api.Utils;

namespace Api.Services.Email
{
	public interface IEmailSenderService
	{
		Task SendEmailAsync(
			object data,
			string toEmail,
			string templateName,
			List<string> pathToFiles = null
			);
		bool IsNeedConfirm();
	}
	public sealed class EmailSenderService : IEmailSenderService
	{
		private readonly IOptionsMonitor<MailOptions> mailOptions;
		private readonly IMailTempleteProvider mailTempleteProvider;

		private readonly ILogger<EmailSenderService> logger;

		private readonly IConfiguration configuration;

		public EmailSenderService(IMailTempleteProvider mailTempleteProvider,
			IOptionsMonitor<MailOptions> mailOptions,
			ILoggerFactory loggerFactory,
			IConfiguration configuration)
		{
			this.mailTempleteProvider = mailTempleteProvider;
			this.mailOptions = mailOptions;
			this.configuration = configuration;

			logger = loggerFactory.CreateLogger<EmailSenderService>();
		}

		public bool IsNeedConfirm()
		{
			return bool.Parse(configuration["needConfirmRegister"]);
		}

		public async Task SendEmailAsync(
			object data,
			string toEmail,
			string templateName,
			List<string> pathToFiles = null
			)
		{
			var templateBuilder = mailTempleteProvider.GetMailTemplate(templateName);
			var mailInfo = templateBuilder.BuildMessage(data);
			await SendEmailAsync(toEmail, mailInfo.Subject, null, mailInfo.Message, pathToFiles);
		}

		private async Task SendEmailAsync(
			string toEmail, 
			string subject,
			string htmlMessage,
			string textMessage = null,
			List<string> pathToFiles = null
			)
		{
		 	using MailMessage mailMessage = new MailMessage
			{
				From = new MailAddress(mailOptions.CurrentValue.Login, mailOptions.CurrentValue.DisplayedName),
				Body = textMessage,
				Subject = subject,
				BodyEncoding = Encoding.UTF8,
				SubjectEncoding = Encoding.UTF8
			};
			bool isFilesExist = false;
			if (pathToFiles.IsListNotNull())
			{
				foreach (var pathToFile in pathToFiles)
					mailMessage.Attachments.Add(new Attachment(pathToFile));
				isFilesExist = true;
			}

			mailMessage.To.Add(mailOptions.CurrentValue.To);
			if (!string.IsNullOrEmpty(htmlMessage))
			{
				var htmlView = AlternateView.CreateAlternateViewFromString(htmlMessage);
				htmlView.ContentType = new ContentType("text/html");
				mailMessage.AlternateViews.Add(htmlView);
			}

			using SmtpClient client = new SmtpClient(MailSenderConstants.Host, MailSenderConstants.Port) // добавить хост и порт
			{
				UseDefaultCredentials = false,
				EnableSsl = true,
				Credentials = new NetworkCredential(mailOptions.CurrentValue.Login, mailOptions.CurrentValue.Password)
			};
			try
			{
				await client.SendMailAsync(mailMessage);
				if (pathToFiles.IsListNotNull())
					foreach (var i in pathToFiles)
						File.Delete(i);

			}
			catch(Exception ex)
			{
				logger.LogError(ex.Message);
			}
		}
	}
	internal class MailSenderConstants
	{
		internal const int Port = 25;
		internal const string Host = "smtp.gmail.com";
	}
}
