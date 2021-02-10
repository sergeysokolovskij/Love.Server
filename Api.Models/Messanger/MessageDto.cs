using System;
using System.Collections.Generic;
using System.Text;

namespace Api.Models.Messanger
{
	[Serializable]
	public class SignMessageDto
	{
		public string MessageId { get; set; }
		public string SenderId { get; set; }
		public string ReceiverId { get; set; }
		public string CryptedText { get; set; }
		public string CryptedAes { get; set; }
		public string SessionId { get; set; }
		public DateTime Created { get; set; }
	}

	
	public class MessageDto
	{
		public SignMessageDto Message { get; set; }
		public string Sign { get; set; }
	}

	public class DecryptedMessageDto
	{
		public string MessageId { get; set; }
		public string SenderId { get; set; }
		public string ReceiverId { get; set; }
		public string CryptedText { get; set; }
		public string Aes { get; set; }
	}

	[Serializable]
	public class MessageBuildDto
	{
		public string MessageId { get; set; }
		public string SenderId { get; set; }
		public string ReceiverId { get; set; }
		public string Text { get; set; }
		public string Aes { get; set; }
		public string SessionId { get; set; }
	}
}
