using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Api.DAL.Base;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Api
{
	public class Program
	{
		public static void Main(string[] args)
		{
			IHostBuilder hostBuilder = CreateHostBuilder(args);

			Configuration.InitialConfig();

			IHost host = null;

			try
			{
				host = hostBuilder.Build();

				var scope = host.Services.GetRequiredService<IServiceScopeFactory>().CreateScope();
				var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationContext>();


				if (Configuration.IsConfigOkey)
				{
					Configuration.CheckAccessToDb(dbContext);
					if (Configuration.IsConnectToDbSuccess)
					{
						try
						{
							dbContext.Migrate();
							host.Run();
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
				dbContext.Dispose();
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
		}

		public static IHostBuilder CreateHostBuilder(string[] args)
		{
			return Host.CreateDefaultBuilder(args)
				.ConfigureWebHostDefaults(webBuilder =>
				{
					webBuilder.UseStartup<Startup>();
					webBuilder.UseUrls("");

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
				});
		}
	}
}
