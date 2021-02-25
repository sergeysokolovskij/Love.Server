using System;
using System.Collections.Generic;
using System.Text;

namespace Api.Models.CacheModels
{
	public class BaseCacheModel
	{
		public string Id { get; set; }
		public string EntityType { get; set; }
		public int CountElementsInDb { get; set; } // на случай, если у нас коллекция хранится в кеше
		public DateTime CreatedElement { get; set; }
	}
}
