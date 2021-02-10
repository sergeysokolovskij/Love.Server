using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Api.Services.Brocker
{
	public interface IBrockerService
	{
		void PublishMessage(string queue, string message);
		void ReceiveMessage(string queue);
		void SubscribeToQuee(string queue);
	}
	public class BrockerService : IBrockerService
	{
		private readonly IConnectionsProvider connectionsProvider;
		private readonly ILogger logger;
		
		public BrockerService(IConnectionsProvider connectionsProvider,
			ILoggerFactory loggerFactory)
		{
			this.connectionsProvider = connectionsProvider;
			this.logger = loggerFactory.CreateLogger<BrockerService>();
		}

		public void PublishMessage(string queue, string message)
		{
			using (var connection = connectionsProvider.GetConnection().CreateModel())
			{
				connection.QueueDeclare(queue, false, false, false, null);
				connection.BasicPublish(string.Empty, queue, null, Encoding.UTF8.GetBytes(message));
			}
		}

		public void ReceiveMessage(string queue)
		{
			using (var connection = connectionsProvider.GetConnection().CreateModel())
			{
				connection.QueueDeclare(queue, false, false, false, null);
				var consumer = new EventingBasicConsumer(connection);
				var result = connection.BasicGet(queue, true);
				if (result != null)
				{
					string data = Encoding.UTF8.GetString(result.Body.ToArray());
					logger.LogInformation(data);
				}
			}
		}

		public void SubscribeToQuee(string queue)
		{
			using (var connection = connectionsProvider.GetConnection().CreateModel())
			{
				connection.ExchangeDeclare(queue, ExchangeType.Fanout);

				var subscriber = new AsyncEventingBasicConsumer(connection);
				subscriber.Received += (conn, ea) =>
				{
					logger.LogInformation(Encoding.UTF8.GetString(ea.Body.ToArray()));
					connection.BasicAck(ea.DeliveryTag, false);

					return Task.CompletedTask;
				};
				connection.BasicConsume(queue, false, subscriber);
			}
		}
	}
}
