using Newtonsoft.Json;
using System;

namespace Api.Services.Exceptions
{
	public enum ErrorTypes
	{
		Visible = 0,
		UnVisible = 1
	}

	public class ServerException
	{
		public int Code { get; set; }
		[JsonIgnore]
		public ErrorTypes ErrorType { get; set; }
		public string Description { get; set; }

		private string message;
		private string stackTrace;
		public string Message
		{
			get
			{
				if (ErrorsMode.GetErrorsMode().IsShowUnVisibleErrors || ErrorType == ErrorTypes.Visible)
					return message;
				return null;
			}
			set { }
		}
		public string StackTrace
		{
			get
			{
				if (ErrorsMode.GetErrorsMode().IsShowUnVisibleErrors || ErrorType == ErrorTypes.Visible)
					return stackTrace;
				return null;
			}
			set { }
		}
		public ServerException(string description, ErrorTypes type = ErrorTypes.Visible)
		{
			Code = 500;
			Description = description;
			ErrorType = type;
		}
		public ServerException(int code, string description, ErrorTypes type = ErrorTypes.Visible)
		{
			Code = code;
			Description = description;
			ErrorType = type;
		}
		public ServerException(int code, string description, string message ,ErrorTypes type)
		{
			Code = code;
			Description = description;
			this.message = message;
			ErrorType = type;
		}
		public ServerException(int code, string description, string message, ErrorTypes type, Exception ex)
		{
			Code = code;
			Description = description;
			ErrorType = type;
			this.message = message;
			stackTrace = ex.StackTrace;
		}
	}

	public class ErrorsMode
	{
		public bool IsShowUnVisibleErrors { get; private set; }
		private static ErrorsMode errorMode { get; set; }
		public static ErrorsMode GetErrorsMode()
		{
			return errorMode;
		}
		public static void SetErrorsMode(bool isShowUnvisibleException)
		{
			var temp = new ErrorsMode();
			temp.IsShowUnVisibleErrors = isShowUnvisibleException;
			errorMode = temp;
		}
	}
}
