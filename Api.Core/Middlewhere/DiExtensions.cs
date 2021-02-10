using Microsoft.AspNetCore.Builder;
using ShopPlatforms.Core.Middlewhere;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShopPlatform.Middlewhere
{
	public static class DiExtensions
	{
		public static void RegisterMiddleWheres(this IApplicationBuilder appBuilder)
		{
			appBuilder.UseMiddleware<XssProtectionMiddlewhere>();
			appBuilder.UseMiddleware<ExceptionMiddlewhere>();
		}
	}
}
