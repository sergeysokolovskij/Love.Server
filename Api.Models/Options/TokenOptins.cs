using System;
using System.Collections.Generic;
using System.Text;

namespace Api.Models.Options
{
	public class TokenLifeTimeOptions
	{
		public int RefreshTokenLifeTime { get; set; }
		public int AccessTokenLifeTime { get; set; }
	}
	public class TokenOptions
	{
		public string Key { get; set; } // ключ для подписи access-token
		public string CypherKey { get; set; } //ключ для шифрования refresh-token (longsession)
	}
}
