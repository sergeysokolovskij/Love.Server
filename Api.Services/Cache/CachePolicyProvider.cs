using Microsoft.Extensions.Options;
using Api.Models.Exceptions;
using Api.Models.Options;
using Api.Services.Cache.CategoryTypes;
using Api.Services.Exceptions;

namespace Api.Services.Cache
{
	public class CachePolicyProvider
	{
		private readonly IOptions<CacheOptions> cacheOptions;
		public CachePolicyProvider(IOptions<CacheOptions> cacheOptions)
		{
			this.cacheOptions = cacheOptions;
		}

		private AlwaysCachePolicy _alwaysCachePolicy;
		private NeverCachePolicy _neverCachePolicy;
		private CustomCachePolicy _customCachePolicy;

		private AlwaysCachePolicy AlwaysCachePolicy
		{
			get
			{
				if (_alwaysCachePolicy == null)
					_alwaysCachePolicy = new AlwaysCachePolicy();
				return _alwaysCachePolicy;
			}
		}
		private NeverCachePolicy NeverCachePolicy
		{
			get
			{
				if (_neverCachePolicy == null)
					_neverCachePolicy = new NeverCachePolicy();
				return _neverCachePolicy;
			}
		}

		private CustomCachePolicy CustomCachePolicy
		{
			get
			{
				if (_customCachePolicy == null)
					_customCachePolicy = new CustomCachePolicy();
				return _customCachePolicy;
			}
		}

		public bool CanCache(RequestOptions reqOptions)
		{
			switch (cacheOptions.Value.PolicyName)
			{
				case "always":
					return AlwaysCachePolicy.CanCache(reqOptions);
				case "never":
					return NeverCachePolicy.CanCache(reqOptions);
				case "custom":
					return CustomCachePolicy.CanCache(reqOptions);
				default:
					throw new ApiError(new ServerException("Ivalic cache policy. You need change globalconfig.json"));
			}
		}
	}
}
