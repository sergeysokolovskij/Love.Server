using Api.Dal;
using Api.Models.Options;
using Api.Provider;
using Api.Utils;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Quartz;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Api.Services.Quartz
{
	public class ConnectionCleanerJob 
	{
		private readonly IConnectionProvider connectionProvider;
		private readonly IOptions<TokenLifeTimeOptions> tokenLifeTimeOptions;

		private readonly ILogger<ConnectionCleanerJob> logger; 

		public ConnectionCleanerJob(IConnectionProvider connectionProvider,
			ILoggerFactory loggerFactory)
		{
			this.connectionProvider = connectionProvider;
			logger = loggerFactory.CreateLogger<ConnectionCleanerJob>();
		}

		public async Task Execute(IJobExecutionContext context)
		{
			logger.LogInformation("ConnectionCleaner job was started");
			int countInvalidateConnections = 0;

			var connections = await connectionProvider.GetModelsBySearchPredicate(x => x.Created.AddMinutes(tokenLifeTimeOptions.Value.AccessTokenLifeTime) < DateTime.Now);

			if (connections.IsListNull())
			{
				logger.LogInformation("Not connecitons to be clear");
			}

			await connectionProvider.DeleteAsync(connections);	
		}
	}
}
