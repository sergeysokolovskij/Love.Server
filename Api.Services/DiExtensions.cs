using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Api.Services.Crypt;
using Api.Services.Auth;
using Api.Services.Cache;
using Api.Services.Email;
using Api.Services.Messanger;
using Api.Services.Brocker;
using Api.Services.HubServices;

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
			serviceCollection.AddMessageServices();
			serviceCollection.AddBrockerService();
			serviceCollection.AddHubs();
		} 
	}
}
