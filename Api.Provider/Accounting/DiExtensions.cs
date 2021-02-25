using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Api.Provider.Accounting
{
	public static class DiExtensions
	{
		public static void RegisterAccounting(this IServiceCollection serviceCollection)
		{
			serviceCollection.AddScoped<IUserOnlineAccountingProvider, UserOnlineAccountingProvider>();
			serviceCollection.AddScoped<IFlowProvider, FlowProvider>();
		}
	}
}
