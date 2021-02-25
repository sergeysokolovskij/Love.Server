using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;

namespace Api.Provider.Base
{
	public interface IBaseRedisProvider
	{
		IDatabase GetDatabase();
		ISubscriber GetSubscribers();
	}
	public class BaseRedisProvider : IBaseRedisProvider
	{
		private ConnectionMultiplexer ConnectionMultiplexer = ConnectionMultiplexer.Connect("localhost");

		public IDatabase GetDatabase()
		{
			return ConnectionMultiplexer.GetDatabase();
		}

		public ISubscriber GetSubscribers()
		{
			return ConnectionMultiplexer.GetSubscriber();
		}
	}
}
