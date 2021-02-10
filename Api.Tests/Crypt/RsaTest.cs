using Api.Services.Crypt;
using Api.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Xunit;
using Assert = Xunit.Assert;

namespace Api.Tests
{
	[TestClass]
	public class RsaTest
	{
		private Mock<IRsaCypher> rsaCypherMock;

		[Fact]
		public void Crypt()
		{
			string dataToCrypt = CryptoRandomizer.GenerateSecurityKey(16).ToUrlSafeBase64();
			rsaCypherMock = new Mock<IRsaCypher>();

			rsaCypherMock.Setup(x => x.GenerateKeys()).Returns<(string, string)>(result => result);
			var keys = rsaCypherMock.Object.GenerateKeys();

			string cryptedText = rsaCypherMock.Object.Crypt(keys.publicKey, dataToCrypt);
			string decryptedText = rsaCypherMock.Object.Decrypt(keys.privateKey, cryptedText);

			Assert.Equal(decryptedText, dataToCrypt);
		}
	}
}
