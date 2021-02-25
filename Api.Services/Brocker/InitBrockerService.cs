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
		void Init();
	}
	public class InitBrockerService : IBrockerService
	{
		private readonly IConnectionsProvider connectionsProvider;
		private readonly ILogger logger;
		
		public InitBrockerService(IConnectionsProvider connectionsProvider,
			ILoggerFactory loggerFactory)
		{
			this.connectionsProvider = connectionsProvider;
			this.logger = loggerFactory.CreateLogger<InitBrockerService>();
		}
	
		public void Init()
        {
			using (var connection = connectionsProvider.GetConnection().CreateModel())
            {
				// создаем точки обмена

				logger.LogInformation("Try to create exchanges...");

				connection.ExchangeDeclare("readedmessages", ExchangeType.Direct, true);
				connection.ExchangeDeclare("useronline", ExchangeType.Direct, true);
				connection.ExchangeDeclare("statistic", ExchangeType.Direct, true);

				logger.LogInformation("Exchanges created and ready to work");
            }
        }


		//public void SubscribeToQuee(string queue)
		//{
		//	var connection = connectionsProvider.GetConnection().CreateModel();

		//	connection.ExchangeDeclare("newexchange", ExchangeType.Fanout, false);
		//	connection.QueueBind(queue, "newexchange", string.Empty);

		//	var subscriber = new AsyncEventingBasicConsumer(connection);

		//	subscriber.Received += (conn, ea) =>
		//	{
		//		logger.LogInformation(Encoding.UTF8.GetString(ea.Body.ToArray()));
		//		return Task.CompletedTask;
		//	};
		//	connection.BasicConsume(queue, true, subscriber);
		//}

		//public void PublishMessage(string queue, string message)
		//{
		//	using (var connection = connectionsProvider.GetConnection().CreateModel())
		//	{
		//		connection.QueueDeclare(queue, false, false, false, null);
		//		connection.BasicPublish(string.Empty, queue, null, Encoding.UTF8.GetBytes(message));
		//	}
		//}


		//public void ReceiveMessage(string queue)
		//{
		//	using (var connection = connectionsProvider.GetConnection().CreateModel())
		//	{
		//		connection.QueueDeclare(queue, false, false, false, null);
		//		var consumer = new EventingBasicConsumer(connection);
		//		var result = connection.BasicGet(queue, true);
		//		if (result != null)
		//		{
		//			string data = Encoding.UTF8.GetString(result.Body.ToArray());
		//			logger.LogInformation(data);
		//		}
		//	}
		//}
	}
}
