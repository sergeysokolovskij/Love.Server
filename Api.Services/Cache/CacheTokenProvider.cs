using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace Api.Services.Cache
{
	public interface ICacheTokenProvider
	{
		void AddItem(string key, CancellationTokenSource tokenSource);
		void RemoveItem(string key);
		void ClearAllCache();
	}
	public class CacheTokenProvider : ICacheTokenProvider
	{
		//Содержит имя коллекции кешированных обьектов и "ключи"
		private readonly ConcurrentDictionary<string, CancellationTokenSource> keysDictionary = new ConcurrentDictionary<string, CancellationTokenSource>();
		public void ClearAllCache()
		{
			foreach (var token in keysDictionary.Values)
			{
				token.Cancel();
			}
		}

		public void AddItem(string key, CancellationTokenSource value)
		{
			if (keysDictionary.ContainsKey(key))
			{
				keysDictionary[key].Cancel(); // Инвалидация предыдущих данных для текущего элемента с ключем key
				keysDictionary.Remove(key, out _);
			}
			keysDictionary.TryAdd(key, value);
		}

		public void RemoveItem(string key) //Нужно проверить, возможно сам MemoryCache не придется дергать
		{
			if (keysDictionary.ContainsKey(key))
			{
				keysDictionary[key].Cancel();
				keysDictionary.Remove(key, out _);
			}
		}
	}
}
