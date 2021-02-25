using Api.DAL;
using System;
using System.Collections.Generic;
using System.Text;

namespace Api.Dal
{
	public class Connection : IBaseEntity
	{
		public long Id { get; set; }
		public string UserId { get; set; }
		public string ConnectionId { get; set; }
		public string SessionId { get; set; }
		public User User { get; set; }
		public DateTime Created { get; set; } 
		public DateTime Updated { get; set; }
	}
}
