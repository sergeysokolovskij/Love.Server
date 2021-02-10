using Api.Dal;
using Api.Factories;
using Api.Models.Cache;
using Api.Models.Options;
using Api.Provider;
using Api.Provider.Base;
using Api.Provider.Cache;
using Api.Utils;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api.Services.Cache
{
	public interface IConnectionCacheService : IBaseCacheService<ConnectionCacheModel>
	{
		Task AddUserConnectionAsync(string userId, string connectionId, string sessionId);
		Task RemoveUserConnectionAsync(string userId, string connectionId);
		Task<Dictionary<string ,string>> GetUserConnectionsAsync(string userId);
	}
	public class ConnectionCacheService : BaseCacheService<ConnectionCacheModel>, IConnectionCacheService
	{
		public override string EntityKey => "ConnectionCache";

		private readonly IConnectionProvider connectionProvider;
		private readonly IOptions<TokenLifeTimeOptions> tokenOptions;

		public ConnectionCacheService(IBaseRedisProvider redisProvider, 
			IRedisPublisher redisPublisher,
			IConnectionProvider connectionProvider,
			IOptions<TokenLifeTimeOptions> tokenOptions) : base(redisProvider, redisPublisher)
		{
			this.connectionProvider = connectionProvider;
			this.tokenOptions = tokenOptions;
		}


		public async Task AddUserConnectionAsync(string userId, string connectionId, string sessionId)
		{
			string cacheKey = CacheKeyFactories.GenerateConnectionCacheKey(userId, EntityKey);

			ConnectionCacheModel model = new ConnectionCacheModel()
			{
				Connections = new Dictionary<string, string>() 
				{
					{connectionId, sessionId }
				},
				UserId = userId
			};

			var connections = await connectionProvider.GetModelsBySearchPredicate(x => x.UserId == userId); 

			if (connections.IsListNotNull())
			{
				var connectionIdenteficators = new List<string>();

				foreach (var connection in connections)
					model.Connections.Add(connection.ConnectionId, connection.SessionId);

				await CacheRemove(cacheKey);
			}

			await connectionProvider.CreateOrUpdateAsync(new Connection()
			{
				UserId = userId,
				Created = DateTime.Now,
				ConnectionId = connectionId,
				SessionId = sessionId
			});

			await CacheSetString(cacheKey, model, TimeSpan.FromMinutes(tokenOptions.Value.AccessTokenLifeTime));
		}

		public async Task RemoveUserConnectionAsync(string userId,string connectionId)
		{
			var savedItem = await connectionProvider.GetModelBySearchPredicate(x => x.ConnectionId == connectionId);

			if (savedItem != null)
				await connectionProvider.RemoveAsync(savedItem);

			string cacheKey = CacheKeyFactories.GenerateConnectionCacheKey(userId, EntityKey);
			var cacheValue = await CacheGetString(cacheKey);

			if (cacheValue != null)
			{
				var connection = cacheValue.Connections.Where(x => x.Key == connectionId).FirstOrDefault();
				if (connection.Key != null)
					cacheValue.Connections.Remove(connection.Key);

				await CacheUpdateString(cacheKey, cacheValue, TimeSpan.FromMinutes(tokenOptions.Value.AccessTokenLifeTime));
			}
		}

		public async Task<Dictionary<string,string>> GetUserConnectionsAsync(string userId)
		{
			string cacheKey = CacheKeyFactories.GenerateConnectionCacheKey(userId, EntityKey);
			int countConnectionsInDb = await connectionProvider.CountConnectionsAsync(userId);

			var dataFromCache = await CacheGetString(cacheKey);
			if (countConnectionsInDb == 0 && dataFromCache == null || (dataFromCache == null || dataFromCache.Connections.Keys.Count == 0))
				return null;

			if (dataFromCache.Connections.Keys.Count > 0 && dataFromCache.Connections.Count == countConnectionsInDb)
				return dataFromCache.Connections;

			var connectionsFromDb = await connectionProvider.GetModelsBySearchPredicate(x => x.UserId == userId);
			var connectionsIdentefier = new Dictionary<string ,string>();

			foreach (var connection in connectionsFromDb)
				connectionsIdentefier.Add(connection.ConnectionId, connection.SessionId);

			var cacheValue = new ConnectionCacheModel()
			{
				UserId = userId,
				Connections = connectionsIdentefier
			};

			await CacheUpdateString(cacheKey, cacheValue, TimeSpan.FromMinutes(tokenOptions.Value.AccessTokenLifeTime));

			return cacheValue.Connections;
		}
	}
}
