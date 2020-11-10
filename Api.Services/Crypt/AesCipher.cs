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

namespace Api.Services.Crypt
{
	public interface IAesCipher
	{
		string Crypt(string cypherName, string value);
		string DecryptString(string cypherName, string value);
	}

	public sealed class AesCipher : IAesCipher
	{
		private readonly IServiceProvider serviceProvider;
		Aes cipher;

		private Dictionary<string, byte[]> cypherSecrets = new Dictionary<string, byte[]>();

		public AesCipher(IServiceProvider serviceProvider)
		{
			cipher = new AesManaged
			{
				KeySize = 256,
				BlockSize = 128,
				Padding = PaddingMode.ISO10126,
				Mode = CipherMode.CBC
			};
			this.serviceProvider = serviceProvider;
			Init();
		}

		public string Crypt(string cypherName, string value)
		{
			var IV = GenerateIV();
			var cryptoTransform = cipher.CreateEncryptor(cypherSecrets[cypherName], IV);
			byte[] textInBytes = Encoding.UTF8.GetBytes(value);
			byte[] result = cryptoTransform.TransformFinalBlock(textInBytes, 0, textInBytes.Length);

			byte[] resultPlusIV = new byte[result.Length + IV.Length];

			result.CopyTo(resultPlusIV, 0);
			IV.CopyTo(resultPlusIV, result.Length);

			return resultPlusIV.ToUrlSafeBase64();
		}

		public string DecryptString(string cypherName ,string value)
		{
			byte[] textInBytes = value.FromUrlSafeBase64();
			byte[] IV = new byte[16];

			Array.Copy(textInBytes, textInBytes.Length - IV.Length, IV, 0, IV.Length);

			var cryptoTransform = cipher.CreateDecryptor(cypherSecrets[cypherName], IV);

			byte[] result = cryptoTransform.TransformFinalBlock(textInBytes, 0, textInBytes.Length - IV.Length);

			return result.FromBytesToString();
		}

		private byte[] GenerateIV()
		{
			var result = new byte[16];
			CryptoRandomizer.CryptoProvider.GetBytes(result);
			return result;
		}

		void Init()
		{
			cypherSecrets.Clear();
			using (var scope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
			{
				var cypherProvider = scope.ServiceProvider.GetRequiredService<ICypherProvider>();
				cypherSecrets = cypherProvider.GetAllCyphers();
			}
		}
	}
}
