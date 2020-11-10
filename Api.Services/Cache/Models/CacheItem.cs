using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Api.Services.Cache.Models
{
	public class CacheItem
	{
		public string Key { get; private set; }
		public object Data { get; private set; }
		public CacheItem(string key,
			object data)
		{
			this.Key = key;
			this.Data = data;
		}
	}
}
