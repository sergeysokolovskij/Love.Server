using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Text;

namespace Api.Services.Auth
{
	//ключ для создания цифровой подписи
	public interface IJwtSigningEncodingKey
	{
		string SignInAlgorithm { get; }
		SecurityKey GetKey();
	}
	public interface IJwtSigningDecodingKey
	{
		SecurityKey GetKey();
	}

	public class SignInSymmetricKey : IJwtSigningEncodingKey, IJwtSigningDecodingKey
	{
		private readonly SymmetricSecurityKey secretKey;

		public string SignInAlgorithm { get; } = SecurityAlgorithms.HmacSha256;

		public SignInSymmetricKey(string key)
		{
			secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
		}

		public SecurityKey GetKey() => secretKey;
	}
}
