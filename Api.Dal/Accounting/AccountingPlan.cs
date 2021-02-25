using System;
using System.Collections.Generic;
using System.Text;

namespace Api.Dal.Accounting
{
	public class AccountingPlan
	{
		public long Id { get; set; }
		public string Name { get; set; }
		public long? ParentId { get; set; }
	}
}
