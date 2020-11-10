using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Api.Services.Crypt
{
	public interface ICipherHelper
	{
	}
	public class CiperHelper : ICipherHelper
	{
		private readonly IAesCipher aesCipher;
		private readonly IConfiguration configuration;

		public CiperHelper(IAesCipher aesCipher, 
			IConfiguration configuration)
		{
			this.aesCipher = aesCipher;
			this.configuration = configuration;
		}
	}
}
