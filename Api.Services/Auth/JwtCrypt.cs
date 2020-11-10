using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Text;

namespace Api.Services.Auth
{
	public interface IJwtEncryptingEncodingKey
	{
		string SigningAlgorithm { get; }
		string EncryptingAlgorithm { get; }
		SecurityKey GetKey();
	}

	public interface IJwtEncryptingDecodingKey
	{
		SecurityKey GetKey();
	}

	public class JwtCrypt : IJwtEncryptingEncodingKey, IJwtEncryptingDecodingKey
	{
		private readonly SymmetricSecurityKey secretKey;

		public JwtCrypt(string key)
		{
			secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
		}
		public string SigningAlgorithm { get; } = JwtConstants.DirectKeyUseAlg;
		public string EncryptingAlgorithm { get; } = SecurityAlgorithms.Aes256CbcHmacSha512;
		public SecurityKey GetKey() => secretKey;
	}
}
