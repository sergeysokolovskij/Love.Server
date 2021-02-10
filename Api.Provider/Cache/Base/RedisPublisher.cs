using Api.Provider.Base;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Api.Provider.Cache
{
	public interface IRedisPublisher
	{
		Task PublishUpdateInRedis(string chanel, string message);
	}

	public class RedisPublisher : IRedisPublisher
	{
		private IBaseRedisProvider baseRedisProvider;

		public RedisPublisher(IBaseRedisProvider baseRedisProvider)
		{
			this.baseRedisProvider = baseRedisProvider;
		}

		public async Task PublishUpdateInRedis(string chanel, string message)
		{
			var subscriber = baseRedisProvider.GetSubscribers();
			try
			{
				await subscriber.PublishAsync(new RedisChannel(chanel, RedisChannel.PatternMode.Literal), new RedisValue(message));
			}
			catch
			{
			}
		}
	}
}
