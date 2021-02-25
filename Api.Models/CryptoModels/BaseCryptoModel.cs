using System;
using System.Collections.Generic;
using System.Text;

namespace Api.Models.CryptoModels
{
	public class BaseCryptoModel
	{
		public string CryptedAesKey { get; set; }
		public string Signature { get; set; }
		public string CryptedData { get; set; }
	}
}
