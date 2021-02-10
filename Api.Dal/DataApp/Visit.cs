using Api.DAL;
using System;
using System.Collections.Generic;
using System.Text;

namespace Api.Dal
{
	public class Visit : IBaseEntity
	{
		public long Id { get; set; }
		public long UserId { get; set; }
		public User User { get; set; }
		public DateTime Created { get; set; }
		public DateTime Updated { get; set; }
	}
}
