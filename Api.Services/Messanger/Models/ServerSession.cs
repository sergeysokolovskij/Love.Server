using System;
using System.Collections.Generic;
using System.Text;

namespace Api.Services.Messanger.Models
{
	public class ServerSession
	{
		public string PrivateSenderKey { get; set; } // используется для расшифровки aes ключа и соли отправителя
		public string PublicReceiverKey { get; set; } //используется для шифровки aes ключа перед отправкой его на клиент
	}
}
