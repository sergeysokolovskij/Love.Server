using System;
using System.Collections.Generic;
using System.Text;

namespace Api.Dal.Messanger
{
	public class Dialog : IBaseEntity
	{
		public long Id { get; set; }
		public string User1Id { get; set; }
		public string User2Id { get; set; }
		public DateTime Created { get; set; } = DateTime.Now;
		public DateTime Updated { get; set; }
	}
}
