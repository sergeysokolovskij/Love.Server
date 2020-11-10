using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Text;

namespace Api.Services.Cache.CategoryTypes
{
	public class FastCache : ITypeCache
	{
		public MemoryCacheEntryOptions GetOptions()
		{
			var options = new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(5));
			return options;
		}
	}
}
