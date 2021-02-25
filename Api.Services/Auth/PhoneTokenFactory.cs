using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Api.Services.Auth
{
	public class PhoneTokenFactory
	{
		public string GenerateToken()
        {
			string result = "";
			Random rnd = new Random();

			for (int i = 0; i < 6; i++)
				result += rnd.Next(0, 9);

			return result;
		}
		public async Task MakePhoneTokenAsync(string userId)
		{
			string token = GenerateToken();

		}
	}
}
