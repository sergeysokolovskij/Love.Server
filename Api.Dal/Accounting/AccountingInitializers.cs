using System;
using System.Collections.Generic;
using System.Text;

namespace Api.Dal.Accounting
{
	public class AccountingInitializers
	{
		public static List<AccountingPlan> GetAccountingsPlan()
		{
			var logginAccounting = new AccountingPlan()
			{
				Id = 1,
				Name = "LoginTime",
				ParentId = null
			};
			var logoutAccounting = new AccountingPlan()
			{
				Id = 2,
				Name = "LogoutTime",
				ParentId = null
			};

			return new List<AccountingPlan>()
			{
				logginAccounting,
				logoutAccounting
			};
		}
	}
}
