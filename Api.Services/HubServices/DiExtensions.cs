using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Api.Services.HubServices
{
	public static class DiExtensions
	{
		public static void AddHubs(this IServiceCollection serviceCollection)
		{
			serviceCollection.AddScoped<IMessangerHubService, MessangerHubService>();
		}
	}
}
