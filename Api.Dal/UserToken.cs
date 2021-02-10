using Api.DAL;
using System;
using System.Collections.Generic;
using System.Text;

namespace Api.Dal
{
	public enum UserTokenType
	{
		PhoneToken
	}

	public class UserToken
	{
		public long Id { get; set; }
		public string UserId { get; set; }
		public string Value { get; set; }
		public User User { get; set; }
		public UserTokenType UserTokenType { get; set; }
	}
}
