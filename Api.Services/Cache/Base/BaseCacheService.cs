using Api.Provider.Base;
using Api.Provider.Cache;
using Api.Utils;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Api.Services.Cache
{
	public interface IBaseCacheService<T> where T : class
	{
		Task<bool> CacheUpdateString(string key, T value);
		Task<bool> CacheRemove(string key);
		Task<bool> CacheSetString(string key, T value, TimeSpan time);
		Task<bool> CacheSetString(string key, T values);
		Task<T> CacheGetString(string key);
		string EntityKey { get; }
		Task PulishMessageAsync(string message);
		bool IsEnabledSubscriber { get; }
	}
	public class BaseCacheService<T> : IBaseCacheService<T> where T : class
	{
		private readonly IBaseRedisProvider redisProvider;

		protected readonly IRedisPublisher redisPublisher;
		protected IDatabase cacheDb;

		public virtual string EntityKey
		{
			get
			{
				return "";
			}
		}

		public virtual bool IsEnabledSubscriber
		{
			get
			{
				return false;
			}
		}

		public BaseCacheService(IBaseRedisProvider redisProvider,
			IRedisPublisher redisPublisher)
		{
			this.redisProvider = redisProvider;
			this.cacheDb = redisProvider.GetDatabase();
			
			this.redisPublisher = redisPublisher;
		}

		public virtual async Task<bool> CacheSetString(string key, T value, TimeSpan time)
		{
			await cacheDb.StringSetAsync(new RedisKey(key), new RedisValue(JsonConvert.SerializeObject(value)), time);
			return true;
		}

		public virtual async Task<bool> CacheSetString(string key, T value)
		{
			await cacheDb.StringSetAsync(new RedisKey(key), new RedisValue(JsonConvert.SerializeObject(value)));
			return true;
		}

		public async virtual Task<bool> CacheRemove(string key)
		{
			var result = await cacheDb.KeyDeleteAsync(key, CommandFlags.None);
			return result;
		}

		public virtual async Task<bool> CacheUpdateString(string key, T value)
		{
			var currentValue = await cacheDb.StringGetAsync(new RedisKey(key));
			if (string.IsNullOrEmpty(currentValue))
				await cacheDb.StringSetAsync(new RedisKey(key), new RedisValue(JsonConvert.SerializeObject(value)));
			else
			{
				await cacheDb.KeyDeleteAsync(new RedisKey(key));
				await cacheDb.StringSetAsync(new RedisKey(key), new RedisValue(JsonConvert.SerializeObject(value)));
			}

			return true;
		}

		public virtual async Task<bool> CacheUpdateString(string key, T value, TimeSpan time)
		{
			var currentValue = await cacheDb.StringGetAsync(new RedisKey(key));
			if (string.IsNullOrEmpty(currentValue))
				await cacheDb.StringSetAsync(new RedisKey(key), new RedisValue(JsonConvert.SerializeObject(value)), time);
			else
			{
				await cacheDb.KeyDeleteAsync(new RedisKey(key));
				await cacheDb.StringSetAsync(new RedisKey(key), new RedisValue(JsonConvert.SerializeObject(value)), time);
			}

			return true;
		}

		public virtual async Task<T> CacheGetString(string key)
		{
			var resultFromCache = (string)await cacheDb.StringGetAsync(key);
			if (string.IsNullOrEmpty(resultFromCache))
				return null;

			return JsonConvert.DeserializeObject<T>(resultFromCache);
		}

		public virtual Task PulishMessageAsync(string message)
		{
			return redisPublisher.PublishUpdateInRedis(EntityKey, message);
		}
	}
}
