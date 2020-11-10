using System;
using System.Collections.Generic;
using System.Text;

namespace Api.Models.Exceptions
{
	public class CacheException : Exception
	{
		public CacheException(string message) : base(message)
		{
		}
	}
}
