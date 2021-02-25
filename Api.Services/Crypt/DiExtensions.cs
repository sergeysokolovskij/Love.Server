using Microsoft.Extensions.DependencyInjection;
using Api.Services.Auth;
using System;
using System.Collections.Generic;
using System.Text;
using Api.Services.Messanger;

namespace Api.Services.Crypt
{
	public static class DiExtensions
	{
		public static void RegisterCyphers(this IServiceCollection serviceCollection)
		{
			serviceCollection.AddScoped<IAesCipher, AesCipher>();
			serviceCollection.AddScoped<IRsaCypher, RsaCypher>();
			serviceCollection.AddScoped<IMessangerCryptor, MessangerCryptor>();
		}
	}
}
