using System;
using System.Collections.Generic;
using System.Text;

namespace Api.Models.Exceptions
{
	public class BaseException : Exception
	{
		public BaseException(string message = null, Exception ex = null) : base(message, ex) { }
	}
}
