using Microsoft.Extensions.DependencyInjection;
using Api.DAL;
using Api.Providers;
using Apis.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Api.Utils;
using System.Threading.Tasks;

namespace Api.Services.Crypt
{
	public interface IAesCipher
	{
		string Crypt(string secret, string value);
		string Decrypt(string secret, string value);
		Task<string> Crypt(long cypherId, string value);
		Task<string> DecryptString(long cypherId, string value);
		string GenerateAesKey();
	}

	public sealed class AesCipher : IAesCipher
	{
		private readonly ICypherProvider cypherProvider;

		Aes cipher;

		public AesCipher(ICypherProvider cypherProvider)
		{
			cipher = new AesManaged
			{
				KeySize = 256,
				BlockSize = 128,
				Padding = PaddingMode.ISO10126,
				Mode = CipherMode.CBC
			};
			this.cypherProvider = cypherProvider;
		}

		public async Task<string> Crypt(long cypherId, string value)
		{
			var cypherSecret = await cypherProvider.GetModelBySearchPredicate(x => x.Id == cypherId);

			var IV = GenerateIV();

			var cryptoTransform = cipher.CreateEncryptor(cypherSecret.Secret, IV);
			byte[] textInBytes = Encoding.UTF8.GetBytes(value);
			byte[] result = cryptoTransform.TransformFinalBlock(textInBytes, 0, textInBytes.Length);

			byte[] resultPlusIV = new byte[result.Length + IV.Length];

			result.CopyTo(resultPlusIV, 0);
			IV.CopyTo(resultPlusIV, result.Length);

			return resultPlusIV.ToUrlSafeBase64();
		}

		public string Crypt(string secret, string value)
		{
			byte[] secretBuffer = secret.FromUrlSafeBase64();
			byte[] IV = GenerateIV();

			var cryptoTransform = cipher.CreateEncryptor(secretBuffer, IV);
			byte[] textInBytes = Encoding.UTF8.GetBytes(value);

			byte[] result = cryptoTransform.TransformFinalBlock(textInBytes, 0, textInBytes.Length);
			byte[] resultPlusIV = new byte[result.Length + IV.Length];

			result.CopyTo(resultPlusIV, 0);
			IV.CopyTo(resultPlusIV, result.Length);

			return resultPlusIV.ToUrlSafeBase64();
		}

		public string Decrypt(string secret, string value)
		{
			byte[] secretBuffer = secret.FromUrlSafeBase64();
			byte[] IV = new byte[16];
			byte[] textInBytes = value.FromUrlSafeBase64();

			Array.Copy(textInBytes, textInBytes.Length - IV.Length, IV, 0, IV.Length);
			var cryptoTransform = cipher.CreateDecryptor(secretBuffer, IV);

			byte[] result = cryptoTransform.TransformFinalBlock(textInBytes, 0, textInBytes.Length - IV.Length);
			return Encoding.UTF8.GetString(result);
		}

		public async Task<string> DecryptString(long cypherId ,string value)
		{
			var cypherSecret = await cypherProvider.GetModelBySearchPredicate(x => x.Id == cypherId);

			byte[] textInBytes = value.FromUrlSafeBase64();
			byte[] IV = new byte[16];

			Array.Copy(textInBytes, textInBytes.Length - IV.Length, IV, 0, IV.Length);

			var cryptoTransform = cipher.CreateDecryptor(cypherSecret.Secret, IV);

			byte[] result = cryptoTransform.TransformFinalBlock(textInBytes, 0, textInBytes.Length - IV.Length);

			return Encoding.UTF8.GetString(result);
		}

		public string GenerateAesKey()
		{
			var result = CryptoRandomizer.GenerateSecurityKey(32);
			return result.ToUrlSafeBase64();
		}

		private byte[] GenerateIV()
		{
			var result = new byte[16];
			CryptoRandomizer.CryptoProvider.GetBytes(result);
			return result;
		}
	}
}
