using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Api.Services.Messanger
{
	public static class DiExtensions
	{
		public static void AddMessageServices(this IServiceCollection serviceCollection)
		{
			serviceCollection.AddScoped<IMessangerService, MessangerService>();
			serviceCollection.AddScoped<ISessionService, SessionService>();
		}
	}
}
