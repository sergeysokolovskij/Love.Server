using Api.DAL;
using System;
using System.Collections.Generic;
using System.Text;

namespace Api.Dal.Auth
{
	public class StrongKey : IBaseEntity
	{
		public long Id { get; set; }
		public string UserId { get; set; }
		public long CypherId { get; set; }
		public User User { get; set; }
		public Cypher Cypher { get; set; }
		public DateTime Created { get; set; }
		public DateTime Updated { get; set; }
	}
}
