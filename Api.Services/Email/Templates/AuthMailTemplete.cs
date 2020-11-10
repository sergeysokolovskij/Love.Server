using Microsoft.Extensions.Options;
using Api.Models.Exceptions;
using Api.Models.Options;
using Api.Services.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Api.Services.Email.Templates
{
	internal class AuthMailInfo
	{
		public string Token;
		public AuthMailInfo(string token) => Token = token;
	}
	public class AuthMailTemplete : MailTemplete
	{
		public AuthMailTemplete(IOptions<UrlPathOptions> urlPathOptions) : base(urlPathOptions)
		{
		}

		public override MailInfo BuildMessage(object obj)
		{
			var data = obj as AuthMailInfo;

			if (data == null)
				throw new ApiError(new ServerException("Переданы некорректные данные"));

			mailInfo.Subject = "Подтверждение регистрации";
			mailInfo.Message = $"Чтобы подтвердить регистрацию перейдите по ссылке {urlPathOptions.Value.Api}Account/confirmregister/token/{data.Token}";

			return base.BuildMessage(mailInfo);
		}
	}
}
