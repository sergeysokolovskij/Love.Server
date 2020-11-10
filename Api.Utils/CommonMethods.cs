using System;
using System.Collections.Generic;
using System.Text;

namespace Api.Utils
{
	public static class CommonMethods
	{
		public static string GenerateRandomString(int length)
		{
			string result = string.Empty;

			var random = new Random();

			int rangeLength = CommonConstants.SymbholsToGenereteString.Length;

			for (int i = 0; i < length; i++)
				result += CommonConstants.SymbholsToGenereteString[random.Next(0, rangeLength)];
			return result;
		}
		public static bool IsListNotNull<T>(this IList<T> lst)
		{
			if (lst != null && lst.Count > 0)
				return true;
			return false;
		}

		public static bool IsListNull<T>(this IList<T> lst)
		{
			if (lst == null || lst.Count == 0)
				return true;
			return false;
		}
	}
}
