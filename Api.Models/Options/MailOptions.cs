using System;
using System.Collections.Generic;
using System.Text;

namespace Api.Models.Options
{
	public class MailOptions
	{
		public string Login { get; set; }
		public string Password { get; set; }
		public string DisplayedName { get; set; }
		public string To { get; set; }
	}
}
