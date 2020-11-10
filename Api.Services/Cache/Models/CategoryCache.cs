using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;

namespace Api.Services.Cache.Models
{
	//Категория кеша представляет собой "хранилище" связных элементов, для подальшей удобной инвалидации этих данных
	public class CategoryCache
	{
		public int Id { get;}
		public string Name { get;}
		public string UrlPath { get; private set; } = "";
		public int? ParrentId { get; private set; }
		public CategoryCache Parrent { get; private set; } = null;
		public ImmutableList<CategoryCache> SubCategories { get; set; }
		public List<string> CacheItemsKeys { get; set; }

		public CategoryCache()
		{
		}
	}
}
