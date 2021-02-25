using Api.Factories;
using Api.Models.CacheModels;
using Api.Provider.Base;
using Api.Utils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static StackExchange.Redis.RedisChannel;

namespace Api.Services.Cache.CacheServices
{
	public interface ISubscriber
	{
		Task SetAllSubscribersAsync();
	}
	public class Subscriber : ISubscriber
	{
		private readonly IBaseRedisProvider baseRedisProvider;
		private readonly IServiceProvider serviceProvider;
		private readonly ILogger<Subscriber> logger;

		public Subscriber(IBaseRedisProvider baseRedisProvider,
			IServiceProvider serviceProvider,
			ILoggerFactory loggerFactory)
		{
			this.baseRedisProvider = baseRedisProvider;
			this.serviceProvider = serviceProvider;
			logger = loggerFactory.CreateLogger<Subscriber>();
		}

		public async Task SetAllSubscribersAsync()
		{
			var cacheDb = baseRedisProvider.GetDatabase();
			var subscribers = baseRedisProvider.GetSubscribers();
			var resultTypes = new List<Type>();

			foreach (var type in Assembly.GetAssembly(typeof(Subscriber)).GetTypes())
			{

			}

			using var scope = serviceProvider.GetService<IServiceScopeFactory>().CreateScope();

			foreach (var type in resultTypes)
			{
				var cacheService = scope.ServiceProvider.GetRequiredService(type);

				string entityName = type.GetPropertyValue<string>("EntityName");
				bool isEnabledSubscriber = type.GetPropertyValue<bool>("IsEnabledSubscriber");
			
				if (isEnabledSubscriber)
				{
					await subscribers.SubscribeAsync(new RedisChannel(entityName, PatternMode.Literal), async (chanel, message) =>
					{
						string strMessage = (string)message;
						logger.LogInformation(strMessage);

						var baseCache = await strMessage.FromJson<BaseCacheModel>();
						var key = CacheKeyFactories.GenerateDynamicCacheKey(baseCache);

						var savedElement = await cacheDb.StringGetAsync(new RedisKey(key));
						if (string.IsNullOrEmpty(savedElement))
						{
							await cacheDb.StringSetAsync(new RedisKey(key), new RedisValue(message));
						}
						else
						{
							await cacheDb.KeyDeleteAsync(new RedisKey(key));
							await cacheDb.StringSetAsync(new RedisKey(key), new RedisValue(message));
						}
					});
				}
			}
		}
	}
}
