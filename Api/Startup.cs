using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Api.Core.HostedService;
using Api.Core.Hubs.Messanger;
using Api.Models.Options;
using Api.Providers;
using Api.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ShopPlatform.Middlewhere;
using ShopPlatforms.Core.Hubs;

namespace Api
{
	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		public void ConfigureServices(IServiceCollection services)
		{
			services.OptionsConfigure(Configuration);

			services.AddAntiforgery(options => { options.HeaderName = "x-xsrf-token"; });
			services.Configure<CookiePolicyOptions>(options =>
			{
			});
			services.CorsConfiguration(Configuration);

			services.ConfigurAuthorization();
			services.AddLocalization();

			bool isDevelopment = Configuration["ASPNETCORE_ENVIRONMENT"] == "Development";

			if (!isDevelopment)
				services.ConfigureSecurity();

			services.ConfigurAuthentication(Configuration, isDevelopment);

			services.AddMvcCore()
				.AddApiExplorer()
				.AddAuthorization(options =>
				{
					options.AddPolicy("UserPolicy", opt =>
					{
						opt.RequireRole("User");
					});
					options.AddPolicy("AdminPolicy", opt => opt.RequireRole("Admin"));
				})
				.AddDataAnnotations()
				.AddNewtonsoftJson()
				.AddXmlSerializerFormatters();

			services.AddHttpContextAccessor();

			services.AddSignalR()
				.AddMessagePackProtocol();

			services.ConfigureDbContext(Configuration);

			services.RegisterProviders();
			services.RegisterServices(Configuration);

			services.AddRabbitMq(Configuration);

			services.AddLocalization();

			services.AddHostedService<DevHostedService>();
		}

		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}
			else
			{
				app.UseHsts();
			}

			app.SetCommonConfig();

			app.UseCookiePolicy();
			app.UseHttpsRedirection();

			app.UseForwardedHeaders(new ForwardedHeadersOptions
			{
				ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto //nginx
			});

			app.UseRouting();
			app.RegisterMiddleWheres();

			app.UseAuthentication();
			app.UseAuthorization();

			app.CorsConfiguration();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapHub<MessangerHub>("/messanger", options => options.Transports = HttpTransportType.WebSockets);
				endpoints.MapControllers();
			});

			//app.DbMigrationsEnable();
		}
	}
}
