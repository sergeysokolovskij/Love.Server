using Api.Services.Exceptions;
using System;
using System.IO;
using System.Text;

namespace Api.Services.Logs
{
	public static class Logger
	{
		private const string logName = "logs.txt";
		private const string errorName = "errors.txt";
		private static string GetLogPathes(string type)
		{
			if (type == logName)
				return Path.Combine(Environment.CurrentDirectory, "Errors", logName);
			else if (type == errorName)
				return Path.Combine(Environment.CurrentDirectory, "Errors", errorName);
			throw new ApiError(new ServerException("Invalid log names"));
		}
		public static void WriteLog(string message)
		{
			var path = GetLogPathes(logName);
			File.AppendAllText(path, message);
		}
		public static void ErrorLog(Exception ex = null, string message = null)
		{
			var path = GetLogPathes(errorName);
			StringBuilder sb = new StringBuilder();

			sb.AppendLine(DateTime.Now.ToString());
			sb.AppendLine();

			if (ex != null)
			{
				sb.AppendLine("Данные по Exception:");
				sb.AppendLine(ex.Message);
				sb.AppendLine();
				sb.AppendLine(ex.StackTrace);
				if (ex.InnerException != null && !string.IsNullOrEmpty(ex.InnerException.Message))
					sb.AppendLine(ex.InnerException.Message);
				sb.AppendLine();
			}
			if (message != null)
				sb.AppendLine(message);
			var result = sb.ToString();
			File.AppendAllText(path, result);
		}
	}
}
