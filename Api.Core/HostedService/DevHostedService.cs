using Api.DAL;
using Api.Provider;
using Api.Services.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Api.Core.HostedService
{
	public class DevHostedService : IHostedService
	{
		private readonly IServiceProvider serviceProvider;

		private readonly ILogger<DevHostedService> logger;

		public DevHostedService(
			IServiceProvider serviceProvider,
			ILoggerFactory loggerFactory)
		{
			this.serviceProvider = serviceProvider;
			this.logger = loggerFactory.CreateLogger<DevHostedService>();
		}

		private async Task RegisteredDevUsersAsync()
		{
			using var scope = serviceProvider.GetService<IServiceScopeFactory>().CreateScope();

			var userDevAccountProvider = scope.ServiceProvider.GetRequiredService<IUserDevAccountProvider>();
			var authService = scope.ServiceProvider.GetRequiredService<IAuthService>();

			var userAccounts = await userDevAccountProvider.GetAllAsync();

			foreach (var user in userAccounts)
			{
				if (await authService.IsUserExistAsync(user))
					continue;

				var createUser = new User()
				{
					UserName = user.Login,
					Email = user.Login
				};

				await authService.RegisterTestUsersAsync(createUser, user.Password);
			}
		}

		public async Task StartAsync(CancellationToken cancellationToken)
		{
			await RegisteredDevUsersAsync();
		}

		public Task StopAsync(CancellationToken cancellationToken)
		{
			return Task.CompletedTask;
		}
	}
}
