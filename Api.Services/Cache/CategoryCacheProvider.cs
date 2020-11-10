using Microsoft.Extensions.Logging;
using Api.Services.Cache.Models;
using Apis.Utils;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using Api.Utils;

namespace Api.Services.Cache
{
	public interface ICategoryCacheProvider
	{
		void AddItemToCategoryCache(CacheItem cacheItem, CategoryCache cacheCategory);
		//void InvalidateCategoryCache(CategoryCache cacheCategory,
		//	CategoryCache subCategory = null,
		//	int count = 0);
	}
	public class CategoryCacheProvider : ICategoryCacheProvider
	{
		private readonly List<CategoryCache> CategoryCaches = new List<CategoryCache>();
		private List<int> CategoryCachesIds = new List<int>();

		private readonly ICacheTokenProvider cacheTokenProvider;
		private readonly ILogger<CategoryCacheProvider> logger;
		public CategoryCacheProvider(ILoggerFactory loggerFactory,
			ICacheTokenProvider cacheTokenProvider)
		{
			this.logger = loggerFactory.CreateLogger<CategoryCacheProvider>();
			this.cacheTokenProvider = cacheTokenProvider;
		}

		public void AddItemToCategoryCache(CacheItem cacheItem, CategoryCache cacheCategory)
		{
			if (CategoryCachesIds.Contains(cacheCategory.Id))
				throw new Exception($"Category with {cacheCategory.Id} does not exist");
			foreach (var category in CategoryCaches)
			{
				if (category.Id != cacheCategory.Id)
					continue;
				if (category.CacheItemsKeys.Contains(cacheItem.Key))
					throw new Exception($"Category cache '{cacheCategory.Id}' has element with key {cacheItem.Key}");
				category.CacheItemsKeys.Add(cacheItem.Key);

				break;
			}
		}

		public void InvalidateCategoryCache(CategoryCache cacheCategory,
			CategoryCache previous = null,
			int count = 0,
			bool isPreviousPrimitive = false
		)
		{
			if (!CategoryCachesIds.Contains(cacheCategory.Id))
				throw new Exception($"Category with {cacheCategory.Id} does not exist");

			if (InvalidateCategoryCachePrimitive(cacheCategory)) //Дерево не имеет листьев
				return;

			if (count < cacheCategory.SubCategories.Count)
			{
				CategoryCache temp;
				if (previous == null) // Понимаем что мы на первом шаге или лист был примитивом
				{
					temp = cacheCategory.SubCategories[count];
					bool isPrimitive = InvalidateCategoryCachePrimitive(temp);

					if (isPrimitive)
						InvalidateCategoryCache(cacheCategory, previous, ++count, true);
				}
			}
		}
		private bool InvalidateCategoryCachePrimitive(CategoryCache cacheCategory)
		{
			if (cacheCategory.SubCategories.IsListNotNull())
			{
				foreach (var cacheItem in cacheCategory.CacheItemsKeys)
					cacheTokenProvider.RemoveItem(cacheItem);
				return true;
			}
			return false;
		}
	}
}
