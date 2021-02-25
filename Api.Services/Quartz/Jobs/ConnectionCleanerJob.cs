using Api.Dal;
using Api.Models.Options;
using Api.Provider;
using Api.Utils;
using Microsoft.Extensions.DependencyInjection;
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
	public class ConnectionCleanerJob : IJob
	{
		private readonly IServiceProvider serviceProvider;
		private readonly IOptions<TokenLifeTimeOptions> tokenLifeTimeOptions;
		private readonly ILogger<ConnectionCleanerJob> logger;

		private IConnectionProvider connectionProvider;


		public ConnectionCleanerJob(IServiceProvider serviceProvider,
			IOptions<TokenLifeTimeOptions> tokenLifeTimeOptions,
			ILoggerFactory loggerFactory)
		{
			this.serviceProvider = serviceProvider;
			this.tokenLifeTimeOptions = tokenLifeTimeOptions;
			logger = loggerFactory.CreateLogger<ConnectionCleanerJob>();
		}

		public async Task Execute(IJobExecutionContext context)
		{
			logger.LogInformation("ConnectionCleaner job was started");

			using var scope = serviceProvider.GetService<IServiceScopeFactory>().CreateScope();
			connectionProvider = scope.ServiceProvider.GetRequiredService<IConnectionProvider>();

			var connections = await connectionProvider.GetModelsBySearchPredicate(x => x.Created.AddMinutes(tokenLifeTimeOptions.Value.AccessTokenLifeTime) < DateTime.Now);

			if (connections.IsListNull())
			{
				logger.LogInformation("Not connecitons to be clear");
			}

			await connectionProvider.DeleteAsync(connections);
			logger.LogInformation($"{connections.Count} was cleared from DB");
		}
	}
}
