using System;
using System.Collections.Generic;
using System.Text;

namespace Api.Services.Cache
{
	public interface ICachePolicy
	{
		bool CanCache(RequestOptions reqOptions);
	}
	public class CustomCachePolicy : ICachePolicy
	{
		public bool CanCache(RequestOptions reqOptions)
		{
			return reqOptions.Page.HasValue && reqOptions.Page == 1 && string.IsNullOrEmpty(reqOptions.Sort);
		}
	}

	public class NeverCachePolicy : ICachePolicy
	{
		public bool CanCache(RequestOptions reqOptions)
		{
			return false;
		}
	}

	public class AlwaysCachePolicy : ICachePolicy
	{
		public bool CanCache(RequestOptions reqOptions)
		{
			return true;
		}
	}
}
