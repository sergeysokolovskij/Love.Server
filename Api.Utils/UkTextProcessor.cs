using System;
using System.Collections.Generic;
using System.Text;

namespace Api.Utils
{
	public class UkTextProcessor
	{
		public static string Encode(string input)
		{
			if (!string.IsNullOrEmpty(input))
				return input.Replace("ґ", "04xbbb").Replace("ї", "03xddd").Replace("і", "03xbbb").Replace("Є", "03xwww")
					.Replace("ґ".ToUpper(), "04xbbB").Replace("ї".ToUpper(), "03xddD").Replace("і".ToUpper(), "03xbbB").Replace("Є".ToLower(), "03xWww");
			return input;
		}
			
		public static string Decode(string input)
		{
			if (!string.IsNullOrEmpty(input))
				return input.Replace("04xbbb", "ґ").Replace("03xddd", "ї").Replace("03xbbb", "і").Replace("03xwww", "Є")
					.Replace("04xbbB", "ґ".ToUpper()).Replace("03xddD", "ї".ToUpper()).Replace("03xbbB", "і".ToUpper()).Replace("03xWww", "Є".ToLower());
			return input;
		}

		public static Dictionary<string, string> DecodeDictionary(Dictionary<string, string> old)
		{
			var result = new Dictionary<string, string>();
			foreach (var p in old)
				result.Add(Decode(p.Key), Decode(p.Value));

			return result;
		}
	}
}
