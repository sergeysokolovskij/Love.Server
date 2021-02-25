using System;
using System.Collections.Generic;
using System.Text;

namespace Api.Utils
{
	public class CommonConstants
	{
		public static char[] SymbholsToGenereteString = new char[] { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'j', 
			'k', 'l', 'm', 'n', 'o', 'p', 'q', 
			'r', 's', 't', 'y', 'w', 'x', 'y', 
			'z', '1', '2', '3', '4' ,'5', '6',
			'7', '8', '9'};

		public const string PublicKeyHeader = "X-PUBLIC-RSA-KEY";
		public const string UniqueClaimName = "UniqueParameter";
	}
}
