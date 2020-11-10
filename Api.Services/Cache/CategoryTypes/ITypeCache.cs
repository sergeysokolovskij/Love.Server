using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Text;

namespace Api.Services.Cache.CategoryTypes
{
	public interface ITypeCache
	{
		MemoryCacheEntryOptions GetOptions();
	}
}
