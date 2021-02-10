using System;
using System.Collections.Generic;
using System.Text;

namespace Api.Dal.Dev
{
	public class UserAccount : IBaseEntity
	{
		public string Id { get; set; }
		public string Login { get; set; }
		public string Password { get; set; }
		public DateTime Created { get; set; } = DateTime.Now;
		public DateTime Updated { get; set; }
	}
}
