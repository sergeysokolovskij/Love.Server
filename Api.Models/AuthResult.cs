using Newtonsoft.Json;

namespace Shared
{
	public class AuthResult
	{
		[JsonProperty("userName")]
		public string UserName { get; set; }
		[JsonProperty("role")]
		public string Role { get; set; }
		[JsonProperty("accessToken")]
		public string AccessToken { get; set; }
		public string RefreshToken { get; set; }
	}
}
