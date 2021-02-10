using System;
using System.Collections.Generic;
using System.Text;
using Api.Dal;
using Api.Dal.Accounting;
using Api.Dal.Auth;
using Microsoft.AspNetCore.Identity;

namespace Api.DAL
{
	public class User : IdentityUser, IBaseEntity
	{
		public long StrongKeyId { get; set; }
		public long ProfileId { get; set; }
		public Profile Profile { get; set; }
		public StrongKey StrongKey { get; set; }
		public List<LongSession> LongSessions { get; set; }
		public List<PushTask> PushTasks { get; set; }
		public List<Like> Likes { get; set; }
		public List<Visit> Visits { get; set; }
		public List<Picture> Pictures { get; set; }
		public List<Cordinate> Cordinates { get; set; }
		public List<RsaKey> RsaKeys { get; set; }
		public List<UserToken> UserTokens { get; set; }
		public List<Session> Sessions { get; set; }
		public List<Connection> Connections { get; set; }
		public List<AccountingRecord> AccountingRecords { get; set; }
		public DateTime Created { get; set; }
		public DateTime Updated { get; set; } = DateTime.Now;
		public User()
		{
			LongSessions = new List<LongSession>();
		}
	}
}
