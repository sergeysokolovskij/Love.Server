﻿using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace Api.Services.Brocker
{
	public interface IConnectionsProvider
	{
		IConnection GetConnection();
	}
	public class ConnectionsProvider : IConnectionsProvider
	{
		private readonly IConfiguration configuration;

		public ConnectionsProvider(IConfiguration configuration)
		{
			this.configuration = configuration;
		}

		public IConnection GetConnection()
		{
			ConnectionFactory connectionFactory = new ConnectionFactory();

			var rabbitSection = configuration.GetSection("RabbitMq");

			connectionFactory.HostName = rabbitSection.GetValue<string>("Host");
			connectionFactory.UserName = rabbitSection.GetValue<string>("UserName");
			connectionFactory.Password = rabbitSection.GetValue<string>("Password");
			connectionFactory.DispatchConsumersAsync = true;

			return connectionFactory.CreateConnection();
		}
	}
}
