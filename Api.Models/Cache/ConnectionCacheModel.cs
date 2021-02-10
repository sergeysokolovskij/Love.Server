using System;
using System.Collections.Generic;
using System.Text;

namespace Api.Models.Cache
{
	public class ConnectionCacheModel
	{
		public string UserId { get; set; }
		public Dictionary<string ,string> Connections { get; set; }
	}
}
