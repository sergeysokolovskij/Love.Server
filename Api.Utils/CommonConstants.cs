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
		
		public static Dictionary<string, Dictionary<int, string>> GetBuildStates()
		{
			var result = new Dictionary<string, Dictionary<int, string>>();
			result.Add("uk", new Dictionary<int, string>() 
			{ 
				 {1, "Новобудова" },
				 {2, "Вторинне житло" }
			});
			result.Add("ru", new Dictionary<int, string>() 
			{
				{1, "Новострой"},
				{2, "Вторичка" }
			});
			result.Add("en", new Dictionary<int, string>()
			{
				{1, "New building" },
				{2, "Resale" }
			});
			result.Add("bg", new Dictionary<int, string>()
			{
				{1, "Нова сграда"},
				{2, "Жилище" }
			});

			return result;
		}
	}
}
