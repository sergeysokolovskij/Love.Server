using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Api.DAL;
using Api.DAL.Base;
using Api.Models.Messanger;
using Api.Provider.Cache;
using Api.Provider.Messanger;
using Api.Providers;
using Api.Services.Brocker;
using Api.Services.Cache;
using Api.Services.Cache.CacheServices;
using Api.Services.Crypt;
using Api.Services.Messanger;
using Api.Utils;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Api
{
	public class Program
	{
		public static async Task Main(string[] args)
		{
			IHostBuilder hostBuilder = CreateHostBuilder(args);

			Configuration.InitialConfig();

			IHost host = null;

			host = hostBuilder.Build();

			var logger = host.Services.GetRequiredService<ILoggerFactory>().CreateLogger<Program>();
			logger.LogInformation("Try to run app...");

			var scope = host.Services.GetRequiredService<IServiceScopeFactory>().CreateScope();

			try
			{
				var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationContext>();

				if (Configuration.IsConfigOkey)
				{
					Configuration.CheckAccessToDb(dbContext);
					if (Configuration.IsConnectToDbSuccess)
					{
						try
						{
							dbContext.Migrate();
						}
						catch (Exception ex)
						{
							Console.WriteLine(ex.Message);
						}
					}
					else
						Console.WriteLine("Невозможно подключится к бд");
				}
				else
				{
					Console.WriteLine(Configuration.ErrorMessage);
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}

			var brocker = scope.ServiceProvider.GetRequiredService<IBrockerService>();
			brocker.Init();

            var subscriber = scope.ServiceProvider.GetRequiredService<ISubscriber>();
			await subscriber.SetAllSubscribersAsync();

			logger.LogInformation("App runing sucess!");
			await host.RunAsync();
		}
		public static IHostBuilder CreateHostBuilder(string[] args)
		{
			return Host.CreateDefaultBuilder(args)
				.ConfigureWebHostDefaults(webBuilder =>
				{
					webBuilder.UseStartup<Startup>();

					webBuilder.ConfigureAppConfiguration((builderContext, config) =>
					{
						string env = builderContext.HostingEnvironment.EnvironmentName;
						string dbSettingsFile = $"{PathConstants.ConfigFolderName}/{PathConstants.dbConfig}.{env}.json";
						string authSettingsFile = Path.Combine(PathConstants.ConfigFolderName, PathConstants.authConfigName);
						string corsConfigFile = Path.Combine(PathConstants.ConfigFolderName, PathConstants.corsConfigName);
						string globalConfigFile = Path.Combine(PathConstants.ConfigFolderName, PathConstants.globalConfig);

						config.AddJsonFile(dbSettingsFile);
						config.AddJsonFile(authSettingsFile);
						config.AddJsonFile(corsConfigFile);
						config.AddJsonFile(globalConfigFile);
					});
				})
				.ConfigureLogging((logger) => 
				{
					logger.AddConsole();
					logger.AddFile("logs/log.txt");
					logger.AddFilter("Microsoft.AspNetCore.SignalR", LogLevel.Debug);
					logger.AddFilter("Microsoft.AspNetCore.Http.Connections", LogLevel.Debug);
				});
		}
	}
}
