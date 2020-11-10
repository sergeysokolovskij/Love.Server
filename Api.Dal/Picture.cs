using Api.DAL;
using System;
using System.Collections.Generic;
using System.Text;

namespace Api.Dal
{
	public class Picture
	{
		public long Id { get; set; }
		public User User { get; set; }
		public long UserId { get; set; }
		public string Path { get; set; }
	}
}
