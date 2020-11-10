using Api.Services.Exceptions;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Apis.Utils
{
	public static class HttpUtilities
	{
		public static string GetDataFromHeaders(HttpContext context, string key)
		{
			if (context.Request.Headers.ContainsKey(key))
				return context.Request.Headers[key];
			throw new ApiError(new ServerException(500, string.Format("Не удалось прочесть данные заголовка: '{0}'", key)));
		}

		public static bool MakeResponseHeader(HttpContext context, string headerName, string data)
		{
			return context.Response.Headers.TryAdd(headerName, data);
		}

		public static bool GetDataFromCookies(HttpContext context, string cookieName, out string result)
		{
			if (context.Request.Cookies.TryGetValue(cookieName, out result))
				return true;
			return false;
		}
	}
}
