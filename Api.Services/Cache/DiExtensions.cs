using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using Api.Provider.Base;
using Api.Services.Cache.CacheServices;

namespace Api.Services.Cache
{
	public static class DiExtensions
	{
		public static void RegisterCacheServices(this IServiceCollection serviceCollection)
		{
			serviceCollection.AddSingleton<ISubscriber, Subscriber>();

			serviceCollection.AddScoped<IUserProfileCacheService, UserProfileCacheService>();
			serviceCollection.AddScoped<ISessionCacheService, SessionCacheService>();
			serviceCollection.AddScoped<IConnectionCacheService, ConnectionCacheService>();
		}
	}
}
