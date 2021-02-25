using Api.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Api.Services.Crypt
{
	public interface IRsaCypher
	{
		string Crypt(string rsaPublicKey, string dataToCrypt);
		string Decrypt(string rsaPrivateKey, string dataToDecrypt);
		(string privateKey, string publicKey) GenerateKeys();
		string SignData(string rsaPrivateKey, byte[] buffer);
		bool VerifySignature(string rsaPublicKey, byte[] buffer, byte[] siggnedBuffer);
	}

	public class RsaCypher : IRsaCypher
	{
		private const string AlgoritmName = "SHA256";
		
		public string Crypt(string rsaPublicKey, string dataToCrypt)
		{
			var rsaPublicParameter = JsonConvert.DeserializeObject<RSAParameters>(rsaPublicKey);
			using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(2048))
			{
				rsa.ImportParameters(rsaPublicParameter);

				byte[] bytesToCrypt = dataToCrypt.FromUrlSafeBase64();
				byte[] buffer = rsa.Encrypt(bytesToCrypt, false);

				return buffer.ToUrlSafeBase64();
			}
		}

		public string Decrypt(string rsaPrivateKey, string dataToDecrypt)
		{
			byte[] bufferToDecrypt = dataToDecrypt.FromUrlSafeBase64();

			using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(2048))
			{
				var rsaPrivateParameter = JsonConvert.DeserializeObject<RSAParameters>(rsaPrivateKey);
				rsa.ImportParameters(rsaPrivateParameter);

				byte[] decryptedData = rsa.Decrypt(bufferToDecrypt, false);

				return decryptedData.ToUrlSafeBase64();
			}
		}

		public (string privateKey, string publicKey) GenerateKeys()
		{
			using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(2048))
			{
				RSAParameters publicKeyParameter = rsa.ExportParameters(false);
				RSAParameters privateKeyParameter = rsa.ExportParameters(true);

				string privateKey = JsonConvert.SerializeObject(privateKeyParameter);
				string publicKey = JsonConvert.SerializeObject(publicKeyParameter);

				return (privateKey, publicKey);
			}
		}

		public string SignData(string rsaPrivateKey ,byte[] buffer)
		{
			using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(2048))
			{
				RSAParameters privateKey = JsonConvert.DeserializeObject<RSAParameters>(rsaPrivateKey);
				rsa.ImportParameters(privateKey);

				byte[] signedHashValue = rsa.SignData(buffer, SHA1.Create());
				return signedHashValue.ToUrlSafeBase64();
			}
		}

		public bool VerifySignature(string rsaPublicKey, byte[] buffer, byte[] siggnedBuffer)
		{
			using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(2048))
			{
				RSAParameters publicKey = JsonConvert.DeserializeObject<RSAParameters>(rsaPublicKey);
				rsa.ImportParameters(publicKey);

				if (rsa.VerifyData(buffer, SHA1.Create(), siggnedBuffer))
					return true;
				return false;
			}
		}

		private RSACryptoServiceProvider GetRsaCryptoProvider()
		{
			RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
			rsa.KeySize = 512;

			return rsa;
		}
	}
}
