using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity;

namespace Api.DAL
{
	public class User : IdentityUser
	{
		public List<LongSession> LongSessions { get; set; }
		public User()
		{
			LongSessions = new List<LongSession>();
		}
	}
}
