using Api.Dal.Messanger;
using Api.DAL;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;

namespace Api.Dal
{
	public class Message : IBaseEntity
	{
		public Guid Id { get; set; }
		public string MessageId { get; set; }
		public string MessageText { get; set; } // криптотекст. Текст сообщения хранится в зашифрованном виде
		public string ReceiverId { get; set; }
		public string SenderId { get; set; }
		public long DialogId { get; set; }
		public Dialog Dialog { get; set; }
		public long CypherId { get; set; }
		public DateTime Created { get; set; }
		public DateTime Updated { get; set; }
		public Cypher Cypher { get; set; }
		public bool IsReaded { get; set; }
	}
}
