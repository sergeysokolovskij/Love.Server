using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

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


		public static int ToInt(this string str)
		{
			return Convert.ToInt32(str);
		}

		public static async Task<string> ToJson(this object obj)
		{
			using (var memStream = new MemoryStream())
			{
				await JsonSerializer.SerializeAsync(memStream, obj, obj.GetType());
				memStream.Position = 0;

				using (var streamReader = new StreamReader(memStream))
				{
					return await streamReader.ReadToEndAsync();
				}
			}
		}

		public static async Task<T> FromJson<T>(this string str)
		{
			using (var memStream = new MemoryStream())
			{
				byte[] buffer = Encoding.UTF8.GetBytes(str);
				await memStream.WriteAsync(buffer, 0, buffer.Length);
				memStream.Position = 0;

				var result = await JsonSerializer.DeserializeAsync<T>(memStream);

				return result;
			}
		}
	
		public static T GetPropertyValue<T>(this object o, string propertyName)
		{
			return (T)o.GetType().GetProperty(propertyName).GetValue(o, null);
		}

		public static byte[] ObjectToBytes(this object obj)
		{
			BinaryFormatter bf = new BinaryFormatter();
			using (MemoryStream memStream = new MemoryStream())
			{
				bf.Serialize(memStream, obj);
				byte[] result = memStream.ToArray();

				return result;
			}
		}
	}
}
