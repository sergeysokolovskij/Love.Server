using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Text;

namespace Api.Services.Cache.CategoryTypes
{
	public class SlowCache : ITypeCache
	{
		public MemoryCacheEntryOptions GetOptions()
		{
			return new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(10));
		}
	}
}
