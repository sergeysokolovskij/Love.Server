using Microsoft.Extensions.DependencyInjection;
using Api.Services.Auth;
using System;
using System.Collections.Generic;
using System.Text;

namespace Api.Services.Crypt
{
	public static class DiExtensions
	{
		public static void RegisterCyphers(this IServiceCollection serviceCollection)
		{
			serviceCollection.AddSingleton<IAesCipher, AesCipher>();
		}
	}
}
