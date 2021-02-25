using Api.Services.Exceptions;
using System;
using System.IO;
using System.Text;

namespace Api.Services.Logs
{
	public static class Logger
	{
		private static string GetLogPathes(string logName)
		{
			return Path.Combine(Environment.CurrentDirectory, logName);
		}
		public static void WriteLog(string message)
		{
			string path = $"{GetLogPathes($"{DateTime.UtcNow}.txt")}";
			File.AppendAllText(path, message);
		}


		public static void ErrorLog(Exception ex = null, string message = null)
		{
			string path = $"{GetLogPathes($"{DateTime.UtcNow}.txt")}";

			if (!File.Exists(path))
				File.Create(path);

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
