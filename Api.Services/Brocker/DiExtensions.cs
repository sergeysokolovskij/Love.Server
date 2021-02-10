using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Api.Services.Brocker
{
	public static class DiExtensions
	{
		public static void AddBrockerService(this IServiceCollection serviceCollection)
		{
			serviceCollection.AddSingleton<IConnectionsProvider, ConnectionsProvider>();
			serviceCollection.AddSingleton<IBrockerService, BrockerService>();
		}
	}
}
