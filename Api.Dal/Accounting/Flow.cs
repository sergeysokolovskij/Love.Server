using System;
using System.Collections.Generic;
using System.Text;

namespace Api.Dal.Accounting
{
	public class Flow
	{
		public long Id { get; set; }
		public Guid PostingId { get; set; }
		public AccountingRecord From { get; set; }
		public AccountingRecord To { get; set; }
		public AccountingComment AccountingComment { get; set; }
		public long? FromId { get; set; }
		public long? ToId { get; set; }
		public long? AccountingCommentId { get; set; }
		public long AccountingPlanId { get; set; }
		public string FollowInformation { get; set; }
		public DateTime Created { get; set; } = DateTime.Now;
	}
}
