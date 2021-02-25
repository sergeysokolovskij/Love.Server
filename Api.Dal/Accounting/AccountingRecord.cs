using Api.DAL;
using System;
using System.Collections.Generic;
using System.Text;

namespace Api.Dal.Accounting
{
 	public class AccountingRecord
	{
		public long Id { get; set; }
		public string UserId { get; set; }
		public long AccountingPlanId { get; set; }
		public string SubConto1 { get; set; } // доп разрезы учета
		public string SubConto2 { get; set; }
		public User User { get; set; }
		public AccountingPlan AccountingPlan { get; set; }
	}
}
