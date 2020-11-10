using Api.Services.Exceptions;
using Api.Services.Logs;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace ShopPlatforms.Core.Middlewhere
{
	public class ShopPlatformExceptionMiddlewhere
	{
		private readonly RequestDelegate next;
		private readonly ILogger<ShopPlatformExceptionMiddlewhere> logger;

		public ShopPlatformExceptionMiddlewhere(RequestDelegate next, 
			ILoggerFactory loggerFactory)
		{
			this.next = next;
			logger = loggerFactory?.CreateLogger<ShopPlatformExceptionMiddlewhere>() ??
				throw new ArgumentNullException(nameof(loggerFactory));
		}


		public async Task InvokeAsync(HttpContext context)
		{
			try
			{
				await next(context);
			}
			catch (OperationCanceledException)
			{
				context.Response.StatusCode = 400;
				logger.LogInformation("Request was canceled");
			}
			catch (ApiError err)
			{
				logger.LogError(err.Message);
				context.Response.StatusCode = 400;

				var json = JsonConvert.SerializeObject(err.ex, Formatting.None, new JsonSerializerSettings 
				{
					NullValueHandling = NullValueHandling.Ignore
				});
				context.Response.ContentType = "application/json";
				await context.Response.WriteAsync(json);
			}
			catch(Exception ex) 
			{
				logger.LogError(ex.Message);
				Logger.ErrorLog(ex);
				context.Response.StatusCode = 500;
				var error = ServerExceptions.ServerError(ex);

				context.Response.ContentType = "application/json";
				await context.Response.WriteAsync(JsonConvert.SerializeObject(error, Formatting.None, new JsonSerializerSettings()
				{
					NullValueHandling = NullValueHandling.Ignore
				}));
			}
		}
	}
}
