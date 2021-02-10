using Api.DAL;
using Newtonsoft.Json;
using System;

namespace Api.Dal.Auth
{
	public class Session : IBaseEntity
	{
		public long Id { get; set; }
		public string UserId { get; set; }
		public string SessionId { get; set; }
		public string ServerPrivateKey { get; set; }
		public string ServerPublicKey { get; set; }
		public string ClientPublicKey { get; set; }
		[JsonIgnore]
		public User User { get; set; }
		public DateTime Created { get; set; } = DateTime.Now;
		public DateTime Updated { get; set; }
	}
}
