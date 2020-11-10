using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Api.Services.Cache.CategoryTypes
{
	public static class DiExtensions
	{
		public static void RegisterTypesCaches(this IServiceCollection serviceCollection)
		{
			serviceCollection.AddSingleton<ITypeCache, FastCache>();
			serviceCollection.AddSingleton<ITypeCache, SlowCache>();
		}
	}
}
