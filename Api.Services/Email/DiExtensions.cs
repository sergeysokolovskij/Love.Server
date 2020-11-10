using Microsoft.Extensions.DependencyInjection;
using Api.Services.Email.Templates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Api.Services.Email
{
	public static class DiExtensions
	{
		public static void RegisterEmailServices(this IServiceCollection serviceCollection)
		{
			serviceCollection.AddSingleton<IEmailSenderService, EmailSenderService>();
			serviceCollection.AddSingleton<IMailTempleteProvider, MailTempleteProvider>();

			
			var types = typeof(IMailTemplate).Assembly.GetTypes();
			var resultTypes = types.Where(x => x.IsPublic && 
				x.IsClass && 
				!x.IsAbstract
				&& typeof(IMailTemplate).IsAssignableFrom(x));

			foreach (var result in resultTypes)
				serviceCollection.AddSingleton(typeof(IMailTemplate), result);
		}
	}
}
