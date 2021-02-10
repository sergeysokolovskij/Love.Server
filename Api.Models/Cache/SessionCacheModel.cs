using System;
using System.Collections.Generic;
using System.Text;

namespace Api.Models.Cache
{
	public class SessionCacheModel
	{
		public string UserId { get; set; }
		public string ClientPublicKey { get; set; }
		public string ServerPublicKey { get; set; }
		public string ServerPrivateKey { get; set; }
		public string SessionId { get; set; }
	}
}
