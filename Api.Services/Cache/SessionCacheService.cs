using Api.Dal.Auth;
using Api.Factories;
using Api.Models.Cache;
using Api.Models.Options;
using Api.Provider.Base;
using Api.Provider.Cache;
using Api.Provider.Messanger;
using Api.Services.Exceptions;
using Api.Services.Logs;
using Api.Utils;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api.Services.Cache
{
	public interface ISessionCacheService : IBaseCacheService<Session>
	{
		Task<SessionCacheModel> GetSessionsFromCacheAsync(string userId, string tokenId);
		Task<bool> AddSessionAsync(Session value, string tokenId);
		Task<bool> CacheSessionKeysAsync(string tokenSessionId, string userId, string sessionId);
		Task<SessionCacheKeyModel> GetSessionCacheKeyAsync(string userId, string tokenId);
	}
	public class SessionCacheService : BaseCacheService<Session>, ISessionCacheService
	{
		public override string EntityKey => "MessangerSession";
		public override bool IsEnabledSubscriber => false;

		private readonly ISessionProvider sessionProvider;
		private readonly IOptions<TokenLifeTimeOptions> authOptions;

		public SessionCacheService(IBaseRedisProvider redisProvider, 
			IRedisPublisher redisPublisher,
			ISessionProvider sessionProvider,
			IOptions<TokenLifeTimeOptions> authOptions) : base(redisProvider, redisPublisher)
		{
			this.sessionProvider = sessionProvider;
			this.authOptions = authOptions;
		}

		public async Task<bool> CacheSessionKeysAsync(string tokenId,string userId, string sessionId)
		{
			string key = CacheKeyFactories.GenerateSessionKeyCache(userId, tokenId);
			var data = await cacheDb.StringGetAsync(new RedisKey(key));

			var cacheModel = new SessionCacheKeyModel()
			{
				SessionId = sessionId,
				UserId = userId
			};

			string json = JsonConvert.SerializeObject(cacheModel);

			if (data.HasValue)
			{
				await cacheDb.KeyDeleteAsync(new RedisKey(key));
				await cacheDb.StringSetAsync(new RedisKey(key), new RedisValue(json), TimeSpan.FromMinutes(authOptions.Value.AccessTokenLifeTime));

				return true;
			}

			await cacheDb.StringSetAsync(new RedisKey(key), new RedisValue(json), TimeSpan.FromMinutes(authOptions.Value.AccessTokenLifeTime));
			return true;
		}

		public async Task<SessionCacheKeyModel> GetSessionCacheKeyAsync(string userId, string tokenId)
		{
			try
			{
				string key = CacheKeyFactories.GenerateSessionKeyCache(userId, tokenId);
				var result = await cacheDb.StringGetAsync(new RedisKey(key));

				return JsonConvert.DeserializeObject<SessionCacheKeyModel>(result);
			}
			catch(Exception ex)
			{
				Logger.ErrorLog(ex);
				return null;
			}
		}
		
		public async Task<bool> AddSessionAsync(Session value, string tokenId)
		{
			var sessionCacheKey = await GetSessionCacheKeyAsync(value.UserId, tokenId);
			if (sessionCacheKey == null)
				throw new ApiError(new ServerException("INVALID SESSION CACHE KEY!!!!"));

			var session = await sessionProvider.GetModelBySearchPredicate(x => x.UserId == value.UserId 
				&& x.SessionId == sessionCacheKey.SessionId);

			if (session == null)
				throw new ApiError(new ServerException("Invalid sessions!!!"));

			string cacheKey = CacheKeyFactories.GenerateSessionCacheKey(value.UserId, tokenId, EntityKey);

			var cacheModel = new SessionCacheModel()
			{
				ClientPublicKey = session.ClientPublicKey,
				ServerPrivateKey = session.ServerPrivateKey,
				ServerPublicKey = session.ServerPublicKey,
				UserId = session.UserId,
				SessionId = session.SessionId
			};

			string json = JsonConvert.SerializeObject(cacheModel);
			await cacheDb.StringSetAsync(cacheKey, json);

			return true;
		}

		public async Task<SessionCacheModel> GetSessionsFromCacheAsync(string userId, string tokenId)
		{
			var sessionCacheKey = await GetSessionCacheKeyAsync(userId, tokenId);

			if (sessionCacheKey == null)
				throw new ApiError(new ServerException("Invalid session cache key!!!"));

			string cacheKey = CacheKeyFactories.GenerateSessionCacheKey(userId,tokenId,EntityKey);
			
			var dataFromCache = await cacheDb.StringGetAsync(new RedisKey(cacheKey));
			if (dataFromCache.HasValue)
			{
				var result = JsonConvert.DeserializeObject<SessionCacheModel>(dataFromCache);
				return result;
			}

			var sessionFromDb = await sessionProvider.GetModelBySearchPredicate(x => x.UserId == userId && x.SessionId == sessionCacheKey.SessionId);
			if (sessionFromDb == null)
				return null; 

			await CacheSetString(cacheKey, sessionFromDb, TimeSpan.FromMinutes(authOptions.Value.AccessTokenLifeTime));

			return new SessionCacheModel()
			{
				ClientPublicKey = sessionFromDb.ClientPublicKey,
				ServerPrivateKey = sessionFromDb.ServerPrivateKey,
				ServerPublicKey = sessionFromDb.ServerPublicKey,
				SessionId = sessionFromDb.SessionId,
				UserId = sessionFromDb.UserId
			};
		}
	}
}
