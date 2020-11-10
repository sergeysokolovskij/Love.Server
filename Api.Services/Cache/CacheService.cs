using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Api.Services.Cache.CategoryTypes;
using Api.Services.Logs;
using System;
using System.Threading;

namespace Api.Services.Cache
{
	public interface ICacheService
	{
		object Get(string key);
		void Set(string key, object value, ITypeCache categoryCache);
	}
	public class CacheService : ICacheService
	{
		private readonly IMemoryCache cache;
		private readonly ICacheTokenProvider cacheTokenProvider;

		private readonly ILogger<CacheService> logger;
		private static object syncObject = new object();
		public CacheService()
		{

		}

		public CacheService(IMemoryCache cache,
			ICacheTokenProvider cacheTokenProvider,
			ILoggerFactory loggerFactory)
		{
			this.cache = cache;
			this.cacheTokenProvider = cacheTokenProvider;

			logger = loggerFactory.CreateLogger<CacheService>();
		}

		public object Get(string key)
		{
			return cache.Get(key);
		}
		public void Set(string key, object value, ITypeCache categoryCache)
		{
			if (cache.Get(key) != null)
				logger.LogInformation(string.Format("Cache with key '{0}' was rewrite", key));

			logger.LogInformation(string.Format("Cache with key '{0}' was set", key));
			lock (syncObject)
			{
				var options = categoryCache.GetOptions();

				var cancelTokenSource = new CancellationTokenSource();
				options.AddExpirationToken(new CancellationChangeToken(cancelTokenSource.Token));
				cacheTokenProvider.AddItem(key, cancelTokenSource);

				options.RegisterPostEvictionCallback((object callbackKey, object callbackValue, EvictionReason reason, object state) =>
				{
					try
					{
						cacheTokenProvider.RemoveItem((string)callbackKey);
					}
					catch(Exception ex)
					{
						logger.LogError(ex.Message);
						Logger.ErrorLog(ex);
					}
				});
				cache.Set(key, value, categoryCache.GetOptions());
			}
		}
	}
}
