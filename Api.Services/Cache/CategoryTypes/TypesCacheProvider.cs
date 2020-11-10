using System;
using System.Collections.Generic;
using System.Text;

namespace Api.Services.Cache.CategoryTypes
{
	public interface ICategoryCacheProvider
	{
		IEnumerable<ITypeCache> CategoryCaches { get; }
	}
	public class TypesCacheProvider : ICategoryCacheProvider
	{
		private readonly IEnumerable<ITypeCache> typeCaches;
		public TypesCacheProvider(IEnumerable<ITypeCache> CategoryCaches)
		{
			typeCaches = CategoryCaches;
		}

		public IEnumerable<ITypeCache> CategoryCaches
		{
			get
			{
				return typeCaches;
			}
		}
	}
}
