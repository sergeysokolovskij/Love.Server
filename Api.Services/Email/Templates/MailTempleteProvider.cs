using Microsoft.Extensions.Options;
using Api.Models.Exceptions;
using Api.Models.Options;
using Api.Services.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Api.Services.Email.Templates
{
	public interface IMailTempleteProvider
	{
		IMailTemplate GetMailTemplate(string templateName);
	}
	public sealed class MailTempleteProvider : IMailTempleteProvider
	{
		private readonly IEnumerable<IMailTemplate> mailTempletesCollection;

		public MailTempleteProvider(IEnumerable<IMailTemplate> mailTempletesCollection)
		{
			this.mailTempletesCollection = mailTempletesCollection;
			Init();
		}

		public Dictionary<string, Type> mailTempletes = new Dictionary<string, Type>();

		public IMailTemplate GetMailTemplate(string templateName)
		{
			if (!mailTempletes.TryGetValue(templateName, out Type t))
				throw new ApiError(new ServerException("неправильное имя шаблона сообщения"));
			int count = mailTempletesCollection.Count();

			return mailTempletesCollection.FirstOrDefault(x => x.GetType() == t);
		}


		void Init()
		{
			mailTempletes.Add(MailTempleteNames.AuthMailTemplateName, typeof(AuthMailTemplete));
			mailTempletes.Add(MailTempleteNames.RequestMailTemplateName, typeof(RequestMailTemplate));	
		}
	}

	//имена констант для построения сообщений
	public class MailTempleteNames
	{
		public const string AuthMailTemplateName = "AuthMailTemplete";
		public const string RequestMailTemplateName = "RequestMailTemplate";
	}
}
