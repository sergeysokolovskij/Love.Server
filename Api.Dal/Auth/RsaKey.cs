using Api.DAL;
using System;
using System.Collections.Generic;
using System.Text;

namespace Api.Dal
{
	public class RsaKey
	{
		public long Id { get; set; }
		public string PrivateKey { get; set; }
		public string PublicKey { get; set; }
		public string UserId { get; set; }
		public User User { get; set; }
	}
}
