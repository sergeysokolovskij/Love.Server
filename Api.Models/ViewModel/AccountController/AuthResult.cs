using Newtonsoft.Json;
using System.Collections.Generic;

namespace Shared
{
	public class AuthResult
	{
		[JsonProperty("userName")]
		public string UserName { get; set; }
		[JsonProperty("roles")]
		public List<string> Roles { get; set; }
		[JsonProperty("accessToken")]
		public string AccessToken { get; set; }
		public string RefreshToken { get; set; }
	}
}
