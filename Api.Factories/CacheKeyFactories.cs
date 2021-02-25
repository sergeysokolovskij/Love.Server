using Api.Models.CacheModels;
using System;
using System.Collections.Generic;

namespace Api.Factories
{
	public class CacheKeyFactories
	{
		public static string GenerateDynamicCacheKey(BaseCacheModel model)
		{
			string result = $"{model.EntityType}-{model.Id}-";
			return result;
		}

		public static string GenerateConnectionCacheKey(string userId, string entityType)
		{
			return $"{userId}-{entityType}";
		}

		public static string GenerateSessionCacheKey(string userId, string tokenId,string entityType)
		{
			return $"{userId}-{tokenId}-{entityType}";
		}

		public static string GenerateSessionKeyCache(string userId, string tokenId)
		{
			return $"{userId}-{tokenId}";
		}

		public static string GenerateAesKeyCache(string userId)
        {
			return $"{userId}-aes";
        }
	}
}
