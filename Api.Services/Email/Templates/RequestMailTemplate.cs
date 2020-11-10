using Microsoft.Extensions.Options;
using Shared;
using Api.Models.Options;
using System.Text;

namespace Api.Services.Email.Templates
{
	public class RequestMailTemplate : MailTemplete
	{
		public RequestMailTemplate(IOptions<UrlPathOptions> urlPathOptions) : base(urlPathOptions)
		{
		}

		public override MailInfo BuildMessage(object obj)
		{
			var data = obj as RequestModel;
			mailInfo.Subject = "Заявка";

			var sb = new StringBuilder();
			sb.AppendLine(string.Format("Email: {0}", data.Email));

			if (data.CustomFields != null)
			{
				foreach (var field in data.CustomFields)
					sb.AppendLine(string.Format("{0}: {1}", field.Key, field.Value));
			}
			mailInfo.Message = sb.ToString();
			return base.BuildMessage(mailInfo);
		}
	}
}
