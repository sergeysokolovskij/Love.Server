using Microsoft.Extensions.DependencyInjection;
using System.Numerics;

namespace Api.Providers
{
	public static class DiExtensions
	{
		public static void RegisterProviders(this IServiceCollection serviceCollection)
		{
			serviceCollection.AddTransient<IUserProvider, UserProvider>();
			serviceCollection.AddTransient<ICypherProvider, CypherProvider>();
		}
	}
}
