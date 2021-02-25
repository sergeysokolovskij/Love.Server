using Api.Factories;
using Api.Models.Cache;
using Api.Provider;
using Api.Provider.Base;
using Api.Provider.Cache;
using Api.Services.Exceptions;
using Api.Utils;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Api.Services.Cache
{
    public interface ICryptCacheService : IBaseCacheService<CryptCacheModel>
    {
        Task<string> GetOrSetAesKeyAsync(string userId);
    }

    /// <summary>
    /// Кеш для быстрого доступа к user strong-key
    /// </summary>

    public class CryptCacheService : BaseCacheService<CryptCacheModel>, ICryptCacheService
    {
        private readonly IStrongKeyProvider strongKeyProvider;
        private readonly ILogger<CryptCacheService> logger;

        public CryptCacheService(IBaseRedisProvider redisProvider,
            IRedisPublisher redisPublisher,
            IStrongKeyProvider strongKeyProvider,
            ILoggerFactory loggerFactory) : base(redisProvider, redisPublisher)
        {
            this.strongKeyProvider = strongKeyProvider;
            this.logger = loggerFactory.CreateLogger<CryptCacheService>();
        }
     
        public async Task<string> GetOrSetAesKeyAsync(string userId)
        {
            logger.LogInformation($"try to get {userId} cacheKey");
            string cacheKey = CacheKeyFactories.GenerateAesKeyCache(userId);
            var result = await CacheGetString(cacheKey);

            if (result != null)
                return result.Aes;

            var strongKey = await strongKeyProvider.GetStrongKeyAsync(userId);
            if (strongKey == null)
            {
                logger.LogInformation($"Key was not exist. User id: {userId}");
                throw new ApiError(new ServerException("Internal server error"));
            }
            var cacheModel = new CryptCacheModel()
            {
                Aes = strongKey.Secret.ToUrlSafeBase64()
            };
            await CacheSetString(cacheKey, cacheModel, TimeSpan.FromMinutes(5));
            return cacheModel.Aes;
        }
    }
}
