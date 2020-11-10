using System;
using System.Collections.Generic;
using System.Text;

namespace Api.Services.Cache
{
	public class CacheKeys 
	{
		public const string CityCache = "allcities";

		public static string GetDynamicCacheKey(string controllerName, string actionName, int? page)
		{
			if (page.HasValue)
				return $"{controllerName}-{actionName}-{page.Value}"; 
			return $"{controllerName}-{actionName}";
		}
	}
}
