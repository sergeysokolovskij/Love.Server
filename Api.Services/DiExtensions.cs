using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Api.Services.Crypt;
using Api.Services.Auth;
using Api.Services.Cache;
using Api.Services.Email;

namespace Api.Services
{
	public static class DiExtensions
	{
		public static void RegisterServices(this IServiceCollection serviceCollection, IConfiguration configuration)
		{
			serviceCollection.RegisterCyphers();
			serviceCollection.AddAuthService();
			serviceCollection.RegisterCacheServices();
			serviceCollection.RegisterEmailServices();
		} 
	}
}
