using Api.Services.Brocker;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace Api.Services.Messanger.Accounting
{
	public interface IOnlineUserHandler
	{

	}
	public class OnlineUserHandler
	{
		private readonly IBrockerService brockerService;

		public OnlineUserHandler(IBrockerService brockerService)
		{
			this.brockerService = brockerService;
		}		
	}
}
