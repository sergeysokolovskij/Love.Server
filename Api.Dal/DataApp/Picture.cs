using Api.DAL;
using System;
using System.Collections.Generic;
using System.Text;

namespace Api.Dal
{
	public class Picture : IBaseEntity
	{
		public long Id { get; set; }
		public User User { get; set; }
		public string UserId { get; set; }
		public string Path { get; set; }
		public DateTime Data { get; set; }
		public DateTime Created { get; set; }
		public DateTime Updated { get; set; } = DateTime.Now;
	}
}
