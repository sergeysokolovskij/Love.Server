using System;
using System.Collections.Generic;
using System.Text;

namespace Api.Services.Cache.Models
{
	public class RequestOptions
	{
		public int? Page { get; set; } // запрашиваемая страница для кеша
		public string Sort { get; set; }
	}
}
