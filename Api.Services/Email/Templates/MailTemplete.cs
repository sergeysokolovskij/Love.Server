using Microsoft.Extensions.Options;
using Api.Models.Exceptions;
using Api.Models.Options;
using Api.Services.Email.Templates;
using Api.Services.Exceptions;
using System;
using System.Collections.Generic;
using System.Security.Policy;
using System.Text;

namespace Api.Services.Email
{
	/*
	 * темлейт для создания сообщений. остальные "стандартные сообщения будут наследоваться от этого"
	 */

	public interface IMailTemplate
	{
		MailInfo BuildMessage(object obj);
	}

	public class MailTemplete : IMailTemplate
	{
		protected MailInfo mailInfo;

		protected readonly IOptions<UrlPathOptions> urlPathOptions;

		public MailTemplete(IOptions<UrlPathOptions> urlPathOptions)
		{
			this.urlPathOptions = urlPathOptions;

			mailInfo = new MailInfo();
		}

		protected bool IsMailInfoCorrect
		{
			get
			{
				return !string.IsNullOrEmpty(mailInfo.Message) &&
					   !string.IsNullOrEmpty(mailInfo.Subject);
			}
		}

		public virtual MailInfo BuildMessage(object obj)
		{
			if (IsMailInfoCorrect)
				return mailInfo;
			throw new ApiError(new ServerException("MailTemplete был построен неверно"));
		}

	}
	public sealed class MailInfo
	{
		public string Message;
		public string Subject;
	}
}
