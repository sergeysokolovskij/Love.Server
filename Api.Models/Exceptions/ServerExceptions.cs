using System;
using System.Collections.Generic;
using System.Text;

namespace Api.Services.Exceptions
{
	public class ServerExceptions
	{
		public static ServerException ServerError(Exception exception)
		{
			return new ServerException(500, "Ошибка API", exception.Message, ErrorTypes.UnVisible, exception);
		}
	}
}
