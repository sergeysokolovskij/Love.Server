using Api.DAL;
using System;
using System.Collections.Generic;
using System.Text;

namespace Api.Dal
{
	// своего рода "задача". нужно для вывода уведомлений о каком-то действии
	public class PushTask : IBaseEntity
	{
		public long Id { get; set; }
		public string UserId { get; set; }
		public User User { get; set; }
		public string Message { get; set; }
		public bool IsCompleted { get; set; }
		public DateTime Created { get; set; }
		public DateTime Updated { get; set; } = DateTime.Now;
	}
}
