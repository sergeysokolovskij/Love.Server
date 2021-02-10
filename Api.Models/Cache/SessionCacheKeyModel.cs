using System;
using System.Collections.Generic;
using System.Text;

namespace Api.Models.Cache
{
	public class SessionCacheKeyModel
	{
		public string SessionId { get; set; }
		public string UserId { get; set; }
	}
}
