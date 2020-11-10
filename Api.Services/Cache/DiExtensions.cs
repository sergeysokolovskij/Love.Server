using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Api.Services.Cache.CategoryTypes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Api.Services.Cache
{
	public static class DiExtensions
	{
		public static void RegisterCacheServices(this IServiceCollection serviceCollection)
		{
			serviceCollection.AddSingleton<ICacheService, CacheService>();
			serviceCollection.AddSingleton<CachePolicyProvider>();
			serviceCollection.AddSingleton<ICacheTokenProvider, CacheTokenProvider>();

			serviceCollection.RegisterTypesCaches();
			serviceCollection.AddMemoryCache();
		}
	}
}
