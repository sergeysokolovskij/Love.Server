using Api.DAL;
using Api.Provider.Accounting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Api.Services.Accounting
{
	public interface IAccountingService
	{

	}
	public class AccountingService : IAccountingService
	{
		private readonly IFlowProvider flowProvider;

		public AccountingService(IFlowProvider flowProvider)
		{
			this.flowProvider = flowProvider;
		}


		public async Task CreateFlowAsync(User fromUser)
		{
				
		}
	}
}
