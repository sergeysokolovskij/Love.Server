using Api.Dal;
using Api.Dal.Auth;
using System;
using System.Collections.Generic;
using System.Text;

namespace Api.DAL
{
	public class LongSession : IBaseEntity
	{
		public long Id { get; set; }
		public string UserId { get; set; }
		public string Value { get; set; }
		public string FingerPrint { get; set; }
		public DateTime Created { get; set; }
		public DateTime Updated { get; set; }
		public DateTime ExpiresIn { get; set; }
		public User User { get; set; }
	}
}
