using Api.Dal;
using Api.Provider.Base;
using Api.Provider.Cache;
using System;
using System.Collections.Generic;
using System.Text;

namespace Api.Services.Cache
{
	public interface IUserProfileCacheService : IBaseCacheService<Profile>
	{

	}

	public class UserProfileCacheService : BaseCacheService<Profile>, IUserProfileCacheService
	{
		public override string EntityKey => "Profile";
		public override bool IsEnabledSubscriber => true;

		public UserProfileCacheService(IBaseRedisProvider redisProvider, 
			IRedisPublisher redisPublisher) : base(redisProvider, redisPublisher)
		{
		}


	}
}
