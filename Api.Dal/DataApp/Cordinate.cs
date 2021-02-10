using Api.DAL;
using System;
using System.Collections.Generic;
using System.Text;

namespace Api.Dal
{
	public class Cordinate : IBaseEntity
	{
		public long Id { get; set; }
		public float Longtitude { get; set; }
		public float Lautitude { get; set; }
		public string UserId { get; set; }
		public User User { get; set; }
		public DateTime Created { get; set; }
		public DateTime Updated { get; set; }
	}
}
