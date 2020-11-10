using Api.Models.Exceptions;
using System;

namespace Api.Services.Exceptions
{
	public class ApiError : BaseException
	{
		public ApiError(ServerException ex)
		{
			this.ex = ex;
		}
		public ServerException ex { get; set; }
	}
}
